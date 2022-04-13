using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Tau.Tests.Objects
{
    [TestFixture]
    public class TestSceneSliders : TauTestScene
    {
        private int depthIndex;

        public TestSceneSliders()
        {
            AddStep("Hit 100", () => AddRange(testMultiple(100, true)));
            AddUntilStep("Wait for object despawn", () => !Children.Any(h => h is DrawableSlider { AllJudged: false }));
        }

        private IEnumerable<Drawable> testMultiple(int count, bool auto = false)
            => Enumerable.Range(0, count).Select(x => testSingle(Time.Current + x * 100, auto));

        private Drawable testSingle(double startTime, bool auto)
        {
            var slider = new Slider
            {
                StartTime = startTime + 1000,
                Nodes = new BindableList<Slider.SliderNode>(new[]
                {
                    new Slider.SliderNode(0, 0),
                    new Slider.SliderNode(500, 90),
                    new Slider.SliderNode(1000, 180),
                    new Slider.SliderNode(1500, 270),
                    new Slider.SliderNode(2000, 0),
                    new Slider.SliderNode(2500, 90),
                    new Slider.SliderNode(3000, 180),
                    new Slider.SliderNode(3500, 270),
                    new Slider.SliderNode(4000, 0),
                    new Slider.SliderNode(4500, 90),
                    new Slider.SliderNode(5000, 180),
                    new Slider.SliderNode(5500, 270),
                    new Slider.SliderNode(6000, 0),
                    new Slider.SliderNode(6500, 90),
                    new Slider.SliderNode(7000, 180),
                    new Slider.SliderNode(7500, 270),
                })
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
