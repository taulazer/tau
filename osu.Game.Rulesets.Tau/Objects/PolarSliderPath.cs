using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using osu.Framework.Bindables;
using osu.Framework.Caching;

namespace osu.Game.Rulesets.Tau.Objects
{
    public class PolarSliderPath
    {
        [JsonIgnore]
        public IBindable<int> Version => version;

        private readonly Bindable<int> version = new();

        public double Duration => EndNode.Time - StartNode.Time;
        public SliderNode EndNode => Nodes.Any() ? Nodes[^1] : default;
        public SliderNode StartNode => Nodes.Any() ? Nodes[0] : default;

        public readonly BindableList<SliderNode> Nodes = new();
        private readonly Cached pathCache = new();

        public PolarSliderPath(IEnumerable<SliderNode> nodes)
        {
            Nodes.AddRange(nodes);
            Nodes.BindCollectionChanged((_, _) =>
            {
                // TODO ensure the list is sorted
                invalidate();
            });
        }

        private double calculatedLength;

        /// <summary>
        /// The distance of the path (in degrees)
        /// </summary>
        public double CalculatedDistance
        {
            get
            {
                ensureValid();
                return calculatedLength;
            }
        }

        // we are expecting the inputs to be similar, and as such caching the current seeker position speeds the process up
        private int nodeIndex;

        /// <summary>
        /// Seeks the <see cref="nodeIndex"/> such that the node at <see cref="nodeIndex"/> is before or at <paramref name="time"/>
        /// and the next node is after <paramref name="time"/> unless there are no more nodes to seek in a given direction.
        /// </summary>
        private void seekTo(float time)
        {
            while (nodeIndex > 0 && Nodes[nodeIndex - 1].Time > time)
                nodeIndex--;
            while (nodeIndex + 1 < Nodes.Count && Nodes[nodeIndex + 1].Time <= time)
                nodeIndex++;
        }

        public NodesEnumerable NodesBetween(float start, float end)
        {
            seekTo(start);
            return new(nodeIndex + 1, end, this);
        }

        public SegmentsEnumerable SegmentsBetween(float start, float end)
        {
            seekTo(start);
            return new(Math.Max(nodeIndex - 1, 0), start, end, this);
        }

        /// <summary>
        /// Returns an interpolated node at a given time
        /// </summary>
        public SliderNode NodeAt(float time)
        {
            if (!Nodes.Any())
                return default;

            if (time <= Nodes[0].Time)
                return Nodes[0];

            if (time >= Nodes[^1].Time)
                return Nodes[^1];

            seekTo(time);
            var from = Nodes[nodeIndex];
            var to = Nodes[nodeIndex + 1];
            float delta = Extensions.GetDeltaAngle(to.Angle, from.Angle);

            // no need to check for div by 0 because the seek skips over 0-duration nodes
            return new(time, from.Angle + delta * (time - from.Time) / (to.Time - from.Time));
        }

        public float AngleAt(float time)
            => NodeAt(time).Angle;

        private void invalidate()
        {
            pathCache.Invalidate();
            version.Value++;
            nodeIndex = 0;
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

            float lastAngle = Nodes[0].Angle;

            foreach (var node in Nodes)
            {
                calculatedLength += Math.Abs(Extensions.GetDeltaAngle(node.Angle, lastAngle));
                lastAngle = node.Angle;
            }
        }

        public float CalculateLazyDistance(float halfTolerance)
        {
            if (Nodes.Count <= 0)
                return 0;

            float length = 0f;
            float lastAngle = Nodes[0].Angle;

            foreach (var node in Nodes)
            {
                float delta = Extensions.GetDeltaAngle(node.Angle, lastAngle);

                if (MathF.Abs(delta) > halfTolerance)
                {
                    lastAngle += delta - (delta > 0 ? halfTolerance : -halfTolerance);
                    length += MathF.Abs(delta);
                }
            }

            return length;
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

    public readonly struct SliderSegment
    {
        public SliderNode From { get; }
        public SliderNode To { get; }
        public float DeltaAngle => Extensions.GetDeltaAngle(To.Angle, From.Angle);
        public float Duration => To.Time - From.Time;

        public SliderSegment(SliderNode from, SliderNode to)
        {
            From = from;
            To = to;
        }

        public SegmentNodesEnumerable Split(float timeStep = 20, float maxAnglePerMs = 5, bool excludeFirst = false, bool excludeLast = false)
        {
            float duration = Duration;
            float delta = DeltaAngle;
            int steps;

            if (duration == 0)
            {
                steps = (int)MathF.Ceiling(MathF.Abs(delta) / maxAnglePerMs);
            }
            else
            {
                float anglePerMs = delta / duration;
                timeStep = Math.Min(timeStep, Math.Abs(maxAnglePerMs / anglePerMs));
                steps = (int)MathF.Ceiling(duration / timeStep);
            }

            steps += 2;
            return new(
                excludeFirst ? new(From.Time + duration / steps, From.Angle + delta / steps) : From,
                excludeLast ? new(To.Time - duration / steps, To.Angle - delta / steps) : To,
                steps - (excludeFirst ? 1 : 0) - (excludeLast ? 1 : 0)
            );
        }

        public override string ToString() => $"({From}) -> ({To})";
    }

    public readonly struct NodesEnumerable : IEnumerable<SliderNode>
    {
        private readonly int index;
        private readonly float endTime;
        private readonly PolarSliderPath path;

        public NodesEnumerable(int index, float endTime, PolarSliderPath path)
        {
            this.index = index;
            this.endTime = endTime;
            this.path = path;
        }

        public NodesEnumerator GetEnumerator() => new(index, endTime, path);

        IEnumerator<SliderNode> IEnumerable<SliderNode>.GetEnumerator()
        {
            foreach (var i in this)
                yield return i;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<SliderNode>)this).GetEnumerator();

        public struct NodesEnumerator
        {
            private int index;
            private readonly float endTime;
            private readonly PolarSliderPath path;

            public NodesEnumerator(int index, float endTime, PolarSliderPath path)
            {
                this.index = index - 1;
                this.endTime = endTime;
                this.path = path;
            }

            public bool MoveNext()
            {
                if (index + 1 < path.Nodes.Count && path.Nodes[index + 1].Time < endTime)
                {
                    index++;
                    return true;
                }

                return false;
            }

            public SliderNode Current => path.Nodes[index];
        }
    }

