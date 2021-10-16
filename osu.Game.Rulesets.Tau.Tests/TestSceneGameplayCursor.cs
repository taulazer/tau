using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Tau.UI.Cursor;
using osu.Game.Tests.Visual;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestSceneGameplayCursor : OsuManualInputManagerTestScene
    {
        private readonly TauCursor cursor;

        public TestSceneGameplayCursor()
        {
            InputManager.ShowVisualCursorGuide = false;

            Add(new Circle
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4.Red.Opacity(0.25f),
                FillMode = FillMode.Fit,
                FillAspectRatio = 1,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            });

            Add(cursor = new TauCursor(CreateBeatmap(new TauRuleset().RulesetInfo).BeatmapInfo.BaseDifficulty));

            AddStep("Reset cursor", () => { InputManager.MoveMouseTo(cursor, new Vector2(0, -50)); });
            AddStep("Hide paddle", () => { cursor.Hide(); });
            AddStep("Show paddle", () => { cursor.Show(); });
        }
    }
}
