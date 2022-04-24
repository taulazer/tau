using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Testing;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.Tau.UI.Effects;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestSceneVisualizer : TestScene
    {
        public TestSceneVisualizer()
        {
            PlayfieldVisualizer visualizer;
            Add(new TauPlayfieldAdjustmentContainer
            {
                Child = visualizer = new PlayfieldVisualizer
                {
                    AccentColour = Color4.White.Opacity(0.25f)
                }
            });

            AddStep("Show visualizer", () => visualizer.FadeIn());

            AddStep("Beat hit", () => visualizer.OnNewResult(new DrawableBeat(new Beat())));
            AddStep("Hard beat hit", () => visualizer.OnNewResult(new DrawableHardBeat(new HardBeat())));
            AddStep("Slider hit", () => visualizer.UpdateSliderPosition(0f));
        }
    }
}
