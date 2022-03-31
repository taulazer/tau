using System.Collections.Generic;
using NUnit.Framework;
using osu.Framework.Bindables;
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
                            Nodes = new BindableList<Slider.SliderNode>
                            {
                                new(0, 0),
                                new(50, -10),
                                new(150, 10),
                            },
                            RepeatCount = 1
                        },
                        new HardBeat
                        {
                            StartTime = 1500
                        }
                    }
                },
                PassCondition = () => Player.ScoreProcessor.JudgedHits >= 5,
            });
        }
    }
}
