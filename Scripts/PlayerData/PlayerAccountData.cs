using System;
using System.Collections.Generic;

[Serializable]
public class PlayerAccountData
{
    public ProfileData profile = new();
    public InventoryData inventory = new();
    public ProgressData progress = new();
    
    [Serializable]
    public class ProfileData
    {
        public string playerId;
        public string playerName;

        public int energy;
        public int gold;
        public int diamond;

        public int bestScore;
        public bool tutorialFinished;
    }

    [Serializable]
    public class InventoryData
    {
        public List<InventoryItemStack> consumables = new();
        public List<string> ownedCardSkinIds = new();
        public List<string> ownedBoardSkinIds = new();
    }

    [Serializable]
    public class ProgressData
    {
        public List<string> unlockedAchievementIds = new();
        public List<string> claimedAchievementRewardIds = new();

        public int totalPlayCount;
        public int totalClearedCellCount;
        public int totalUseItemCount;
    }
}