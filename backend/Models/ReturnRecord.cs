using System.ComponentModel.DataAnnotations;

namespace LostAndFound.Api.Models;

public sealed class ReturnRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ItemId { get; set; }

    public Item? Item { get; set; }

    public Guid ClaimId { get; set; }

    public Claim? Claim { get; set; }

    [MaxLength(160)]
    public required string ResponsiblePerson { get; set; }

    public DateTimeOffset ReturnDate { get; set; } = DateTimeOffset.UtcNow;

    public bool OwnerConfirmation { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}
