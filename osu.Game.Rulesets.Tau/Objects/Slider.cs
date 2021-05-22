using System;
using System.Linq;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Tau.Objects
{
    public class Slider : TauHitObject, IHasDuration
    {
        public double Duration
        {
            get => Nodes.Max(n => n.Time);
            set { }
        }

        public double EndTime => StartTime + Duration;

        public SliderNode[] Nodes { get; set; }

        protected override HitWindows CreateHitWindows() => HitWindows.Empty;
    }
}
