using System;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.Objects;
using osuTK;

namespace osu.Game.Rulesets.Tau.Difficulty.Preprocessing
{
    public class TauDifficultyHitObject : DifficultyHitObject
    {
        protected new TauHitObject BaseObject => (TauHitObject)base.BaseObject;

        /// <summary>
        /// Normalized distance from the end position of the previous <see cref="TauDifficultyHitObject"/> to the start position of this <see cref="OsuDifficultyHitObject"/>.
        /// </summary>
        public double JumpDistance { get; private set; }

        /// <summary>
        /// Normalized distance between the start and end position of the previous <see cref="TauDifficultyHitObject"/>.
        /// </summary>
        public double TravelDistance { get; private set; }

        /// <summary>
        /// Angle the player has to take to hit this <see cref="TauDifficultyHitObject"/>.
        /// Calculated as the angle between the circles (current-2, current-1, current).
        /// </summary>
        public double? Angle { get; private set; }

        /// <summary>
        /// Milliseconds elapsed since the start time of the previous <see cref="TauDifficultyHitObject"/>, with a minimum of 50ms.
        /// </summary>
        public readonly double StrainTime;

        public IBeatmap beatmap;

        public readonly TauHitObject lastLastObject;
        public readonly TauHitObject lastObject;

        public TauDifficultyHitObject(HitObject hitObject, HitObject lastLastObject, HitObject lastObject, double clockRate, IBeatmap bm)
            : base(hitObject, lastObject, clockRate)
        {
            this.lastLastObject = (TauHitObject)lastLastObject;
            this.lastObject = (TauHitObject)lastObject;
            this.beatmap = bm;

            setDistances();

            // Every strain interval is hard capped at the equivalent of 375 BPM streaming speed as a safety measure
            StrainTime = Math.Max(50, DeltaTime);
        }

        private void setDistances()
        {

            JumpDistance = (getEndCursorPosition(BaseObject) - getEndCursorPosition(lastObject)).Length;

            if (lastLastObject != null)
            {  
                // Vector2 lastLastCursorPosition = getEndCursorPosition(lastLastObject);

                // Vector2 v1 = lastLastCursorPosition - getEndCursorPosition(lastObject);
                // Vector2 v2 = getEndCursorPosition(BaseObject) - lastCursorPosition;

                // float dot = Vector2.Dot(v1, v2);
                // float det = v1.X * v2.Y - v1.Y * v2.X;
                // Angle = Math.Abs(Math.Atan2(det, dot));

                // Inscribed angle
                Angle = 0.5f * (lastLastObject.Angle - BaseObject.Angle);
            }
        }

        

        private Vector2 getEndCursorPosition(TauHitObject hitObject)
        {
            // Vector2 pos = hitObject.Position;
            Vector2 pos = Extensions.GetCircularPosition(10f, hitObject.Angle); //TODO: change 10f to radius of tau playfield and adjust sr elsewhere

            return pos;
        }
    }
}