using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Tau.Difficulty.Preprocessing;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Utils;

namespace osu.Game.Rulesets.Tau.Difficulty.Evaluators
{
    public static class ComplexityEvaluator
    {
        /// <summary>
        /// Maximum number of entries to keep in <see cref="mono_history"/>.
        /// </summary>
        private const int mono_history_max_length = 5;

        /// <summary>
        /// Queue with the lengths of the last <see cref="mono_history_max_length"/> most recent mono patterns,
        /// with the most recent value at the end of the queue.
        /// </summary>
        private static readonly LimitedCapacityQueue<int> mono_history = new(mono_history_max_length);

        /// <summary>
        /// The <see cref="HitType"/> of the last object hit before the one being considered.
        /// </summary>
        private static HitType? previousHitType;

        /// <summary>
        /// Length of the current mono pattern.
        /// </summary>
        private static int currentMonoLength;

        public static double EvaluateDifficulty(DifficultyHitObject current)
        {
            var tauCurrent = (TauDifficultyHitObject)current;

            if (tauCurrent.DeltaTime >= 1000)
            {
                mono_history.Clear();

                var currentHit = tauCurrent.BaseObject;
                currentMonoLength = currentHit != null ? 1 : 0;
                previousHitType = getHitType(tauCurrent);
            }

            double objectStrain = 0.0;

            if (previousHitType != null && (getHitType(tauCurrent) != previousHitType))
            {
                objectStrain = 1.0;

                if (mono_history.Count < 2)
                {
                    objectStrain = 0.0;
                }
                else if ((mono_history[^1] + currentMonoLength) % 2 == 0)
                {
                    objectStrain = 0.0;
                }

                objectStrain *= repetitionPenalties();
                currentMonoLength = 1;
            }
            else
            {
                currentMonoLength++;
            }

            previousHitType = getHitType(tauCurrent);
            return objectStrain;
        }

        public static double EvaluatePerformance(TauPerformanceContext context)
        {
            double rawComplexity = context.DifficultyAttributes.ComplexityDifficulty;

            double complexityValue = Math.Pow(5.0 * Math.Max(1.0, rawComplexity / 0.0675) - 4.0, 3.0) / 100000.0;

            // Penalize misses by assessing # of misses relative to the total # of objects. Default a 3% reduction for any # of misses.
            if (context.EffectiveMissCount > 0)
                complexityValue *= 0.97 * Math.Pow(1 - Math.Pow(context.EffectiveMissCount / context.TotalHits, 0.775), context.EffectiveMissCount);

            // Length bonus is added on for beatmaps with more than 2,000 hitobjects.
            double lengthBonus =
                0.95 + 0.4 * Math.Min(1.0, context.TotalHits / 2000.0) + (context.TotalHits > 2000 ? Math.Log10(context.TotalHits / 2000.0) * 0.5 : 0.0);
            complexityValue *= lengthBonus;

            return complexityValue;
        }

        /// <summary>
        /// The penalty to apply due to the length of repetition.
        /// </summary>
        private static double repetitionPenalties()
        {
            const int most_recent_patterns_to_compare = 2;
            double penalty = 1.0;

            mono_history.Enqueue(currentMonoLength);

            for (int start = mono_history.Count - most_recent_patterns_to_compare - 1; start >= 0; start--)
            {
                if (!isSamePattern(start, most_recent_patterns_to_compare))
                    continue;

                int notesSince = 0;
                for (int i = start; i < mono_history.Count; i++) notesSince += mono_history[i];
                penalty *= repetitionPenalty(notesSince);
                break;
            }

            return penalty;
        }

        /// <summary>
        /// Determines whether the last <paramref name="mostRecentPatternsToCompare"/> patterns have repeated in the history
        /// of note sequences, starting from <paramref name="start"/>.
        /// </summary>
        private static bool isSamePattern(int start, int mostRecentPatternsToCompare)
        {
            for (int i = 0; i < mostRecentPatternsToCompare; i++)
            {
                if (mono_history[start + i] != mono_history[mono_history.Count - mostRecentPatternsToCompare + i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Calculates the strain penalty for a pattern repetition.
        /// </summary>
        /// <param name="notesSince">The number of notes since the last repetition of the pattern.</param>
        private static double repetitionPenalty(int notesSince)
            => Math.Min(1.0, 0.032 * notesSince);

        private static HitType getHitType(TauDifficultyHitObject hitObject)
        {
            if (hitObject.BaseObject is StrictHardBeat)
                return HitType.HardBeat;

            if (hitObject.BaseObject.NestedHitObjects.Count > 0 && hitObject.BaseObject.NestedHitObjects[0] is SliderHardBeat)
                return HitType.HardBeat;

            return hitObject.BaseObject is AngledTauHitObject ? HitType.Angled : HitType.HardBeat;
        }

        private enum HitType
        {
            Angled,
            HardBeat
        }
    }
}
