using System.Collections.Generic;

public class EvaluationResult
{
    public CardPattern pattern = CardPattern.None;
    public BasicPattern activePattern = null;
}

public static class HandEvaluator
{
    public static EvaluationResult Evaluate(List<CardView> normalCards)
    {
        var result = new EvaluationResult();

        if (normalCards == null || normalCards.Count == 0)
        {
            result.pattern = CardPattern.None;
            result.activePattern = null;
            return result;
        }

        // 收集有效 shape
        var counts = new Dictionary<BasicPattern, int>();
        var firstIndexMap = new Dictionary<BasicPattern, int>();

        for (int i = 0; i < normalCards.Count; i++)
        {
            var pattern = normalCards[i].Card.pattern;
            if (pattern == null) continue;

            if (!counts.ContainsKey(pattern))
            {
                counts[pattern] = 0;
                firstIndexMap[pattern] = i; // 记录该 shape 第一次出现的位置，用于平手时稳定决策
            }

            counts[pattern]++;
        }

        if (counts.Count == 0)
        {
            result.pattern = CardPattern.None;
            result.activePattern = null;
            return result;
        }

        // Rainbow：必须正好 4 张普通牌，且 4 张都不同
        if (normalCards.Count == 4 && counts.Count == 4)
        {
            result.pattern = CardPattern.Rainbow;
            result.activePattern = normalCards[0].Card.pattern; // Rainbow 实际不依赖 shape，给个占位即可
            return result;
        }

        // 找出现次数最多的 shape
        // 若次数相同，取最早出现的那个，保证结果稳定
        BasicPattern bestShape = null;
        int bestCount = 0;
        int bestFirstIndex = int.MaxValue;

        foreach (var kv in counts)
        {
            var shape = kv.Key;
            int count = kv.Value;
            int firstIndex = firstIndexMap[shape];

            if (count > bestCount || (count == bestCount && firstIndex < bestFirstIndex))
            {
                bestShape = shape;
                bestCount = count;
                bestFirstIndex = firstIndex;
            }
        }

        result.activePattern = bestShape;

        result.pattern = bestCount switch
        {
            >= 4 => CardPattern.Quad,
            3 => CardPattern.Triple,
            2 => CardPattern.Pair,
            _ => CardPattern.Single
        };

        return result;
    }
}