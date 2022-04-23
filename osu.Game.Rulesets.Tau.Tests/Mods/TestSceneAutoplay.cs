using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Tests.Mods
{
    public class TestSceneAutoplay : TauModTestScene
    {
        [Test]
        public void TestAutoplay()
        {
            CreateModTest(new ModTestData
            {
                Autoplay = true,
                Beatmap = new Beatmap
                {
                    HitObjects = new List<HitObject>
                    {
                        new Beat
                        {
                            StartTime = 500,
                            Angle = 0
                        },
                        new Slider
                        {
                            StartTime = 750,
                            Path = new PolarSliderPath(new SliderNode[]
                            {
                                new(0, 0),
                                new(50, -10),
                                new(150, 10),
                            }),
                            RepeatCount = 1
                        },
                        new HardBeat
                        {
                            StartTime = 1500
                        }
                    }
                },
                PassCondition = () => Player.Results.Count(r => r.IsHit) >= 5,
            });
        }
    }
}
