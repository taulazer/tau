using System;
using System.Linq;
using osu.Game.Rulesets.Objects.Types;

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
    }
}
