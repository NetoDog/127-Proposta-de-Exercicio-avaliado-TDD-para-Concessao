using System;
using Xunit;
using Gamification.Domain.Policies;
using Gamification.Domain.Model;
using Gamification.Domain.Exceptions;

namespace Gamification.Domain.Tests;
public class BonusPolicyTests
{
    [Fact]
    public void Calculate_returns_full_weight_when_before_or_equal_fullEnd()
    {
        var now = DateTimeOffset.UtcNow;
        var bonusStart = now.AddDays(-1);
        var fullEnd = now.AddMinutes(1);
        var finalEnd = now.AddDays(1);
        var (xp, reason) = BonusPolicy.Calculate(now, bonusStart, fullEnd, finalEnd, new XpAmount(100), new XpAmount(50));
        Assert.Equal(100, xp.Value);
        Assert.Equal("janela integral", reason);
    }

    [Fact]
    public void Calculate_returns_reduced_between_full_and_final()
    {
        var now = DateTimeOffset.UtcNow.AddDays(1);
        var bonusStart = now.AddDays(-10);
        var fullEnd = now.AddDays(-1);
        var finalEnd = now.AddDays(10);
        var (xp, reason) = BonusPolicy.Calculate(now, bonusStart, fullEnd, finalEnd, new XpAmount(100), new XpAmount(50));
        Assert.Equal(50, xp.Value);
        Assert.Equal("janela reduzida", reason);
    }

    [Fact]
    public void Calculate_returns_zero_after_final()
    {
        var now = DateTimeOffset.UtcNow.AddDays(20);
        var bonusStart = now.AddDays(-30);
        var fullEnd = now.AddDays(-10);
        var finalEnd = now.AddDays(-1);
        var (xp, reason) = BonusPolicy.Calculate(now, bonusStart, fullEnd, finalEnd, new XpAmount(100), new XpAmount(50));
        Assert.Equal(0, xp.Value);
        Assert.Equal("fora da janela de bônus", reason);
    }

    [Fact]
    public void Calculate_throws_on_inconsistent_dates()
    {
        var now = DateTimeOffset.UtcNow;
        var bonusStart = now.AddDays(10);
        var fullEnd = now.AddDays(5);
        var finalEnd = now.AddDays(1);
        Assert.Throws<ConfigurationException>(() =>
            BonusPolicy.Calculate(now, bonusStart, fullEnd, finalEnd, new XpAmount(100), new XpAmount(50))
        );
    }
}
