using Gamification.Domain.Model;

namespace Gamification.Domain.Ports;
public interface IAwardsWriteStore
{
    void SalvarConcessaoAtomica(Badge badge, XpAmount? xp, RewardLog log, string? requestId = null);
}
