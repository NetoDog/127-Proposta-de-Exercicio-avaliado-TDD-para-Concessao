using System;
using Xunit;
using Gamification.Domain.Awards;
using Gamification.Domain.Tests.Fakes;
using Gamification.Domain.Model;
using Gamification.Domain.Exceptions;

namespace Gamification.Domain.Tests;
public class AwardBadgeServiceTests
{
    [Fact]
    public void ConcederBadge_quando_missao_concluida_concede_uma_unica_vez()
    {
        var studentId = Guid.NewGuid();
        var missionId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var dates = (now.AddDays(-1), now.AddDays(1), now.AddDays(2));
        var read = new FakeAwardsReadStore(concluida: true, jaPossui: false, dates: dates);
        var write = new FakeAwardsWriteStore();
        var service = new AwardBadgeService(read, write);

        service.AwardBadge(studentId, missionId, "math_master", now);

        Assert.True(write.ConcessaoGravada);
        Assert.Equal("math_master", write.LastBadge?.Slug);
        Assert.NotNull(write.LastXp);
    }

    [Fact]
    public void RepetirConcessao_mesma_badge_para_mesmo_estudante_deve_ser_idempotente_por_chave_natural()
    {
        var studentId = Guid.NewGuid();
        var missionId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var dates = (now.AddDays(-1), now.AddDays(1), now.AddDays(2));
        // First call: not yet has badge
        var read1 = new FakeAwardsReadStore(concluida: true, jaPossui: false, dates: dates);
        var write1 = new FakeAwardsWriteStore();
        var service1 = new AwardBadgeService(read1, write1);
        service1.AwardBadge(studentId, missionId, "math_master", now);

        // Simulate that now the store reports already has badge
        var read2 = new FakeAwardsReadStore(concluida: true, jaPossui: true, dates: dates);
        var write2 = new FakeAwardsWriteStore();
        var service2 = new AwardBadgeService(read2, write2);

        Assert.Throws<AlreadyAwardedException>(() => service2.AwardBadge(studentId, missionId, "math_master", now));
    }

    [Fact]
    public void ConcederBadge_sem_concluir_missao_deve_falhar()
    {
        var studentId = Guid.NewGuid();
        var missionId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var read = new FakeAwardsReadStore(concluida: false, jaPossui: false, dates: null);
        var write = new FakeAwardsWriteStore();
        var service = new AwardBadgeService(read, write);

        Assert.Throws<NotEligibleException>(() => service.AwardBadge(studentId, missionId, "noob", now));
        Assert.False(write.ConcessaoGravada);
    }

    [Fact]
    public void ConcederBadge_ate_bonusFullWeightEndDate_concede_bonus_integral()
    {
        var studentId = Guid.NewGuid();
        var missionId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var dates = (now.AddDays(-2), now.AddDays(1), now.AddDays(10));
        var read = new FakeAwardsReadStore(concluida: true, jaPossui: false, dates: dates);
        var write = new FakeAwardsWriteStore();
        var service = new AwardBadgeService(read, write);

        service.AwardBadge(studentId, missionId, "fast_solver", now);

        Assert.Equal(100, write.LastXp?.Value);
        Assert.Equal("janela integral", write.LastLog?.Reason);
    }

    [Fact]
    public void ConcederBadge_entre_fullWeight_e_finalDate_concede_bonus_reduzido()
    {
        var studentId = Guid.NewGuid();
        var missionId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow.AddDays(5);
        var dates = (now.AddDays(-10), now.AddDays(-1), now.AddDays(10));
        var read = new FakeAwardsReadStore(concluida: true, jaPossui: false, dates: dates);
        var write = new FakeAwardsWriteStore();
        var service = new AwardBadgeService(read, write);

        service.AwardBadge(studentId, missionId, "late_bird", now);

        Assert.Equal(50, write.LastXp?.Value);
        Assert.Equal("janela reduzida", write.LastLog?.Reason);
    }

    [Fact]
    public void ConcederBadge_apos_bonusFinalDate_nao_concede_bonus_mas_concede_badge()
    {
        var studentId = Guid.NewGuid();
        var missionId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow.AddDays(20);
        var dates = (now.AddDays(-30), now.AddDays(-10), now.AddDays(-1));
        var read = new FakeAwardsReadStore(concluida: true, jaPossui: false, dates: dates);
        var write = new FakeAwardsWriteStore();
        var service = new AwardBadgeService(read, write);

        service.AwardBadge(studentId, missionId, "very_late", now);

        Assert.Null(write.LastXp);
        Assert.Equal("fora da janela de bônus", write.LastLog?.Reason);
    }
}
