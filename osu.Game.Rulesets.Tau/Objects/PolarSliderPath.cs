using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using osu.Framework.Bindables;
using osu.Framework.Caching;
using osu.Framework.Utils;

namespace osu.Game.Rulesets.Tau.Objects
{
    public class PolarSliderPath
    {
        [JsonIgnore]
        public IBindable<int> Version => version;

        private readonly Bindable<int> version = new();

        public readonly Bindable<double?> ExpectedDistance = new();

        public double Duration => Nodes.Max(n => n.Time);
        public SliderNode EndNode => Nodes.LastOrDefault();

        public readonly BindableList<SliderNode> Nodes = new();

        private readonly Cached pathCache = new();

        private double calculatedLength;

        public PolarSliderPath()
        {
            Nodes.CollectionChanged += (_, _) =>
            {
                invalidate();
            };
        }

        public PolarSliderPath(SliderNode[] nodes, double? expectedDistance = null)
            : this()
        {
            Nodes.AddRange(nodes);
            ExpectedDistance.Value = expectedDistance;
        }

        /// <summary>
        /// The distance of the path prior to lengthening/shortening to account for <see cref="ExpectedDistance"/>.
        /// </summary>
        public double CalculatedDistance
        {
            get
            {
                ensureValid();
                return calculatedLength;
            }
        }

        public IEnumerable<SliderNode> NodesBetween(float start, float end)
            => Nodes
              .SkipWhile(node => node.Time < start)
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

        private void invalidate()
        {
            pathCache.Invalidate();
            version.Value++;
        }

        private void ensureValid()
        {
            if (pathCache.IsValid)
                return;

            calculateLength();

            pathCache.Validate();
        }

        private void calculateLength()
        {
            calculatedLength = 0;

            if (Nodes.Count <= 0)
                return;

            (float angle, float sum) result = (angle: Nodes[0].Angle, sum: 0f);

            foreach (var node in Nodes)
            {
                result.sum += Math.Abs(Extensions.GetDeltaAngle(result.angle, node.Angle));
                result.angle = node.Angle;
            }

            calculatedLength = result.sum;
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
