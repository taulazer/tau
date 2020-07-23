using NUnit.Framework;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Tau.Tests
{
    [TestFixture]
    public class TestSceneTauEditor : EditorTestScene
    {
        protected override Ruleset CreateEditorRuleset() => new TauRuleset();
    }
}
