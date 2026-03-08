using System.Collections.Generic;

[System.Serializable]
public class PlayerProgressData
{
    public List<string> unlockedAchievementIds = new();
    public List<string> claimedAchievementRewardIds = new();

    public int totalPlayCount;
    public int totalClearedCellCount;
    public int totalUseItemCount;
}