﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using osu.Framework.Bindables;
using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Tau.Objects
{
    public class Slider : AngledTauHitObject, IHasRepeats, IHasOffsetAngle
    {
        public double Duration
        {
            get => Nodes.Max(n => n.Time);
            set { }
        }

        public double EndTime => StartTime + Duration;

        public SliderNode EndNode => Nodes.LastOrDefault();

        public override IList<HitSampleInfo> AuxiliarySamples => CreateSlidingSamples().Concat(TailSamples).ToArray();

        public IList<HitSampleInfo> CreateSlidingSamples()
        {
            var slidingSamples = new List<HitSampleInfo>();

            var normalSample = Samples.FirstOrDefault(s => s.Name == HitSampleInfo.HIT_NORMAL);
            if (normalSample != null)
                slidingSamples.Add(normalSample.With("sliderslide"));

            var whistleSample = Samples.FirstOrDefault(s => s.Name == HitSampleInfo.HIT_WHISTLE);
            if (whistleSample != null)
                slidingSamples.Add(whistleSample.With("sliderwhistle"));

            return slidingSamples;
        }

        [JsonIgnore]
        public SliderHeadBeat HeadBeat { get; protected set; }

        public BindableList<SliderNode> Nodes { get; set; }

        private SliderPath path;

        [JsonIgnore]
        public SliderPath Path
        {
            get
            {
                if (path != null)
                    return path;

                var positions = Nodes.Select(node => new Vector2(node.Time * 99999999, node.Angle)).ToList();
                return path = new SliderPath(PathType.Linear, positions.ToArray());
            }
        }

        /// <summary>
        /// The length of one span of this <see cref="Slider"/>.
        /// </summary>
        public double SpanDuration => Duration / this.SpanCount();

        /// <summary>
        /// Velocity of this <see cref="Slider"/>.
        /// </summary>
        public double Velocity { get; private set; }

        /// <summary>
        /// Spacing between <see cref="SliderTick"/>s of this <see cref="Slider"/>.
        /// </summary>
        public double TickDistance { get; private set; }

        /// <summary>
        /// An extra multiplier that affects the number of <see cref="SliderTick"/>s generated by this <see cref="Slider"/>.
        /// An increase in this value increases <see cref="TickDistance"/>, which reduces the number of ticks generated.
        /// </summary>
        public double TickDistanceMultiplier = 4;

        [JsonIgnore]
        public IList<HitSampleInfo> TailSamples { get; private set; }

        public const int BASE_SCORING_DISTANCE = 100;

        protected override void ApplyDefaultsToSelf(ControlPointInfo controlPointInfo, IBeatmapDifficultyInfo difficulty)
        {
            base.ApplyDefaultsToSelf(controlPointInfo, difficulty);

            TimingControlPoint timingPoint = controlPointInfo.TimingPointAt(StartTime);

            Velocity = BASE_SCORING_DISTANCE / timingPoint.BeatLength;
            TickDistance = BASE_SCORING_DISTANCE / difficulty.SliderTickRate * TickDistanceMultiplier;
        }

        protected override void CreateNestedHitObjects(CancellationToken cancellationToken)
        {
            base.CreateNestedHitObjects(cancellationToken);

            var sliderEvents = SliderEventGenerator.Generate(StartTime, SpanDuration, Velocity, TickDistance, Duration, this.SpanCount(), null, cancellationToken);

            int nodeIndex = 0;

            void seek(float time)
            {
                nodeIndex = 0;
                while (nodeIndex > 0 && Nodes[nodeIndex - 1].Time > time)
                    nodeIndex--;
                while (nodeIndex + 1 < Nodes.Count && Nodes[nodeIndex + 1].Time <= time)
                    nodeIndex++;
            }

            float angleAt(float time)
            {
                seek(time);
                if (nodeIndex + 1 == Nodes.Count)
                    return Nodes[nodeIndex].Angle;
                if (Nodes.Count == 1)
                    return Nodes[0].Angle;

                var nodeA = Nodes[nodeIndex];
                var nodeB = Nodes[nodeIndex + 1];
                var deltaAngle = Extensions.GetDeltaAngle(nodeB.Angle, nodeA.Angle);
                var duration = nodeB.Time - nodeA.Time;
                if (duration == 0)
                    return nodeB.Angle;

                return nodeA.Angle + deltaAngle * (time - nodeA.Time) / duration;
            }

            foreach (var e in sliderEvents)
            {
                switch (e.Type)
                {
                    case SliderEventType.Head:
                        AddNested(HeadBeat = new SliderHeadBeat
                        {
                            ParentSlider = this,
                            StartTime = StartTime,
                            Angle = Nodes[0].Angle
                        });
                        break;

                    case SliderEventType.Repeat:
                        var time = (e.SpanIndex + 1) * SpanDuration;
                        var pos = Path.PositionAt(time / Duration);
                        AddNested(new SliderRepeat
                        {
                            ParentSlider = this,
                            RepeatIndex = e.SpanIndex,
                            StartTime = StartTime + time,
                            Angle = pos.Y
                        });
                        break;

                    case SliderEventType.Tick:
                        AddNested(new SliderTick()
                        {
                            ParentSlider = this,
                            StartTime = e.Time,
                            Angle = angleAt((float)(e.Time - StartTime))
                        });
                        break;
                }
            }

            updateNestedSamples();
        }

        private void updateNestedSamples()
        {
            var firstSample = Samples.FirstOrDefault(s => s.Name == HitSampleInfo.HIT_NORMAL)
                              ?? Samples.FirstOrDefault();
            var sampleList = new List<HitSampleInfo>();
            if (firstSample != null)
                sampleList.Add(firstSample.With("slidertick"));

            foreach (var repeat in NestedHitObjects.OfType<SliderRepeat>())
                repeat.Samples = this.GetNodeSamples(repeat.RepeatIndex + 1);
            foreach (var tick in NestedHitObjects.OfType<SliderTick>())
                tick.Samples = sampleList;
            if (HeadBeat != null)
                HeadBeat.Samples = this.GetNodeSamples(0);

            TailSamples = this.GetNodeSamples(RepeatCount + 1);
        }

        protected override HitWindows CreateHitWindows() => HitWindows.Empty;

        public float GetAbsoluteAngle(SliderNode node) => Angle + node.Angle;

        public float GetOffsetAngle() => EndNode.Angle;

        public int RepeatCount { get; set; }
        public IList<IList<HitSampleInfo>> NodeSamples { get; set; } = new List<IList<HitSampleInfo>>();

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
        }
    }
}
