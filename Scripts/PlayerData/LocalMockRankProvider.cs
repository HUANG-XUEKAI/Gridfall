using System.Collections.Generic;
using System.Linq;

public static class LocalMockRankProvider
{
    private static readonly List<RankEntry> fakeFriends = new()
    {
        new RankEntry("friend_001", "好友a", 12800),
        new RankEntry("friend_002", "好友b", 9600),
        new RankEntry("friend_003", "好友c", 8200),
        new RankEntry("friend_004", "好友d", 6500),
        new RankEntry("friend_005", "好友e", 4200),
        new RankEntry("friend_006", "好友f", 2100),
    };

    public static List<RankEntry> GetRankList()
    {
        List<RankEntry> list = new();

        var adc = AccountDataCenter.Instance;

        string selfId = "LocalPlayer";
        string selfName = "Player";
        int selfBestScore = 0;

        if (adc != null && adc.Profile != null)
        {
            selfId = string.IsNullOrEmpty(adc.Profile.playerId) ? "LocalPlayer" : adc.Profile.playerId;
            selfName = string.IsNullOrEmpty(adc.Profile.playerName) ? "Player" : adc.Profile.playerName;
            selfBestScore = adc.Profile.bestScore;
        }

        list.Add(new RankEntry(selfId, selfName, selfBestScore, true));

        foreach (var friend in fakeFriends)
        {
            if (friend == null) 
                continue;

            if (friend.playerId == selfId)
                continue;

            list.Add(new RankEntry(friend.playerId, friend.displayName, friend.bestScore, false));
        }

        return list
            .OrderByDescending(x => x.bestScore)
            .ThenBy(x => x.playerId)
            .ToList();
    }
}