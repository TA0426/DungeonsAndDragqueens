namespace DungeonsAndDragqueens.Core;

public static class GearStats
{
    public static int AtLevel(int baseValue, int level) => baseValue * Math.Clamp(level, 1, 3);
}
