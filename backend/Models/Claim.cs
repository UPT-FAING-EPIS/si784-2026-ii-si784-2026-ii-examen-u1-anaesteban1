using System.ComponentModel.DataAnnotations;

namespace LostAndFound.Api.Models;

public sealed class Claim
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ItemId { get; set; }

    public Item? Item { get; set; }

    [MaxLength(160)]
    public required string ClaimantName { get; set; }

    [EmailAddress, MaxLength(200)]
    public required string ClaimantEmail { get; set; }

    [MaxLength(1000)]
    public required string OwnershipEvidence { get; set; }

    public DateTimeOffset RequestedAt { get; set; } = DateTimeOffset.UtcNow;

    public bool IsVerified { get; set; }

    public DateTimeOffset? VerifiedAt { get; set; }

    public ReturnRecord? ReturnRecord { get; set; }
}
