using osu.Game.Rulesets.Tau.UI.Cursor;
using osu.Game.Tests.Visual;
using osuTK;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestSceneGameplayCursor : OsuManualInputManagerTestScene
    {
        private readonly TauCursor cursor;

        public TestSceneGameplayCursor()
        {
            InputManager.ShowVisualCursorGuide = false;

            Add(cursor = new TauCursor(CreateBeatmap(new TauRuleset().RulesetInfo).BeatmapInfo.BaseDifficulty));

            AddStep("Reset cursor", () => { InputManager.MoveMouseTo(cursor, new Vector2(0, -50)); });
        }
    }
}
