using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Tests.Visual;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Tests.Objects
{
    [TestFixture]
    public class TestSceneSlider : OsuTestScene
    {
        private readonly Container content;
        protected override Container<Drawable> Content => content;

        private int depthIndex;

        public TestSceneSlider()
        {
            base.Content.Add(content = new TauInputManager(new RulesetInfo { ID = 0 })
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(768),
                RelativeSizeAxes = Axes.None,
                Scale = new Vector2(.6f)
            });

            Add(new Circle
            {
                Colour = Color4.Red,
                Size = new Vector2(25),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            });

            AddStep("Miss Single", () => testSingle());
            AddStep("Hit Single", () => testSingle(true));
            AddUntilStep("Wait for object despawn", () => !Children.Any(h => h is DrawableTauHitObject hitObject && hitObject.AllJudged == false));
        }

        private void testSingle(bool auto = false)
        {
            var slider = new Slider
            {
                StartTime = Time.Current + 1000,
                Nodes = new[]
                {
                    new SliderNode(500, 25),
                    new SliderNode(1000, 270),
                    new SliderNode(1500, 180),
                }
            };

            slider.ApplyDefaults(new ControlPointInfo(), new BeatmapDifficulty());

            Add(new TestDrawableSlider(slider, auto)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Depth = depthIndex++
            });
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
