using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Tau.Tests
{
    public abstract class TauModTestScene : ModTestScene
    {
        protected override Ruleset CreatePlayerRuleset() => new TauRuleset();
    }
}
