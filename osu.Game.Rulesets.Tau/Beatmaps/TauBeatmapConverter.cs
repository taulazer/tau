using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using osu.Framework.Bindables;
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

        public bool CanConvertToSliders = true;
        public bool CanConvertToHardBeats = true;

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
                    if (!CanConvertToSliders)
                        goto default;

                    if (pathData.Duration < IBeatmapDifficultyInfo.DifficultyRange(Beatmap.BeatmapInfo.BaseDifficulty.ApproachRate, 1800, 1200, 450) / 2)
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
                        NodeSamples = pathData.NodeSamples,
                        NewCombo = comboData?.NewCombo ?? false,
                        ComboOffset = comboData?.ComboOffset ?? 0,
                        Nodes = new BindableList<SliderNode>(nodes),
                    }.Yield();

                case IHasDuration durationData:
                    //Is a spinner, should use a slider.
                    if (!CanConvertToSliders)
                        goto default;

                    // Should check if less than a desired time...
                    if (durationData.Duration < IBeatmapDifficultyInfo.DifficultyRange(Beatmap.BeatmapInfo.BaseDifficulty.ApproachRate, 1800, 1200, 450) / 2)
                        goto default;

                    // Please don't convert spinners that are negative..
                    if (durationData.Duration <= 0)
                        goto default;

                    var sliderNodes = new List<SliderNode>();
                    // should go in direction of previous object, otherwise, go anti-clockwise.
                    // True = clockwise, False = antiClockwise.
                    bool direction = beatmap.HitObjects.GetPrevious(original) is IHasPosition previous && -1 * previous.Position.GetHitObjectAngle() > 0;

                    // amount of nodes should be dependent on how many quarter revolutions it can do.
                    // Let's do a sane one and make it change on bpm later on... (0.5x = 2 seconds)
                    double nodeDuration = (1000 * original.DifficultyControlPoint.SliderVelocity) / 4;
                    float currAngle = 0;

                    for (double time = 0; time < durationData.Duration; time += nodeDuration)
                    {
                        sliderNodes.Add(new SliderNode((float)time, currAngle));
                        currAngle += direction ? 45 : -45;
                    }

                    return new Slider
                    {
                        Samples = sample,
                        StartTime = original.StartTime,
                        NewCombo = true,
                        Nodes = new BindableList<SliderNode>(sliderNodes),
                    }.Yield();

                default:
                    if (isHard && CanConvertToHardBeats)
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
            DifficultyControlPoint difficultyPoint = original.DifficultyControlPoint;

            double scoringDistance = 100 * difficulty.SliderMultiplier * difficultyPoint.SliderVelocity;

            var velocity = scoringDistance / timingPoint.BeatLength;
            var tickDistance = scoringDistance / difficulty.SliderTickRate;

            double legacyLastTickOffset = (original as IHasLegacyLastTickOffset)?.LegacyLastTickOffset ?? 0;

            foreach (var e in SliderEventGenerator.Generate(original.StartTime, spanDuration, velocity, tickDistance, curve.Path.Distance, curve.RepeatCount + 1, legacyLastTickOffset,
                CancellationToken.None))
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
