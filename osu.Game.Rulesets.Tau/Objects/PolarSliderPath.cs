using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Utils;

namespace osu.Game.Rulesets.Tau.Objects
{
    public class PolarSliderPath
    {
        public readonly BindableList<SliderNode> Nodes = new();

        public PolarSliderPath(SliderNode[] nodes)
        {
            Nodes.AddRange(nodes);
        }

        public IEnumerable<SliderNode> NodesBetween(float start, float end)
            => Nodes
              .Where(node => !(node.Time < start))
              .TakeWhile(node => !(node.Time > end));

        public SliderNode NodeAt(float time)
        {
            var last = Nodes.LastOrDefault();

            if (time <= 0)
                return Nodes.First();

            if (time >= last.Time)
                return last;

            var closest = Nodes.OrderBy(n => Math.Abs(time - n.Time)).ToArray();
            var start = closest[0];
            var end = closest[1];

            var index = Nodes.BinarySearch(start);

            if (index == Nodes.Count)
                return start;

            var angle = Interpolation.ValueAt(time, start.Angle, end.Angle, start.Time, end.Time);

            return new SliderNode(time, angle);
        }
    }

    public readonly struct SliderNode : IComparable<SliderNode>
    {
        public float Time { get; }

        public float Angle { get; }

        public SliderNode(float time, float angle)
        {
            Time = time;
            Angle = angle;
        }

        public int CompareTo(SliderNode other) => Time.CompareTo(other.Time);

        public override string ToString() => $"T: {Time} | A: {Angle}";
    }
}
