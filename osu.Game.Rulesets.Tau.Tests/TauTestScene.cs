using osu.Framework.Allocation;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Tau.Tests;

public class TauTestScene : OsuTestScene
{
    [Cached]
    private TauCachedProperties cachedProperties { get; set; } = new();

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        cachedProperties.Dispose();
    }

    protected override Ruleset CreateRuleset()
        => new TauRuleset();
}
