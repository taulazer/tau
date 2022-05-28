using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Difficulty.Preprocessing
{
    public class TauDifficultyHitObject : DifficultyHitObject
    {
        private const int min_delta_time = 25;

        public new TauHitObject BaseObject => (TauHitObject)base.BaseObject;

        public new TauHitObject LastObject => (TauHitObject)base.LastObject;

        /// <summary>
        /// Milliseconds elapsed since the start time of the previous <see cref="TauDifficultyHitObject"/>, with a minimum of 25ms.
        /// </summary>
        public double StrainTime;

        public TauDifficultyHitObject(HitObject hitObject, HitObject lastObject, double clockRate)
            : base(hitObject, lastObject, clockRate)
        {
            // Capped to 25ms to prevent difficulty calculation breaking from simultaneous objects.
            StrainTime = Math.Max(DeltaTime, min_delta_time);
        }
    }
}
