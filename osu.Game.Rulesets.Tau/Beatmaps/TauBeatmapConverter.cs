using System.Collections.Generic;
using System.Linq;
using System.Threading;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Beatmaps
{
    public class TauBeatmapConverter : BeatmapConverter<TauHitObject>
    {
        public override bool CanConvert() => Beatmap.HitObjects.All(h => h is IHasPosition);

        public TauBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
        }

        protected override IEnumerable<TauHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap, CancellationToken cancellationToken)
        {
            var position = ((IHasPosition)original).Position;
            var comboData = original as IHasCombo;
            bool isHard = (original is IHasPathWithRepeats tmp ? tmp.NodeSamples[0] : original.Samples).Any(s => s.Name == HitSampleInfo.HIT_FINISH);
            var sample = original is IHasPathWithRepeats c ? c.NodeSamples[0] : original.Samples;

            switch (original)
            {
                case IHasPath pathData:
                    var nodes = new List<SliderNode>();

                    foreach (var point in pathData.Path.ControlPoints)
                    {
                        var time = pathData.Duration / pathData.Path.ControlPoints.Count * pathData.Path.ControlPoints.IndexOf(point);
                        nodes.Add(new SliderNode((float)time, (position + point.Position.Value).GetHitObjectAngle()));
                    }

                    return new Slider
                    {
                        Samples = sample,
                        StartTime = original.StartTime,
                        NewCombo = comboData?.NewCombo ?? false,
                        ComboOffset = comboData?.ComboOffset ?? 0,
                        Nodes = nodes.ToArray(),
                    }.Yield();

                default:
                    if (isHard)
                        return new HardBeat
                        {
                            Samples = sample,
                            StartTime = original.StartTime,
                            NewCombo = comboData?.NewCombo ?? false,
                            ComboOffset = comboData?.ComboOffset ?? 0,
                        }.Yield();
                    else
                        return new Beat
                        {
                            Samples = sample,
                            StartTime = original.StartTime,
                            Angle = position.GetHitObjectAngle(),
                            NewCombo = comboData?.NewCombo ?? false,
                            ComboOffset = comboData?.ComboOffset ?? 0,
                        }.Yield();
            }
        }

        protected override Beatmap<TauHitObject> CreateBeatmap() => new TauBeatmap();
    }
}
