using System;

namespace Gamification.Domain.Model;
public record RewardLog(
    Guid StudentId,
    string BadgeSlug,
    string Reason,
    DateTimeOffset Timestamp,
    string Source = "mission_completion"
);
