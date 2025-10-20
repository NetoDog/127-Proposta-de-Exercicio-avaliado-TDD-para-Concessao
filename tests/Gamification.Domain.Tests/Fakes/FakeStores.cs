using System;
using Gamification.Domain.Ports;
using Gamification.Domain.Model;
using System.Collections.Generic;

namespace Gamification.Domain.Tests.Fakes;
public class FakeAwardsReadStore : IAwardsReadStore
{
    private readonly bool _concluida;
    private readonly bool _jaPossui;
    private readonly (DateTimeOffset bonusStart, DateTimeOffset fullEnd, DateTimeOffset finalEnd)? _dates;
    private readonly HashSet<string> _processedRequestIds = new();

    public FakeAwardsReadStore(bool concluida = true, bool jaPossui = false,
        (DateTimeOffset, DateTimeOffset, DateTimeOffset)? dates = null)
    {
        _concluida = concluida;
        _jaPossui = jaPossui;
        _dates = dates;
    }

    public bool MissaoConcluida(Guid studentId, Guid missionId) => _concluida;
    public bool JaPossuiBadge(Guid studentId, Guid missionId, string badgeSlug) => _jaPossui;
    public (DateTimeOffset bonusStart, DateTimeOffset fullEnd, DateTimeOffset finalEnd)? ObterDatasDoTema(Guid missionId) => _dates;

    public void MarkRequestIdProcessed(string requestId) => _processedRequestIds.Add(requestId);
    public bool RequestIdProcessed(string requestId) => !string.IsNullOrWhiteSpace(requestId) && _processedRequestIds.Contains(requestId);
}

public class FakeAwardsWriteStore : IAwardsWriteStore
{
    public bool ConcessaoGravada { get; private set; } = false;
    public Badge? LastBadge { get; private set; }
    public XpAmount? LastXp { get; private set; }
    public Gamification.Domain.Model.RewardLog? LastLog { get; private set; }

    public void SalvarConcessaoAtomica(Badge badge, XpAmount? xp, Gamification.Domain.Model.RewardLog log, string? requestId = null)
    {
        // simulate atomic write; could throw to simulate failure
        ConcessaoGravada = true;
        LastBadge = badge;
        LastXp = xp;
        LastLog = log;
    }
}
