using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.Difficulty;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Tests.Beatmaps;

namespace osu.Game.Rulesets.Tau.Tests.NonVisual;

public class AimCalculationTests : CalculationTestScene
{
    public override WorkingBeatmap CreateBeatmap()
    {
        // TODO: Fetch from resource instead?
        var beatmap = new Beatmap
        {
            HitObjects = new List<HitObject>
            {
                createBeatObject(0, 0),
                createBeatObject(1000, 0),
                createBeatObject(1500, 90),
                createBeatObject(1750, 180),
            }
        };

        return new TestWorkingBeatmap(beatmap);
    }

    public override void AssertDifficulty(IEnumerable<Mod> mods, TauDifficultyAttributes attributes)
    {
        var modsArray = mods.ToArray();

        if (modsArray.Length == 0)
        {
            Assert.AreEqual(attributes.StarRating, 0.54396336135843126);
            return;
        }

        if (ContainMods(modsArray, new Mod[] { new TauModDoubleTime(), new TauModHardRock() }))
        {
            Assert.AreEqual(attributes.StarRating, 0.70501975613138956);
            return;
        }
    }

    public override void AssertPerformance(IEnumerable<Mod> mods, TauDifficultyAttributes attributes)
    {
    }

    private Beat createBeatObject(double time, float angle)
    {
        return new Beat
        {
            Angle = angle,
            StartTime = time
        };
    }
}
