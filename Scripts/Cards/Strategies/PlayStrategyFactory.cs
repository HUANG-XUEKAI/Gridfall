public static class PlayStrategyFactory
{
    private static readonly BasePlayStrategy single = new SinglePlayStrategy();
    private static readonly BasePlayStrategy pair = new PairPlayStrategy();
    private static readonly BasePlayStrategy triple = new TriplePlayStrategy();
    private static readonly BasePlayStrategy quad = new QuadPlayStrategy();
    private static readonly BasePlayStrategy rainbow = new RainbowPlayStrategy();

    public static BasePlayStrategy GetStrategy(CardPattern pattern)
    {
        return pattern switch
        {
            CardPattern.Single => single,
            CardPattern.Pair => pair,
            CardPattern.Triple => triple,
            CardPattern.Quad => quad,
            CardPattern.Rainbow => rainbow,
            _ => null
        };
    }
}