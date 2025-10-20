namespace Gamification.Domain.Model;
public record XpAmount(int Value)
{
    public static XpAmount Zero => new XpAmount(0);
}
