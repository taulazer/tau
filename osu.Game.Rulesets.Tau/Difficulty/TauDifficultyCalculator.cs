using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Difficulty.Preprocessing;
using osu.Game.Rulesets.Tau.Difficulty.Skills;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Scoring;
using osu.Game.Rulesets.Tau.UI;

namespace osu.Game.Rulesets.Tau.Difficulty;

public class TauDifficultyCalculator : DifficultyCalculator
{
    private readonly TauCachedProperties properties = new();
    private double hitWindowGreat;

    private const double difficulty_multiplier = 0.0825;

    public TauDifficultyCalculator(IRulesetInfo ruleset, IWorkingBeatmap beatmap)
        : base(ruleset, beatmap)
    {
    }

    protected override DifficultyAttributes CreateDifficultyAttributes(IBeatmap beatmap, Mod[] mods, Skill[] skills, double clockRate)
    {
        if (beatmap.HitObjects.Count == 0)
            return new TauDifficultyAttributes { Mods = mods };

        double aim = Math.Sqrt(skills[0].DifficultyValue()) * difficulty_multiplier;
        double aimNoSliders = Math.Sqrt(skills[1].DifficultyValue()) * difficulty_multiplier;
        double speed = Math.Sqrt(skills[2].DifficultyValue()) * difficulty_multiplier;
        double complexity = Math.Sqrt(skills[3].DifficultyValue()) * difficulty_multiplier;

        if (mods.Any(m => m is TauModRelax))
        {
            speed = 0.0;
            complexity = 0.0;
        }

        double preempt = IBeatmapDifficultyInfo.DifficultyRange(beatmap.Difficulty.ApproachRate, 1800, 1200, 450) / clockRate;

        double baseAim = Math.Pow(5 * Math.Max(1, aim / 0.0675) - 4, 3) / 100000;
        double baseSpeed = Math.Pow(5 * Math.Max(1, speed / 0.0675) - 4, 3) / 100000;
        double baseComplexity = Math.Pow(5 * Math.Max(1, complexity / 0.0675) - 4, 3) / 100000;

        double basePerformance =
            Math.Pow(
                Math.Pow(baseAim, 1.1) + Math.Pow(baseSpeed, 1.1) + Math.Pow(baseComplexity, 1.1),
                1.0 / 1.1
            );

        double starRating = basePerformance > 0.00001 ? Math.Cbrt(1.12) * 0.027 * (Math.Cbrt(100000 / Math.Pow(2, 1 / 1.1) * basePerformance) + 4) : 0;

        return new TauDifficultyAttributes
        {
            AimDifficulty = aim,
            SpeedDifficulty = speed,
            ComplexityDifficulty = complexity,
            StarRating = starRating,
            Mods = mods,
            MaxCombo = beatmap.GetMaxCombo(),
            OverallDifficulty = beatmap.Difficulty.OverallDifficulty,
            ApproachRate = preempt > 1200 ? (1800 - preempt) / 120 : (1200 - preempt) / 150 + 5,
            NotesCount = beatmap.HitObjects.Count(h => h is Beat),
            SliderCount = beatmap.HitObjects.Count(s => s is Slider),
            HardBeatCount = beatmap.HitObjects.Count(hb => hb is HardBeat),
            SliderFactor = aim > 0 ? aimNoSliders / aim : 1
        };
    }

    protected override IEnumerable<DifficultyHitObject> CreateDifficultyHitObjects(IBeatmap beatmap, double clockRate)
    {
        properties.SetRange(beatmap.Difficulty.CircleSize);

        TauHitObject lastObject = null;
        TauAngledDifficultyHitObject lastAngled = null;
        var objects = new List<DifficultyHitObject>();

        foreach (var hitObject in beatmap.HitObjects.Cast<TauHitObject>())
        {
            if (lastObject != null)
            {
                if (hitObject is AngledTauHitObject)
                {
                    var obj = new TauAngledDifficultyHitObject(hitObject, lastObject, clockRate, objects, objects.Count, properties, lastAngled);
                    objects.Add(obj);

                    lastAngled = obj;
                }
                else
                    objects.Add(new TauDifficultyHitObject(hitObject, lastObject, clockRate, objects, objects.Count));
            }

            lastObject = hitObject;
        }

        return objects;
    }

    protected override Skill[] CreateSkills(IBeatmap beatmap, Mod[] mods, double clockRate)
    {
        HitWindows hitWindows = new TauHitWindow();
        hitWindows.SetDifficulty(beatmap.Difficulty.OverallDifficulty);

        hitWindowGreat = hitWindows.WindowFor(HitResult.Great) / clockRate;
        return new Skill[]
        {
            new Aim(mods, new[] { typeof(Beat), typeof(SliderRepeat), typeof(Slider) }),
            new Aim(mods, new[] { typeof(Beat) }),
            new Speed(mods, hitWindowGreat),
            new Complexity(mods)
        };
    }
}
