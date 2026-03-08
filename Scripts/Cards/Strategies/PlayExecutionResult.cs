public readonly struct PlayExecutionResult
{
    public readonly bool success;
    public readonly int clearedCellCount;

    public PlayExecutionResult(bool success, int cleared)
    {
        this.success = success;
        this.clearedCellCount = cleared;
    }

    public static PlayExecutionResult Fail()
        => new PlayExecutionResult(false, 0);

    public static PlayExecutionResult Success(int cleared)
        => new PlayExecutionResult(true, cleared);
}