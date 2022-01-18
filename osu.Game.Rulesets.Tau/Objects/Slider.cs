using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using osu.Framework.Bindables;
using osu.Game.Audio;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Tau.Objects
{
    public class Slider : TauHitObject, IHasRepeats
    {
        public double Duration
        {
            get => Nodes.Max(n => n.Time);
            set { }
        }

        public double EndTime => StartTime + Duration;

        [JsonIgnore]
        public SliderHeadBeat HeadBeat { get; protected set; }

        public BindableList<SliderNode> Nodes { get; set; }

        protected override void CreateNestedHitObjects(CancellationToken cancellationToken)
        {
            base.CreateNestedHitObjects(cancellationToken);

            // NOTE (nao): I'm considering replacing this entire function with the SliderEventGenerator.
            AddNested(HeadBeat = new SliderHeadBeat
            {
                StartTime = StartTime,
                Angle = Nodes[0].Angle
            });

            updateNestedSamples();
        }

        private void updateNestedSamples()
        {
            if (HeadBeat != null)
                HeadBeat.Samples = this.GetNodeSamples(0);

            Samples = this.GetNodeSamples(RepeatCount + 1);
        }

        protected override HitWindows CreateHitWindows() => HitWindows.Empty;

        public int RepeatCount { get; set; }
        public IList<IList<HitSampleInfo>> NodeSamples { get; set; } = new List<IList<HitSampleInfo>>();

        public struct SliderNode
        {
            public float Time { get; }

            public float Angle { get; }

            public SliderNode(float time, float angle)
            {
                Time = time;
                Angle = angle;
            }
        }
    }
}