    public readonly struct SegmentsEnumerable : IEnumerable<SliderSegment>
    {
        private readonly int index;
        private readonly float startTime;
        private readonly float endTime;
        private readonly PolarSliderPath path;

        public SegmentsEnumerable(int index, float startTime, float endTime, PolarSliderPath path)
        {
            this.index = index;
            this.startTime = startTime;
            this.endTime = endTime;
            this.path = path;
        }

        IEnumerator<SliderSegment> IEnumerable<SliderSegment>.GetEnumerator()
        {
            foreach (var i in this)
                yield return i;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<SliderSegment>)this).GetEnumerator();

        public SegmentsEnumerator GetEnumerator() => new(index, startTime, endTime, path);

        public struct SegmentsEnumerator
        {
            private int index;
            private readonly float startTime;
            private readonly float endTime;
            private readonly PolarSliderPath path;

            public SegmentsEnumerator(int index, float startTime, float endTime, PolarSliderPath path)
            {
                this.index = index - 1;
                this.startTime = startTime;
                this.endTime = endTime;
                this.path = path;
            }

            public bool MoveNext()
            {
                if (index + 2 < path.Nodes.Count && path.Nodes[index + 1].Time <= endTime)
                {
                    index++;
                    return true;
                }

                return false;
            }

            public SliderSegment Current
            {
                get
                {
                    var from = path.Nodes[index];
                    var to = path.Nodes[index + 1];
                    float deltaAngle = Extensions.GetDeltaAngle(to.Angle, from.Angle);
                    float duration = to.Time - from.Time;

                    if (to.Time > endTime && duration != 0)
                    {
                        to = new(endTime, from.Angle + deltaAngle * (endTime - from.Time) / duration);
                    }

                    if (from.Time < startTime && duration != 0)
                    {
                        from = new(startTime, from.Angle + deltaAngle * (startTime - from.Time) / duration);
                    }

                    return new(from, to);
                }
            }
        }
    }

    public readonly struct SegmentNodesEnumerable : IEnumerable<SliderNode>
    {
        private readonly SliderNode from;
        private readonly SliderNode to;
        private readonly int steps;

        public SegmentNodesEnumerable(SliderNode from, SliderNode to, int steps)
        {
            this.from = from;
            this.to = to;
            this.steps = steps;
        }

        public SegmentNodesEnumerator GetEnumerator() => new(from, to, steps);

        IEnumerator<SliderNode> IEnumerable<SliderNode>.GetEnumerator()
        {
            foreach (var i in this)
                yield return i;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<SliderNode>)this).GetEnumerator();

        public struct SegmentNodesEnumerator
        {
            private int current = -1;
            private readonly SliderNode from;
            private readonly int steps;
            private readonly float span;
            private readonly float timeSpan;

            public SegmentNodesEnumerator(SliderNode from, SliderNode to, int steps)
            {
                this.from = from;
                this.steps = steps;

                if (steps <= 1)
                {
                    span = 0;
                    timeSpan = 0;
                }
                else
                {
                    span = Extensions.GetDeltaAngle(to.Angle, from.Angle) / (steps - 1);
                    timeSpan = (to.Time - from.Time) / (steps - 1);
                }
            }

            public bool MoveNext()
            {
                return ++current < steps;
            }

            public SliderNode Current => new(from.Time + timeSpan * current, from.Angle + span * current);
        }
    }
}
