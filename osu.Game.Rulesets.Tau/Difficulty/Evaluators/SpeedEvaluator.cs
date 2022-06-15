using System;
using System.Linq;
using osu.Framework.Utils;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Tau.Difficulty.Preprocessing;
using osu.Game.Rulesets.Tau.Mods;

namespace osu.Game.Rulesets.Tau.Difficulty.Evaluators;

public static class SpeedEvaluator
{
    private const double single_spacing_threshold = 125;
    private const double min_speed_bonus = 75; // ~200BPM
    private const double speed_balancing_factor = 40;

    public static double EvaluateDifficulty(DifficultyHitObject current, double greatWindow)
    {
        // derive strainTime for calculation
        var tauCurrObj = (TauDifficultyHitObject)current;
        var tauPrevObj = current.Index > 0 ? (TauDifficultyHitObject)current.Previous(0) : null;

        double strainTime = tauCurrObj.StrainTime;
        double greatWindowFull = greatWindow * 2;
        double speedWindowRatio = strainTime / greatWindowFull;

        // Aim to nerf cheesy rhythms (Very fast consecutive doubles with large deltatimes between)
        if (tauPrevObj != null && strainTime < greatWindowFull && tauPrevObj.StrainTime > strainTime)
            strainTime = Interpolation.Lerp(tauPrevObj.StrainTime, strainTime, speedWindowRatio);

        // Cap deltatime to the OD 300 hitwindow.
        // 0.93 is derived from making sure 260bpm OD8 streams aren't nerfed harshly, whilst 0.92 limits the effect of the cap.
        strainTime /= Math.Clamp((strainTime / greatWindowFull) / 0.93, 0.92, 1);

        // derive speedBonus for calculation
        double speedBonus = 1.0;

        if (strainTime < min_speed_bonus)
            speedBonus = 1 + 0.75 * Math.Pow((min_speed_bonus - strainTime) / speed_balancing_factor, 2);

        double distance = single_spacing_threshold;

        if (current is TauAngledDifficultyHitObject currAngled && (current.Index > 0 && current.Previous(0) is TauAngledDifficultyHitObject lastAngled))
        {
            double travelDistance = Math.Abs(currAngled.Distance);
            distance = Math.Min(single_spacing_threshold,
                travelDistance + Math.Abs(lastAngled.Distance)); // tauCurrobj.Disance used to be MinJumpDistance, replace if found alternate.
        }

        return (speedBonus + speedBonus * Math.Pow(distance / single_spacing_threshold, 3.5)) / strainTime;
    }

    public static double EvaluatePerformance(TauPerformanceContext context)
    {
        TauDifficultyAttributes attributes = context.DifficultyAttributes;
        double speedValue = Math.Pow(5.0 * Math.Max(1.0, attributes.SpeedDifficulty / 0.0675) - 4.0, 3.0) / 100000.0;

        // Length bonus is added on for beatmaps with more than 2,000 hitobjects.
        double lengthBonus =
            0.95 + 0.4 * Math.Min(1.0, context.TotalHits / 2000.0) + (context.TotalHits > 2000 ? Math.Log10(context.TotalHits / 2000.0) * 0.5 : 0.0);
        speedValue *= lengthBonus;

        // Penalize misses by assessing # of misses relative to the total # of objects. Default a 3% reduction for any # of misses.
        if (context.EffectiveMissCount > 0)
            speedValue *= 0.97 * Math.Pow(1 - Math.Pow(context.EffectiveMissCount / context.TotalHits, 0.775), Math.Pow(context.EffectiveMissCount, .875));

        speedValue *= getComboScalingFactor(context);

        double approachRateFactor = 0.0;
        if (attributes.ApproachRate > 10.33)
            approachRateFactor = 0.3 * (attributes.ApproachRate - 10.33);

        speedValue *= 1.0 + approachRateFactor * lengthBonus; // Buff for longer maps with high AR.

        if (context.Score.Mods.Any(m => m is TauModFadeIn))
        {
            // We want to give more reward for lower AR when it comes to aim and Fade In. This nerfs high AR and buffs lower AR.
            speedValue *= 1.0 + 0.04 * (12.0 - attributes.ApproachRate);
        }

        // Scale the speed value with accuracy and OD.
        speedValue *= (0.95 + Math.Pow(attributes.OverallDifficulty, 2) / 750) * Math.Pow(context.Accuracy, (14.5 - Math.Max(attributes.OverallDifficulty, 8)) / 2);

        return speedValue;
    }

    private static double getComboScalingFactor(TauPerformanceContext context) => context.DifficultyAttributes.MaxCombo <= 0
                                                                                      ? 1.0
                                                                                      : Math.Min(Math.Pow(context.ScoreMaxCombo, 0.8) / Math.Pow(context.DifficultyAttributes.MaxCombo, 0.8), 1.0);
}
