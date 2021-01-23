using System;
using System.Linq;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Tau.Objects
{
    public class Slider : TauHitObject, IHasDuration
    {
        public double Duration
        {
            get => Nodes.Max(n => n.Time);
            set => throw new NotSupportedException();
        }

        public double EndTime => StartTime + Duration;

        public SliderNode[] Nodes { get; set; }

        protected override HitWindows CreateHitWindows() => HitWindows.Empty;

        internal Vector2? EndPosition;

        /// <summary>
        /// The distance travelled by the cursor upon completion of this <see cref="Slider"/> if it was hit
        /// with as few movements as possible. This is set and used by difficulty calculation.
        /// </summary>
        internal float LazyTravelDistance;
    }
}
