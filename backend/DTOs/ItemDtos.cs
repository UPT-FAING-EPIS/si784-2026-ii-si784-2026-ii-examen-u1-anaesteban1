using System.ComponentModel.DataAnnotations;
using LostAndFound.Api.Models;

namespace LostAndFound.Api.DTOs;

/// <summary>Payload for reporting a lost or found university item.</summary>
public sealed record CreateItemRequest
{
    [Required, MaxLength(120)]
    public required string Name { get; init; }

    [Required, MaxLength(800)]
    public required string Description { get; init; }

    [Required, MaxLength(80)]
    public required string Category { get; init; }

    [Required, MaxLength(120)]
    public required string Location { get; init; }

    [Required, MaxLength(120)]
    public required string Faculty { get; init; }

    public DateTimeOffset ReportDate { get; init; } = DateTimeOffset.UtcNow;

    [Url, MaxLength(500)]
    public string? PhotoUrl { get; init; }

    [Required, MaxLength(1000)]
    public required string Characteristics { get; init; }
}

/// <summary>Payload for claiming ownership of an item.</summary>
public sealed record CreateClaimRequest
{
    [Required, MaxLength(160)]
    public required string ClaimantName { get; init; }

    [Required, EmailAddress, MaxLength(200)]
    public required string ClaimantEmail { get; init; }

    [Required, MaxLength(1000)]
    public required string OwnershipEvidence { get; init; }
}

/// <summary>Payload for verifying an owner claim.</summary>
public sealed record VerifyOwnerRequest
{
    [Required]
    public required Guid ClaimId { get; init; }
}

/// <summary>Payload for registering item return.</summary>
public sealed record ReturnItemRequest
{
    [Required]
    public required Guid ClaimId { get; init; }

    [Required, MaxLength(160)]
    public required string ResponsiblePerson { get; init; }

    public DateTimeOffset ReturnDate { get; init; } = DateTimeOffset.UtcNow;

    public bool OwnerConfirmation { get; init; }

    [MaxLength(1000)]
    public string? Notes { get; init; }
}

/// <summary>Represents an item returned by the API.</summary>
public sealed record ItemResponse(
    Guid Id,
    string Name,
    string Description,
    string Category,
    string Location,
    string Faculty,
    DateTimeOffset ReportDate,
    ItemType ItemType,
    ItemStatus Status,
    string? PhotoUrl,
    string Characteristics,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyList<ClaimResponse> Claims,
    ReturnRecordResponse? ReturnRecord);

/// <summary>Represents an item claim returned by the API.</summary>
public sealed record ClaimResponse(
    Guid Id,
    Guid ItemId,
    string ClaimantName,
    string ClaimantEmail,
    string OwnershipEvidence,
    DateTimeOffset RequestedAt,
    bool IsVerified,
    DateTimeOffset? VerifiedAt);

/// <summary>Represents a return record returned by the API.</summary>
public sealed record ReturnRecordResponse(
    Guid Id,
    Guid ItemId,
    Guid ClaimId,
    string ResponsiblePerson,
    DateTimeOffset ReturnDate,
    bool OwnerConfirmation,
    string? Notes);
