namespace FOOD.Core.Syntax.Statements;

public readonly struct SwitchRegion
{
    public readonly SwitchRegion[] Regions;
    public readonly SwitchCase[] Cases;
    public Statement[] CommonStart { get; init; }
    public Statement[] CommonEnd { get; init; }

    public SwitchRegion(SwitchRegion[] regions, SwitchCase[] cases, Statement[] commonStart, Statement[] commonEnd)
    {
        Regions = regions;
        Cases = cases;
        CommonStart = commonStart;
        CommonEnd = commonEnd;
    }

    public IEnumerable<Statement> Statements => Cases.SelectMany(x => x.Statements);
}
