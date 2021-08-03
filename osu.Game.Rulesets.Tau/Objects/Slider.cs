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
    public class Slider : TauHitObject, IHasDuration, IHasRepeats
    {
        public double Duration
        {
            get => Nodes.Max(n => n.Time);
            set { }
        }

        public double EndTime => StartTime + Duration;

        public BindableList<SliderNode> Nodes { get; set; }

        [JsonIgnore]
        public IList<HitSampleInfo> TailSamples { get; private set; }

        [JsonIgnore]
        public SliderHeadBeat HeadBeat { get; protected set; }

        protected override void CreateNestedHitObjects(CancellationToken cancellationToken)
        {
            base.CreateNestedHitObjects(cancellationToken);

            // There will always be a Head hit object.
            // NOTE: This should be replaced with SliderEventGenerator to support both ticks and repeats(?).
            AddNested(HeadBeat = new SliderHeadBeat
            {
                StartTime = StartTime,
                Angle = Nodes[0].Angle,
                SampleControlPoint = SampleControlPoint
            });

            updateNestedSamples();
        }

        private void updateNestedSamples()
        {
            if (HeadBeat != null)
                HeadBeat.Samples = this.GetNodeSamples(0);

            TailSamples = this.GetNodeSamples(RepeatCount + 1);
        }

        protected override HitWindows CreateHitWindows() => HitWindows.Empty;

        public int RepeatCount { get; set; }
        public List<IList<HitSampleInfo>> NodeSamples { get; set; } = new List<IList<HitSampleInfo>>();
    }
}
