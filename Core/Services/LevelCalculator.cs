namespace DungeonsAndDragqueens.Core.Services;

public class LevelCalculator
{
    public int CalculateLevel(int xp)
    {
        return xp / 100 + 1;
    }
}
