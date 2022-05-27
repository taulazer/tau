using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Difficulty.Utils;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Difficulty.Preprocessing;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Difficulty.Skills
{
    public class Complexity : StrainDecaySkill
    {
        protected override double SkillMultiplier => 50;

        protected override double StrainDecayBase => 0.4;

        /// <summary>
        /// Maximum number of entries to keep in <see cref="monoHistory"/>.
        /// </summary>
        private const int mono_history_max_length = 5;

        /// <summary>
        /// Queue with the lengths of the last <see cref="mono_history_max_length"/> most recent mono patterns,
        /// with the most recent value at the end of the queue.
        /// </summary>
        private readonly LimitedCapacityQueue<int> monoHistory = new LimitedCapacityQueue<int>(mono_history_max_length);

        /// <summary>
        /// The <see cref="HitType"/> of the last object hit before the one being considered.
        /// </summary>
        private HitType? previousHitType;

        /// <summary>
        /// Length of the current mono pattern.
        /// </summary>
        private int currentMonoLength;

        public Complexity(Mod[] mods)
            : base(mods)
        {
        }

        protected override double StrainValueOf(DifficultyHitObject current)
        {
            var tauCurrent = (TauDifficultyHitObject)current;

            if (tauCurrent.DeltaTime >= 1000)
            {
                monoHistory.Clear();

                var currentHit = tauCurrent.BaseObject;
                currentMonoLength = currentHit != null ? 1 : 0;
                previousHitType = getHitType(tauCurrent);
            }

            double objectStrain = 0.0;

            if (previousHitType != null && (getHitType(tauCurrent) != previousHitType))
            {
                objectStrain = 1.0;

                if (monoHistory.Count < 2)
                {
                    objectStrain = 0.0;
                }
                else if ((monoHistory[^1] + currentMonoLength) % 2 == 0)
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

        /// <summary>
        /// The penalty to apply due to the length of repetition.
        /// </summary>
        private double repetitionPenalties()
        {
            const int most_recent_patterns_to_compare = 2;
            double penalty = 1.0;

            monoHistory.Enqueue(currentMonoLength);

            for (int start = monoHistory.Count - most_recent_patterns_to_compare - 1; start >= 0; start--)
            {
                if (!isSamePattern(start, most_recent_patterns_to_compare))
                    continue;

                int notesSince = 0;
                for (int i = start; i < monoHistory.Count; i++) notesSince += monoHistory[i];
                penalty *= repetitionPenalty(notesSince);
                break;
            }

            return penalty;
        }

        /// <summary>
        /// Determines whether the last <paramref name="mostRecentPatternsToCompare"/> patterns have repeated in the history
        /// of note sequences, starting from <paramref name="start"/>.
        /// </summary>
        private bool isSamePattern(int start, int mostRecentPatternsToCompare)
        {
            for (int i = 0; i < mostRecentPatternsToCompare; i++)
            {
                if (monoHistory[start + i] != monoHistory[monoHistory.Count - mostRecentPatternsToCompare + i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Calculates the strain penalty for a pattern repetition.
        /// </summary>
        /// <param name="notesSince">The number of notes since the last repetition of the pattern.</param>
        private double repetitionPenalty(int notesSince)
            => Math.Min(1.0, 0.032 * notesSince);

        private HitType getHitType(TauDifficultyHitObject hitObject)
            => hitObject.BaseObject is AngledTauHitObject ? HitType.Angled : HitType.HardBeat;

        #region PP Calculations

        public static double CalculatePerformance(TauPerformanceContext context)
        {
            double rawComplexity = context.DifficultyAttributes.ComplexityDifficulty;

            double complexityValue = Math.Pow(5.0 * Math.Max(1.0, rawComplexity / 0.0675) - 4.0, 3.0) / 100000.0;

            // Penalize misses by assessing # of misses relative to the total # of objects. Default a 3% reduction for any # of misses.
            if (context.EffectiveMissCount > 0)
                complexityValue *= 0.97 * Math.Pow(1 - Math.Pow(context.EffectiveMissCount / context.TotalHits, 0.775), context.EffectiveMissCount); // TODO: Figure values here.

            double lengthBonus =
                0.95 + 0.4 * Math.Min(1.0, context.TotalHits / 2000.0) + (context.TotalHits > 2000 ? Math.Log10(context.TotalHits / 2000.0) * 0.5 : 0.0);
            complexityValue *= lengthBonus;

            return complexityValue;
        }

        #endregion

        private enum HitType
        {
            Angled,
            HardBeat
        }
    }
}
