using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.Tau.Difficulty;

public class TauPerformanceCalculator : PerformanceCalculator
{
    private double accuracy;
    private int scoreMaxCombo;
    private int countGreat;
    private int countOk;
    private int countMeh;
    private int countMiss;

    public TauPerformanceCalculator()
        : base(new TauRuleset())
    {
    }

    protected override PerformanceAttributes CreatePerformanceAttributes(ScoreInfo score, DifficultyAttributes attributes)
    {
        var tauAttributes = (TauDifficultyAttributes)attributes;

        accuracy = score.Accuracy;
        scoreMaxCombo = score.MaxCombo;
        countGreat = score.Statistics.GetValueOrDefault(HitResult.Great);
        countOk = score.Statistics.GetValueOrDefault(HitResult.Ok);
        countMeh = score.Statistics.GetValueOrDefault(HitResult.Meh);
        countMiss = score.Statistics.GetValueOrDefault(HitResult.Miss);

        // Mod multipliers here, let's just set to default osu! value.
        double multiplier = 1.12;

        double aimValue = computeAimValue(score, tauAttributes);
        double accuracyValue = computeAccuracyValue(score, tauAttributes);

        double totalValue = Math.Pow(
            Math.Pow(aimValue, 1.1) +
            Math.Pow(accuracyValue, 1.1),
            1.0 / 1.1
        ) * multiplier;

        return new TauPerformanceAttribute()
        {
            Aim = aimValue,
            Accuracy = accuracyValue,
            Total = totalValue
        };
    }

    private double computeAimValue(ScoreInfo score, TauDifficultyAttributes attributes)
    {
        double rawAim = attributes.AimDifficulty;
        double aimValue = Math.Pow(5.0 * Math.Max(1.0, rawAim / 0.0675) - 4.0, 3.0) / 100000.0; // TODO: Figure values here.

        aimValue *= accuracy;

        return aimValue;
    }

    private double computeAccuracyValue(ScoreInfo score, TauDifficultyAttributes attributes)
    {
        if (score.Mods.Any(mod => mod is TauModRelax))
            return 0.0;

        // This percentage only considers HitCircles of any value - in this part of the calculation we focus on hitting the timing hit window.
        double betterAccuracyPercentage;
        int amountHitObjectsWithAccuracy = attributes.HitCircleCount;

        if (amountHitObjectsWithAccuracy > 0)
            betterAccuracyPercentage = ((countGreat - (totalHits - amountHitObjectsWithAccuracy)) * 6 + countOk * 2 + countMeh) / (double)(amountHitObjectsWithAccuracy * 6);
        else
            betterAccuracyPercentage = 0;

        // It is possible to reach a negative accuracy with this formula. Cap it at zero - zero points.
        if (betterAccuracyPercentage < 0)
            betterAccuracyPercentage = 0;

        // Lots of arbitrary values from testing.
        // Considering to use derivation from perfect accuracy in a probabilistic manner - assume normal distribution.
        double accuracyValue = Math.Pow(1.52163, attributes.OverallDifficulty) * Math.Pow(betterAccuracyPercentage, 24) * 2.83;

        // Bonus for many hitcircles - it's harder to keep good accuracy up for longer.
        accuracyValue *= Math.Min(1.15, Math.Pow(amountHitObjectsWithAccuracy / 1000.0, 0.3));
        return accuracyValue;
    }

    private int totalHits => countGreat + countOk + countMeh + countMiss;
}
