using System;

namespace Gamification.Domain.Ports;
public interface IAwardsReadStore
{
    bool MissaoConcluida(Guid studentId, Guid missionId);
    bool JaPossuiBadge(Guid studentId, Guid missionId, string badgeSlug);
    (DateTimeOffset bonusStart, DateTimeOffset fullEnd, DateTimeOffset finalEnd)? ObterDatasDoTema(Guid missionId);
    bool RequestIdProcessed(string requestId);
}
