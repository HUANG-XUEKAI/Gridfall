
public class Display_Score : Display_Base<GameEvents.ChangeData>
{
    protected override void Subscribe()
    {
        GameEvents.ScoreChanged += UpdateDisplay;
    }
    
    protected override void Unsubscribe()
    {
        GameEvents.ScoreChanged -= UpdateDisplay;
    }
    
    protected override GameEvents.ChangeData GetInitialValue()
    {
        return new GameEvents.ChangeData();
    }

    protected override void UpdateDisplay(GameEvents.ChangeData data)
    {
        textMesh.text = $"Score: {data.currValue}";
    }
}
