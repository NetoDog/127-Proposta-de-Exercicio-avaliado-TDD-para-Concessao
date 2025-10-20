using System;
using Gamification.Domain.Model;

namespace Gamification.Domain.Policies;
public static class BonusPolicy
{
    public static (XpAmount xp, string reason) Calculate(
        DateTimeOffset now,
        DateTimeOffset bonusStart,
        DateTimeOffset fullWeightEnd,
        DateTimeOffset finalEnd,
        XpAmount xpFull,
        XpAmount xpReduced)
    {
        if (bonusStart > finalEnd || bonusStart > fullWeightEnd || fullWeightEnd > finalEnd)
            throw new Exceptions.ConfigurationException("Datas de bônus inconsistentes.");

        if (now <= fullWeightEnd)
            return (xpFull, "janela integral");
        if (now <= finalEnd)
            return (xpReduced, "janela reduzida");
        return (XpAmount.Zero, "fora da janela de bônus");
    }
}
