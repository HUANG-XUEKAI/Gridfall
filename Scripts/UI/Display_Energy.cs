public class Display_Energy : Display_Base<GameEvents.ChangeData>
{
    protected override void Subscribe()
    {
        GameEvents.EnergyChanged += UpdateDisplay;
    }
    
    protected override void Unsubscribe()
    {
        GameEvents.EnergyChanged -= UpdateDisplay;
    }
    
    protected override GameEvents.ChangeData GetInitialValue()
    {
        return new GameEvents.ChangeData
        {
           currValue = AccountDataCenter.Instance.Profile.energy,
        };
    }

    protected override void UpdateDisplay(GameEvents.ChangeData data)
    {
        textMesh.text = $"E: {data.currValue}";
    }
}