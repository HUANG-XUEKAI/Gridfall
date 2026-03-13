
public class Display_Diamond : Display_Base<GameEvents.ChangeData>
{
    protected override void Subscribe()
    {
        GameEvents.DiamondChanged += UpdateDisplay;
    }
    
    protected override void Unsubscribe()
    {
        GameEvents.DiamondChanged -= UpdateDisplay;
    }
    
    protected override GameEvents.ChangeData GetInitialValue()
    {
        return new GameEvents.ChangeData
        {
            currValue = AccountDataCenter.Instance.Profile.diamond,
        };
    }

    protected override void UpdateDisplay(GameEvents.ChangeData data)
    {
        textMesh.text = $"D: {data.currValue}";
    }
}
