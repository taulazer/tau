using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Difficulty;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Tau.Tests.NonVisual;

public abstract class CalculationTestScene : OsuTestScene
{
    public abstract WorkingBeatmap CreateBeatmap();

    public abstract void AssertDifficulty(IEnumerable<Mod> mods, TauDifficultyAttributes attributes);

    public abstract void AssertPerformance(IEnumerable<Mod> mods, TauDifficultyAttributes attributes);

    private WorkingBeatmap beatmap;
    private TauDifficultyCalculator diffCalculator;

    [SetUp]
    public void Setup()
    {
        beatmap = CreateBeatmap();
        diffCalculator = new TauDifficultyCalculator(new TauRuleset().RulesetInfo, beatmap);
    }

    [Test]
    public void TestDifficulty()
    {
        assertDifficulty();
        assertDifficulty(new Mod[] { new TauModDoubleTime(), new TauModHardRock() });
    }

    [Test]
    public void TestPerformance()
    {
        assertPerformance();
        assertPerformance(new Mod[] { new TauModDoubleTime(), new TauModHardRock() });
    }

    private void assertDifficulty(IEnumerable<Mod> mods = null)
    {
        mods ??= Array.Empty<Mod>();
        AssertDifficulty(mods, diffCalculator.Calculate(mods) as TauDifficultyAttributes);
    }

    private void assertPerformance(IEnumerable<Mod> mods = null)
    {
        mods ??= Array.Empty<Mod>();
        AssertPerformance(mods, diffCalculator.Calculate(mods) as TauDifficultyAttributes);
    }

    protected static bool ContainMods(IReadOnlyList<Mod> mods, IEnumerable<Mod> toCheck)
        => toCheck.All(mod => mods.Any(m => m.GetType() == mod.GetType()));
}
