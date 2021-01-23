using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;

namespace osu.Game.Rulesets.Tau.Tests.Objects
{
    [TestFixture]
    public class TestSceneSlider : TauSkinnableTestScene
    {
        private int depthIndex;

        public TestSceneSlider()
        {
            AddStep("Miss Single", () => SetContents(() => testSingle()));
            AddStep("Hit Single", () => SetContents(() => testSingle(true)));
            AddUntilStep("Wait for object despawn", () => !Children.Any(h => h is DrawableTauHitObject hitObject && hitObject.AllJudged == false));
        }

        private Drawable testSingle(bool auto = false)
        {
            var slider = new Slider
            {
                StartTime = Time.Current + 1000,
                Nodes = new[]
                {
                    new SliderNode(0, 0),
                    new SliderNode(500, 90),
                    new SliderNode(1000, 180),
                    new SliderNode(1500, 270),
                    new SliderNode(2000, 0),
                    new SliderNode(2500, 90),
                    new SliderNode(3000, 180),
                    new SliderNode(3500, 270),
                    new SliderNode(4000, 0),
                    new SliderNode(4500, 90),
                    new SliderNode(5000, 180),
                    new SliderNode(5500, 270),
                    new SliderNode(6000, 0),
                    new SliderNode(6500, 90),
                    new SliderNode(7000, 180),
                    new SliderNode(7500, 270),
                }
            };

            slider.ApplyDefaults(new ControlPointInfo(), new BeatmapDifficulty());

            return new TestDrawableSlider(slider, auto)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Depth = depthIndex++
            };
        }

        private class TestDrawableSlider : DrawableSlider
        {
            private readonly bool auto;

            public TestDrawableSlider(Slider h, bool auto)
                : base(h)
            {
                this.auto = auto;
            }

            protected override void CheckForResult(bool userTriggered, double timeOffset)
            {
                if (auto && !userTriggered && timeOffset > 0)
                {
                    // force success
                    ApplyResult(r => r.Type = HitResult.Great);
                }
                else
                    base.CheckForResult(userTriggered, timeOffset);
            }
        }
    }
}
