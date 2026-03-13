
public class Display_HP : Display_Base<GameEvents.ChangeData>
{
    protected override void Subscribe()
    {
        GameEvents.HPChanged += UpdateDisplay;
    }
    
    protected override void Unsubscribe()
    {
        GameEvents.HPChanged -= UpdateDisplay;
    }
    
    protected override GameEvents.ChangeData GetInitialValue()
    {
        return new GameEvents.ChangeData
        {
            currValue = MatchData.DefaultHP
        };
    }

    protected override void UpdateDisplay(GameEvents.ChangeData data)
    {
        textMesh.text = $"HP: {data.currValue}";
    }
}
