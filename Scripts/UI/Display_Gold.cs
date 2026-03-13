
public class Display_Gold : Display_Base<GameEvents.ChangeData>
{
    protected override void Subscribe()
    {
        GameEvents.GoldChanged += UpdateDisplay;
    }
    
    protected override void Unsubscribe()
    {
        GameEvents.GoldChanged -= UpdateDisplay;
    }
    
    protected override GameEvents.ChangeData GetInitialValue()
    {
        return new GameEvents.ChangeData
        {
            currValue = AccountDataCenter.Instance.Profile.gold,
        };
    }

    protected override void UpdateDisplay(GameEvents.ChangeData data)
    {
        textMesh.text = $"G: {data.currValue}";
    }
}
