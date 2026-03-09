public readonly struct PlayExecutionResult
{
    public readonly bool success;
    public readonly int clearedCellCount;
    public readonly int clearedBaseScore;

    public PlayExecutionResult(bool success, int clearedCellCount, int clearedBaseScore)
    {
        this.success = success;
        this.clearedCellCount = clearedCellCount;
        this.clearedBaseScore = clearedBaseScore;
    }

    public static PlayExecutionResult Fail()
    { 
        return new PlayExecutionResult(false, 0, 0);
    }
    
    public static PlayExecutionResult Success(int clearedCellCount, int clearedBaseScore)
    {
        return new PlayExecutionResult(true, clearedCellCount, clearedBaseScore);
    }
}