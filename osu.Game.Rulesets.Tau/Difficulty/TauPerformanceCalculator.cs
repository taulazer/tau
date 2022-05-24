using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Difficulty.Skills;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.Tau.Difficulty;

public class TauPerformanceCalculator : PerformanceCalculator
{
    private TauPerformanceContext context;

    public TauPerformanceCalculator()
        : base(new TauRuleset())
    {
    }

    protected override PerformanceAttributes CreatePerformanceAttributes(ScoreInfo score, DifficultyAttributes attributes)
    {
        var tauAttributes = (TauDifficultyAttributes)attributes;
        context = new TauPerformanceContext(score, tauAttributes);

        // Mod multipliers here, let's just set to default osu! value.
        double multiplier = 1.12;

        double aimValue = Aim.ComputePerformance(context);
        double speedValue = Speed.ComputePerformance(context);
        double accuracyValue = computeAccuracy(context);
        double effectiveMissCount = calculateEffectiveMissCount(context);

        if (score.Mods.Any(m => m is TauModNoFail))
            multiplier *= Math.Max(0.90, 1.0 - 0.02 * effectiveMissCount);

        double totalValue = Math.Pow(
                                Math.Pow(aimValue, 1.1) + Math.Pow(accuracyValue, 1.1) + Math.Pow(speedValue, 1.1),
                                1.0 / 1.1
                            ) *
                            multiplier;

        return new TauPerformanceAttribute
        {
            Aim = aimValue,
            Speed = speedValue,
            Accuracy = accuracyValue,
            Total = totalValue,
            EffectiveMissCount = effectiveMissCount
        };
    }

    private double computeAccuracy(TauPerformanceContext context)
    {
        if (context.Score.Mods.Any(mod => mod is TauModRelax))
            return 0.0;

        // This percentage only considers beats of any value - in this part of the calculation we focus on hitting the timing hit window.
        double betterAccuracyPercentage;
        int amountHitObjectsWithAccuracy = context.DifficultyAttributes.NotesCount;

        if (amountHitObjectsWithAccuracy > 0)
            betterAccuracyPercentage = ((context.CountGreat - (context.TotalHits - amountHitObjectsWithAccuracy)) * 6 + context.CountOk * 2) /
                                       (double)(amountHitObjectsWithAccuracy * 6);
        else
            betterAccuracyPercentage = 0;

        // It is possible to reach a negative accuracy with this formula. Cap it at zero - zero points.
        if (betterAccuracyPercentage < 0)
            betterAccuracyPercentage = 0;

        // Lots of arbitrary values from testing.
        // Considering to use derivation from perfect accuracy in a probabilistic manner - assume normal distribution.
        double accuracyValue = Math.Pow(1.52163, context.DifficultyAttributes.OverallDifficulty) * Math.Pow(betterAccuracyPercentage, 24) * 2.83;

        // Bonus for many hitcircles - it's harder to keep good accuracy up for longer.
        accuracyValue *= Math.Min(1.15, Math.Pow(amountHitObjectsWithAccuracy / 1000.0, 0.3));
        return accuracyValue;
    }

    public double calculateEffectiveMissCount(TauPerformanceContext context)
    {
        // Guess the number of misses + slider breaks from combo
        double comboBasedMissCount = 0.0;

        if (context.DifficultyAttributes.SliderCount > 0)
        {
            double fullComboThreshold = context.DifficultyAttributes.MaxCombo - 0.1 * context.DifficultyAttributes.SliderCount;
            if (context.ScoreMaxCombo < fullComboThreshold)
                comboBasedMissCount = fullComboThreshold / Math.Max(1.0, context.ScoreMaxCombo);
        }

        // Clamp miss count since it's derived from combo and can be higher than total hits and that breaks some calculations
        comboBasedMissCount = Math.Min(comboBasedMissCount, context.TotalHits);

        return Math.Max(context.CountMiss, comboBasedMissCount);
    }
}

public struct TauPerformanceContext
{
    public double Accuracy => Score.Accuracy;
    public int ScoreMaxCombo => Score.MaxCombo;
    public int CountGreat => Score.Statistics.GetValueOrDefault(HitResult.Great);
    public int CountOk => Score.Statistics.GetValueOrDefault(HitResult.Ok);
    public int CountMiss => Score.Statistics.GetValueOrDefault(HitResult.Miss);

    public double EffectiveMissCount => 0.0;

    public int TotalHits => CountGreat + CountOk + CountMiss;

    public ScoreInfo Score { get; set; }
    public TauDifficultyAttributes DifficultyAttributes { get; set; }

    public TauPerformanceContext(ScoreInfo score, TauDifficultyAttributes attributes)
    {
        Score = score;
        DifficultyAttributes = attributes;
    }
}
