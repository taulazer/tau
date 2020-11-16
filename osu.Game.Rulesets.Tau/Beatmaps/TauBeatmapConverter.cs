using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
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
                case IHasPathWithRepeats pathData:

                    if (pathData.Duration < BeatmapDifficulty.DifficultyRange(Beatmap.BeatmapInfo.BaseDifficulty.ApproachRate, 1800, 1200, 450) / 2)
                        goto default;
                    var nodes = new List<SliderNode>();

                    float? lastAngle = null;
                    double? lastTime = null;

                    for (double t = 0; t < pathData.Duration; t += 20)
                    {
                        float angle = ((original as IHasPosition).Position + pathData.CurvePositionAt(t / pathData.Duration)).GetHitObjectAngle();

                        // We don't want sliders that switch angles too fast. We would default to a normal note in this case
                        if (lastAngle.HasValue && (Math.Abs(Extensions.GetDeltaAngle(lastAngle.Value, angle)) / (float)Math.Abs(lastTime.Value - t)) > 0.6)
                            goto default;

                        lastAngle = angle;
                        lastTime = t;
                        nodes.Add(new SliderNode((float)t, angle));
                    }

                    float finalAngle = ((original as IHasPosition).Position + pathData.CurvePositionAt(1)).GetHitObjectAngle();
                    if (lastAngle.HasValue && (Math.Abs(Extensions.GetDeltaAngle(lastAngle.Value, finalAngle)) / (float)Math.Abs(lastTime.Value - pathData.Duration)) > 0.6)
                        goto default;

                    nodes.Add(new SliderNode((float)pathData.Duration, finalAngle));

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

        private IEnumerable<SliderNode> createNodeFromTicks(HitObject original)
        {
            var curve = original as IHasPathWithRepeats;
            double spanDuration = curve.Duration / (curve.RepeatCount + 1);
            bool isRepeatSpam = spanDuration < 75 && curve.RepeatCount > 0;

            if (isRepeatSpam)
                yield break;

            var difficulty = Beatmap.BeatmapInfo.BaseDifficulty;

            var controlPointInfo = Beatmap.ControlPointInfo;
            TimingControlPoint timingPoint = controlPointInfo.TimingPointAt(original.StartTime);
            DifficultyControlPoint difficultyPoint = controlPointInfo.DifficultyPointAt(original.StartTime);

            double scoringDistance = 100 * difficulty.SliderMultiplier * difficultyPoint.SpeedMultiplier;

            var velocity = scoringDistance / timingPoint.BeatLength;
            var tickDistance = scoringDistance / difficulty.SliderTickRate;

            double legacyLastTickOffset = (original as IHasLegacyLastTickOffset)?.LegacyLastTickOffset ?? 0;

            foreach (var e in SliderEventGenerator.Generate(original.StartTime, spanDuration, velocity, tickDistance, curve.Path.Distance, curve.RepeatCount + 1, legacyLastTickOffset, CancellationToken.None))
            {
                switch (e.Type)
                {
                    case SliderEventType.Repeat:
                        yield return new SliderNode((float)(e.Time - original.StartTime), Extensions.GetHitObjectAngle(curve.CurvePositionAt(e.PathProgress)));
                        break;
                }
            }
        }
    }
}
