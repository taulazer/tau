using NUnit.Framework;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Tau.Tests
{
    [TestFixture]
    public class TestSceneOsuPlayerAutoplay : PlayerTestScene
    {
        protected override bool Autoplay => true;
        protected override Ruleset CreatePlayerRuleset() => new TauRuleset();
    }
}
