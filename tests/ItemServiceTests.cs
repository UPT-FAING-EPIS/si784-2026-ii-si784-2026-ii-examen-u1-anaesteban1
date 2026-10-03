using LostAndFound.Api.Data;
using LostAndFound.Api.DTOs;
using LostAndFound.Api.Models;
using LostAndFound.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace LostAndFound.Tests;

public sealed class ItemServiceTests
{
    [Fact]
    public async Task CreateLostItemAsync_CreatesReportedLostItem()
    {
        var service = CreateService();
        var item = await service.CreateLostItemAsync(NewItemRequest(), CancellationToken.None);

        Assert.Equal(ItemType.Lost, item.ItemType);
        Assert.Equal(ItemStatus.Reported, item.Status);
        Assert.NotEqual(Guid.Empty, item.Id);
    }

    [Fact]
    public async Task GetItemsAsync_ReturnsCreatedItems()
    {
        var service = CreateService();
        await service.CreateFoundItemAsync(NewItemRequest(location: "Biblioteca Central"), CancellationToken.None);

        var items = await service.GetItemsAsync(ItemStatus.InCustody, "biblioteca", null, null, CancellationToken.None);

        Assert.Single(items);
        Assert.Equal(ItemType.Found, items[0].ItemType);
    }

    [Fact]
    public async Task ClaimItemAsync_CreatesClaimAndMarksItemClaimed()
    {
        var service = CreateService();
        var item = await service.CreateFoundItemAsync(NewItemRequest(), CancellationToken.None);

        var claim = await service.ClaimItemAsync(item.Id, NewClaimRequest(), CancellationToken.None);
        var updated = await service.GetItemAsync(item.Id, CancellationToken.None);

        Assert.Equal(item.Id, claim.ItemId);
        Assert.Equal(ItemStatus.Claimed, updated!.Status);
    }

    [Fact]
    public async Task VerifyOwnerAsync_VerifiesClaim()
    {
        var service = CreateService();
        var item = await service.CreateFoundItemAsync(NewItemRequest(), CancellationToken.None);
        var claim = await service.ClaimItemAsync(item.Id, NewClaimRequest(), CancellationToken.None);

        var verified = await service.VerifyOwnerAsync(item.Id, new VerifyOwnerRequest { ClaimId = claim.Id }, CancellationToken.None);

        Assert.NotNull(verified);
        Assert.True(verified.IsVerified);
        Assert.NotNull(verified.VerifiedAt);
    }

    [Fact]
    public async Task ReturnItemAsync_ReturnsVerifiedItem()
    {
        var service = CreateService();
        var item = await service.CreateFoundItemAsync(NewItemRequest(), CancellationToken.None);
        var claim = await service.ClaimItemAsync(item.Id, NewClaimRequest(), CancellationToken.None);
        await service.VerifyOwnerAsync(item.Id, new VerifyOwnerRequest { ClaimId = claim.Id }, CancellationToken.None);

        var returned = await service.ReturnItemAsync(item.Id, new ReturnItemRequest
        {
            ClaimId = claim.Id,
            ResponsiblePerson = "Oficina de Bienestar",
            OwnerConfirmation = true,
            Notes = "Documento firmado por el estudiante."
        }, CancellationToken.None);
        var updated = await service.GetItemAsync(item.Id, CancellationToken.None);

        Assert.NotNull(returned);
        Assert.Equal(ItemStatus.Returned, updated!.Status);
    }

    [Fact]
    public async Task ReturnItemAsync_RejectsUnverifiedOwner()
    {
        var service = CreateService();
        var item = await service.CreateFoundItemAsync(NewItemRequest(), CancellationToken.None);
        var claim = await service.ClaimItemAsync(item.Id, NewClaimRequest(), CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ReturnItemAsync(item.Id, new ReturnItemRequest
            {
                ClaimId = claim.Id,
                ResponsiblePerson = "Oficina de Bienestar",
                OwnerConfirmation = true
            }, CancellationToken.None));
    }

    private static ItemService CreateService()
    {
        var options = new DbContextOptionsBuilder<LostAndFoundDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ItemService(new LostAndFoundDbContext(options));
    }

    private static CreateItemRequest NewItemRequest(string location = "Pabellon A")
    {
        return new CreateItemRequest
        {
            Name = "Mochila negra",
            Description = "Mochila con cuadernos y calculadora.",
            Category = "Accesorios",
            Location = location,
            Faculty = "Ingenieria",
            ReportDate = DateTimeOffset.UtcNow,
            Characteristics = "Tiene un llavero azul."
        };
    }

    private static CreateClaimRequest NewClaimRequest()
    {
        return new CreateClaimRequest
        {
            ClaimantName = "Ana Esteban",
            ClaimantEmail = "ana.esteban@example.edu",
            OwnershipEvidence = "Describe el llavero azul y el contenido interno."
        };
    }
}
