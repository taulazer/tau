using osu.Game.Rulesets.Tau.UI;
using osu.Game.Tests.Visual;
using osuTK;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestSceneTauCursor : OsuManualInputManagerTestScene
    {
        private readonly TauCursor cursor;

        public TestSceneTauCursor()
        {
            Add(cursor = new TauCursor());

            AddStep("Reset cursor", () => { InputManager.MoveMouseTo(cursor, new Vector2(0, -50)); });
            AddStep("Hide paddle", () => { cursor.Hide(); });
            AddStep("Show paddle", () => { cursor.Show(); });
        }
    }
}
