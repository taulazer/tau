using System.Collections.Generic;
using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Tests.Objects
{
    [Ignore("Will not work under NUnit testing due to it constantly timing out.")]
    public class TestSceneAutoplaySlider : TauModTestScene
    {
        [TestCase(1000, 4)]
        [TestCase(1000, 8)]
        [TestCase(1000, 12)]
        [TestCase(500, 4)]
        [TestCase(500, 8)]
        [TestCase(500, 12)]
        public void TestAutoplay(float duration, int amount)
        {
            var hitObjects = new List<HitObject>();
            var angleDelta = 360f / amount;
            var timeDelta = duration / amount;

            for (int i = 0; i < amount; i++)
            {
                hitObjects.Add(new Slider()
                {
                    StartTime = 1000 + (timeDelta * i + (100 * i)),
                    Angle = angleDelta * i,
                    Path = new PolarSliderPath(new SliderNode[]
                    {
                        new(0, 0),
                        new(timeDelta * 0.5f, angleDelta)
                    })
                });
            }

            CreateModTest(new ModTestData
            {
                Autoplay = true,
                Beatmap = new Beatmap
                {
                    HitObjects = hitObjects
                },
                PassCondition = () => Player.ScoreProcessor.Combo.Value >= amount * 2,
            });
        }

        [TestCase(4, 1)]
        [TestCase(4, 2)]
        [TestCase(4, 4)]
        [TestCase(4, 8)]
        [TestCase(8, 1)]
        [TestCase(8, 2)]
        [TestCase(8, 4)]
        [TestCase(8, 8)]
        [TestCase(12, 1)]
        [TestCase(12, 2)]
        [TestCase(12, 4)]
        [TestCase(12, 8)]
        public void TestAutoplayWithRepeats(int amount, int repeats)
        {
            const float duration = 1000;

            var hitObjects = new List<HitObject>();
            var timeDelta = duration / amount;
            var angleDelta = 360f / amount;

            for (int i = 0; i < amount; i++)
            {
                hitObjects.Add(new Slider()
                {
                    StartTime = 1000 + (timeDelta * i + (100 * i)),
                    Angle = angleDelta * i,
                    Path = new PolarSliderPath(new SliderNode[]
                    {
                        new(0, 0),
                        new(timeDelta * 0.5f, angleDelta)
                    }),
                    // TODO: Sliders should automatically repeat. (?)
                    RepeatCount = repeats
                });
            }

            CreateModTest(new ModTestData
            {
                Autoplay = true,
                Beatmap = new Beatmap
                {
                    HitObjects = hitObjects
                },
                PassCondition = () => Player.ScoreProcessor.Combo.Value >= amount * 2 + repeats,
            });
        }
    }
}
