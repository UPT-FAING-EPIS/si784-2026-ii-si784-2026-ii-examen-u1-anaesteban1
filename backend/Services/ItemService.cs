using LostAndFound.Api.Data;
using LostAndFound.Api.DTOs;
using LostAndFound.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LostAndFound.Api.Services;

public sealed class ItemService(LostAndFoundDbContext dbContext) : IItemService
{
    public Task<ItemResponse> CreateLostItemAsync(CreateItemRequest request, CancellationToken cancellationToken)
    {
        return CreateItemAsync(request, ItemType.Lost, ItemStatus.Reported, cancellationToken);
    }

    public Task<ItemResponse> CreateFoundItemAsync(CreateItemRequest request, CancellationToken cancellationToken)
    {
        return CreateItemAsync(request, ItemType.Found, ItemStatus.InCustody, cancellationToken);
    }

    public async Task<IReadOnlyList<ItemResponse>> GetItemsAsync(
        ItemStatus? status,
        string? location,
        string? faculty,
        DateOnly? date,
        CancellationToken cancellationToken)
    {
        IQueryable<Item> query = dbContext.Items
            .AsNoTracking()
            .Include(item => item.Claims)
            .Include(item => item.ReturnRecord);

        if (status is not null)
        {
            query = query.Where(item => item.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            var normalizedLocation = location.Trim().ToLowerInvariant();
            query = query.Where(item => item.Location.ToLower().Contains(normalizedLocation));
        }

        if (!string.IsNullOrWhiteSpace(faculty))
        {
            var normalizedFaculty = faculty.Trim().ToLowerInvariant();
            query = query.Where(item => item.Faculty.ToLower().Contains(normalizedFaculty));
        }

        if (date is not null)
        {
            var start = new DateTimeOffset(date.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc));
            var end = new DateTimeOffset(date.Value.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc));
            query = query.Where(item => item.ReportDate >= start && item.ReportDate < end);
        }

        var items = await query.OrderByDescending(item => item.ReportDate).ToListAsync(cancellationToken);
        return items.Select(MapItem).ToList();
    }

    public async Task<ItemResponse?> GetItemAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = await dbContext.Items
            .AsNoTracking()
            .Include(entity => entity.Claims)
            .Include(entity => entity.ReturnRecord)
            .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);

        return item is null ? null : MapItem(item);
    }

    public async Task<ClaimResponse> ClaimItemAsync(Guid itemId, CreateClaimRequest request, CancellationToken cancellationToken)
    {
        var item = await dbContext.Items
            .Include(entity => entity.Claims)
            .FirstOrDefaultAsync(entity => entity.Id == itemId, cancellationToken);

        if (item is null)
        {
            throw new KeyNotFoundException("Item not found.");
        }

        if (item.Status == ItemStatus.Returned)
        {
            throw new InvalidOperationException("Returned items cannot be claimed.");
        }

        if (string.IsNullOrWhiteSpace(request.OwnershipEvidence))
        {
            throw new ArgumentException("Ownership evidence is required.");
        }

        var claim = new Claim
        {
            ItemId = item.Id,
            ClaimantName = request.ClaimantName.Trim(),
            ClaimantEmail = request.ClaimantEmail.Trim(),
            OwnershipEvidence = request.OwnershipEvidence.Trim()
        };

        dbContext.Claims.Add(claim);
        item.Status = ItemStatus.Claimed;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapClaim(claim);
    }

    public async Task<ClaimResponse?> VerifyOwnerAsync(Guid itemId, VerifyOwnerRequest request, CancellationToken cancellationToken)
    {
        var item = await dbContext.Items
            .Include(entity => entity.Claims)
            .FirstOrDefaultAsync(entity => entity.Id == itemId, cancellationToken);

        if (item is null)
        {
            return null;
        }

        var claim = item.Claims.FirstOrDefault(entity => entity.Id == request.ClaimId);
        if (claim is null)
        {
            return null;
        }

        if (item.Status != ItemStatus.Claimed)
        {
            throw new InvalidOperationException("Only claimed items can be verified.");
        }

        claim.IsVerified = true;
        claim.VerifiedAt = DateTimeOffset.UtcNow;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapClaim(claim);
    }

    public async Task<ReturnRecordResponse?> ReturnItemAsync(Guid itemId, ReturnItemRequest request, CancellationToken cancellationToken)
    {
        var item = await dbContext.Items
            .Include(entity => entity.Claims)
            .Include(entity => entity.ReturnRecord)
            .FirstOrDefaultAsync(entity => entity.Id == itemId, cancellationToken);

        if (item is null)
        {
            return null;
        }

        if (item.ReturnRecord is not null || item.Status == ItemStatus.Returned)
        {
            throw new InvalidOperationException("Item was already returned.");
        }

        var claim = item.Claims.FirstOrDefault(entity => entity.Id == request.ClaimId);
        if (claim is null)
        {
            return null;
        }

        if (!claim.IsVerified)
        {
            throw new InvalidOperationException("The owner must be verified before returning the item.");
        }

        if (!request.OwnerConfirmation)
        {
            throw new ArgumentException("Owner confirmation is required.");
        }

        var returnRecord = new ReturnRecord
        {
            ItemId = item.Id,
            ClaimId = claim.Id,
            ResponsiblePerson = request.ResponsiblePerson.Trim(),
            ReturnDate = request.ReturnDate,
            OwnerConfirmation = request.OwnerConfirmation,
            Notes = request.Notes?.Trim()
        };

        dbContext.ReturnRecords.Add(returnRecord);
        item.Status = ItemStatus.Returned;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapReturnRecord(returnRecord);
    }

    private async Task<ItemResponse> CreateItemAsync(
        CreateItemRequest request,
        ItemType itemType,
        ItemStatus status,
        CancellationToken cancellationToken)
    {
        if (request.ReportDate > DateTimeOffset.UtcNow.AddMinutes(5))
        {
            throw new ArgumentException("Report date cannot be in the future.");
        }

        var now = DateTimeOffset.UtcNow;
        var item = new Item
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Category = request.Category.Trim(),
            Location = request.Location.Trim(),
            Faculty = request.Faculty.Trim(),
            ReportDate = request.ReportDate,
            ItemType = itemType,
            Status = status,
            PhotoUrl = request.PhotoUrl?.Trim(),
            Characteristics = request.Characteristics.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.Items.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);
        return MapItem(item);
    }

    private static ItemResponse MapItem(Item item)
    {
        return new ItemResponse(
            item.Id,
            item.Name,
            item.Description,
            item.Category,
            item.Location,
            item.Faculty,
            item.ReportDate,
            item.ItemType,
            item.Status,
            item.PhotoUrl,
            item.Characteristics,
            item.CreatedAt,
            item.UpdatedAt,
            item.Claims.Select(MapClaim).ToList(),
            item.ReturnRecord is null ? null : MapReturnRecord(item.ReturnRecord));
    }

    private static ClaimResponse MapClaim(Claim claim)
    {
        return new ClaimResponse(
            claim.Id,
            claim.ItemId,
            claim.ClaimantName,
            claim.ClaimantEmail,
            claim.OwnershipEvidence,
            claim.RequestedAt,
            claim.IsVerified,
            claim.VerifiedAt);
    }

    private static ReturnRecordResponse MapReturnRecord(ReturnRecord returnRecord)
    {
        return new ReturnRecordResponse(
            returnRecord.Id,
            returnRecord.ItemId,
            returnRecord.ClaimId,
            returnRecord.ResponsiblePerson,
            returnRecord.ReturnDate,
            returnRecord.OwnerConfirmation,
            returnRecord.Notes);
    }
}
