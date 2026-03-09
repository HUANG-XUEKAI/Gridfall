using UnityEngine;

public class SinglePlayStrategy : BasePlayStrategy
{
    public override PlayExecutionResult Execute(
        BoardManager board,
        BasicPattern pattern,
        int x,
        int y,
        bool isRowMode)
    {
        if (pattern == null) return PlayExecutionResult.Fail();
        
        var targetBlock = board.Get(x, y);
        if (targetBlock == null) 
            return PlayExecutionResult.Fail();
        if (targetBlock.pattern != pattern) 
            return PlayExecutionResult.Fail();

        int clearedBaseScore = targetBlock.baseScore;
        board.Clear(x, y);
        
        return PlayExecutionResult.Success(1, clearedBaseScore);
    }
}

public class PairPlayStrategy : BasePlayStrategy
{
    public override PlayExecutionResult Execute(
        BoardManager board,
        BasicPattern pattern,
        int x,
        int y,
        bool isRowMode)
    {
        if (pattern == null) return PlayExecutionResult.Fail();

        int cleared = 0;
        int clearedBaseScore = 0;
        BasicBlock targetBlock;

        if (isRowMode)
        {
            for (int xx = 0; xx < board.Width; xx++)
            {
                targetBlock = board.Get(xx, y);
                if (targetBlock != null && targetBlock.pattern == pattern)
                {
                    clearedBaseScore += targetBlock.baseScore;
                    board.Clear(xx, y);
                    cleared++;
                }
            }
        }
        else
        {
            for (int yy = 0; yy < board.Height; yy++)
            {
                targetBlock = board.Get(x, yy);
                if (targetBlock != null && targetBlock.pattern == pattern)
                {
                    clearedBaseScore += targetBlock.baseScore;
                    board.Clear(x, yy);
                    cleared++;
                }
            }
        }

        return cleared > 0
            ? PlayExecutionResult.Success(cleared, clearedBaseScore)
            : PlayExecutionResult.Fail();
    }
}

public class TriplePlayStrategy : BasePlayStrategy
{
    private int GetTripleRangeStart(int index, int maxExclusive)
    {
        if (index <= 0) return 0;
        if (index >= maxExclusive - 1) return maxExclusive - 3;
        return index - 1;
    }

    public override PlayExecutionResult Execute(
        BoardManager board,
        BasicPattern pattern,
        int x,
        int y,
        bool isRowMode)
    {
        if (pattern == null) return PlayExecutionResult.Fail();

        int cleared = 0;
        int clearedBaseScore = 0;
        BasicBlock targetBlock;

        if (isRowMode)
        {
            int startY = GetTripleRangeStart(y, board.Height);

            for (int yy = startY; yy < startY + 3; yy++)
            for (int xx = 0; xx < board.Width; xx++)
            {
                targetBlock = board.Get(xx, yy);
                if (targetBlock != null && targetBlock.pattern == pattern)
                {
                    clearedBaseScore += targetBlock.baseScore;
                    board.Clear(xx, yy);
                    cleared++;
                }
            }
        }
        else
        {
            int startX = GetTripleRangeStart(x, board.Width);

            for (int xx = startX; xx < startX + 3; xx++)
            for (int yy = 0; yy < board.Height; yy++)
            {
                targetBlock = board.Get(xx, yy);
                if (targetBlock != null && targetBlock.pattern == pattern)
                {
                    clearedBaseScore += targetBlock.baseScore;
                    board.Clear(xx, yy);
                    cleared++;
                }
            }
        }

        return cleared > 0
            ? PlayExecutionResult.Success(cleared, clearedBaseScore)
            : PlayExecutionResult.Fail();
    }
}

public class QuadPlayStrategy : BasePlayStrategy
{
    public override PlayExecutionResult Execute(
        BoardManager board,
        BasicPattern pattern,
        int x,
        int y,
        bool isRowMode)
    {
        Vector2Int result = board.ClearAllMatching(pattern);
        int cleared = result.x;
        int clearedBaseScore = result.y;
        return cleared > 0
            ? PlayExecutionResult.Success(cleared, clearedBaseScore)
            : PlayExecutionResult.Fail();
    }
}

public class RainbowPlayStrategy : BasePlayStrategy
{
    public override PlayExecutionResult Execute(
        BoardManager board,
        BasicPattern pattern,
        int x,
        int y,
        bool isRowMode)
    {
        int startX = Mathf.Clamp(x - 3, 0, board.Width - 4);
        int startY = Mathf.Clamp(y - 3, 0, board.Height - 4);

        int clearedBaseScore = 0;
        int cleared = 0;
        BasicBlock targetBlock;

        for (int yy = startY; yy < startY + 4; yy++)
        for (int xx = startX; xx < startX + 4; xx++)
        {
            targetBlock = board.Get(xx, yy);
            if (targetBlock != null)
            {
                clearedBaseScore += targetBlock.baseScore;
                board.Clear(xx, yy);
                cleared++;
            }
        }

        return cleared > 0
            ? PlayExecutionResult.Success(cleared, clearedBaseScore)
            : PlayExecutionResult.Fail();
    }
}