using System.ComponentModel.DataAnnotations;

namespace LostAndFound.Api.Models;

public sealed class Item
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(120)]
    public required string Name { get; set; }

    [MaxLength(800)]
    public required string Description { get; set; }

    [MaxLength(80)]
    public required string Category { get; set; }

    [MaxLength(120)]
    public required string Location { get; set; }

    [MaxLength(120)]
    public required string Faculty { get; set; }

    public DateTimeOffset ReportDate { get; set; }

    public ItemType ItemType { get; set; }

    public ItemStatus Status { get; set; }

    [MaxLength(500)]
    public string? PhotoUrl { get; set; }

    [MaxLength(1000)]
    public required string Characteristics { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public List<Claim> Claims { get; set; } = [];

    public ReturnRecord? ReturnRecord { get; set; }
}
