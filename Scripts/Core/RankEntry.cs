using System;

[Serializable]
public class RankEntry
{
    public string playerId;
    public string displayName;
    public int bestScore;
    public bool isSelf;

    public RankEntry(string playerId, string displayName, int bestScore, bool isSelf = false)
    {
        this.playerId = playerId;
        this.displayName = displayName;
        this.bestScore = bestScore;
        this.isSelf = isSelf;
    }
}