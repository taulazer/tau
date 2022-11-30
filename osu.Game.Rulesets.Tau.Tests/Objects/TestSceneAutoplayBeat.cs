using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Tests.Objects
{
    public partial class TestSceneAutoplayBeat : TauModTestScene
    {
        [TestCase(2000, 4)]
        [TestCase(2000, 8)]
        [TestCase(2000, 12)]
        [TestCase(2000, 24)]
        [TestCase(1000, 4)]
        [TestCase(1000, 8)]
        [TestCase(1000, 12)]
        [TestCase(1000, 24)]
        [TestCase(500, 4)]
        [TestCase(500, 8)]
        [TestCase(500, 12)]
        [TestCase(500, 24)]
        public void TestAutoplay(float duration, int amount)
        {
            var hitObjects = new List<HitObject>();
            var angleDelta = 360f / amount;
            var timeDelta = duration / amount;

            for (int i = 0; i < amount; i++)
            {
                hitObjects.Add(new Beat
                {
                    StartTime = 1000 + (timeDelta * i),
                    Angle = angleDelta * i
                });
            }

            CreateModTest(new ModTestData
            {
                Autoplay = false,
                Mod = new TauModAutoplay(),
                Beatmap = new Beatmap
                {
                    HitObjects = hitObjects
                },
                PassCondition = () => Player.Results.Count(r => r.IsHit) >= amount,
            });
        }
    }
}
