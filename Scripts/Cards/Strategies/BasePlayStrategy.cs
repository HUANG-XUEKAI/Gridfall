public abstract class BasePlayStrategy
{
    public abstract PlayExecutionResult Execute(
        BoardManager board,
        BasicPattern pattern,
        int x,
        int y,
        bool isRowMode
    );
}