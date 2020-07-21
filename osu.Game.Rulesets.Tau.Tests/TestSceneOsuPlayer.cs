using NUnit.Framework;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Tau.Tests
{
    [TestFixture]
    public class TestSceneOsuPlayer : PlayerTestScene
    {
        protected override Ruleset CreatePlayerRuleset() => new TauRuleset();
    }
}
