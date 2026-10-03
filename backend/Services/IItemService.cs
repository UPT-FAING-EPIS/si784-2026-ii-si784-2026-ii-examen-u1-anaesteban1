using LostAndFound.Api.DTOs;
using LostAndFound.Api.Models;

namespace LostAndFound.Api.Services;

public interface IItemService
{
    Task<ItemResponse> CreateLostItemAsync(CreateItemRequest request, CancellationToken cancellationToken);

    Task<ItemResponse> CreateFoundItemAsync(CreateItemRequest request, CancellationToken cancellationToken);

    Task<IReadOnlyList<ItemResponse>> GetItemsAsync(
        ItemStatus? status,
        string? location,
        string? faculty,
        DateOnly? date,
        CancellationToken cancellationToken);

    Task<ItemResponse?> GetItemAsync(Guid id, CancellationToken cancellationToken);

    Task<ClaimResponse> ClaimItemAsync(Guid itemId, CreateClaimRequest request, CancellationToken cancellationToken);

    Task<ClaimResponse?> VerifyOwnerAsync(Guid itemId, VerifyOwnerRequest request, CancellationToken cancellationToken);

    Task<ReturnRecordResponse?> ReturnItemAsync(Guid itemId, ReturnItemRequest request, CancellationToken cancellationToken);
}
