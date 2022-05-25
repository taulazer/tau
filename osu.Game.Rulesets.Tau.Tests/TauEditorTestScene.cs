using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Tau.Tests;

public class TauEditorTestScene : EditorTestScene
{
    protected override Ruleset CreateEditorRuleset() => new TauRuleset();
}
