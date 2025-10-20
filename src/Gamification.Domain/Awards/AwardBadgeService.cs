using System;
using Gamification.Domain.Model;
using Gamification.Domain.Ports;
using Gamification.Domain.Policies;
using Gamification.Domain.Exceptions;

namespace Gamification.Domain.Awards;
public class AwardBadgeService
{
    private readonly IAwardsReadStore _read;
    private readonly IAwardsWriteStore _write;

    public AwardBadgeService(IAwardsReadStore read, IAwardsWriteStore write)
    {
        _read = read;
        _write = write;
    }

    public void AwardBadge(Guid studentId, Guid missionId, string badgeSlug, DateTimeOffset now, string? requestId = null)
    {
        // Idempotency via requestId
        if (!string.IsNullOrWhiteSpace(requestId) && _read.RequestIdProcessed(requestId))
        {
            // silently return (or could return previous result)
            return;
        }

        if (!_read.MissaoConcluida(studentId, missionId))
            throw new NotEligibleException();

        if (_read.JaPossuiBadge(studentId, missionId, badgeSlug))
            throw new AlreadyAwardedException();

        var dates = _read.ObterDatasDoTema(missionId);
        if (dates is null)
            throw new ThemeNotConfiguredException();

        var (bonusStart, fullEnd, finalEnd) = dates.Value;

        var (xp, reason) = BonusPolicy.Calculate(now, bonusStart, fullEnd, finalEnd,
            new XpAmount(100), new XpAmount(50));

        var badge = new Badge(badgeSlug, "Badge da Missão");
        var log = new RewardLog(studentId, badgeSlug, reason, now);

        // Atomic save - write store is responsible for atomicity; exceptions bubble up
        _write.SalvarConcessaoAtomica(badge, xp.Value == 0 ? null : xp, log, requestId);
    }
}
