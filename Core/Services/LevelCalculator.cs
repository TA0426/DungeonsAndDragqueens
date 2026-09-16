namespace OppgaveUkeEnModul3.Core.Services;

public class LevelCalculator
{
    public int CalculateLevel(int xp)
    {
        return xp / 100 + 1;
    }
}
