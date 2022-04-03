using System.Collections.Generic;
using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Tests.Mods
{
    public class TestSceneHardRock : TauModTestScene
    {
        [Test]
        public void TestHardRock()
        {
            CreateModTest(new ModTestData()
            {
                Mod = new TauModHardRock(),
                Beatmap = new Beatmap
                {
                    HitObjects = new List<HitObject>
                    {
                        new Beat
                        {
                            StartTime = 500,
                            Angle = 0
                        },
                        new Beat
                        {
                            StartTime = 1000,
                            Angle = 90
                        },
                        new Beat
                        {
                            StartTime = 1500,
                            Angle = 180
                        },
                        new Beat
                        {
                            StartTime = 2000,
                            Angle = 270
                        }
                    }
                },
                PassCondition = () =>
                {
                    // ReSharper disable once ReplaceWithSingleAssignment.True
                    var result = true;

                    if (getBeat(0).Angle != 180) result = false;
                    if (getBeat(1).Angle != 270) result = false;
                    if (getBeat(2).Angle != 0) result = false;
                    if (getBeat(3).Angle != 90) result = false;

                    return result;
                }
            });
        }

        private Beat getBeat(int index)
            => Beatmap.Value.Beatmap.HitObjects[index] as Beat;
    }
}
