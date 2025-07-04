﻿using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.Api.Responses.EntranceTest;

public record EntranceTestResponse
{
    public string Name { get; init; } = default!;
    public required Guid Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }

    public DateTime? DeletedAt { get; init; }

    public RecordStatus Status { get; init; }

    public required Guid RoomId { get; init; }

    public string? RoomName { get; init; }

    public int? RoomCapacity { get; init; }

    public required Shift Shift { get; init; }

    public required DateOnly Date { get; init; }

    public bool IsAnnouncedScore { get; init; }

    public string? InstructorId { get; init; }

    public string? InstructorName { get; init; }

    public required string CreatedById { get; init; }

    public int RegisterStudents { get; init; }

    public EntranceTestStatus TestStatus { get; init; }
}