using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Legacy;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Tau.Objects;
using osuTK;

namespace osu.Game.Rulesets.Tau.Beatmaps
{
    public class TauBeatmapConverter : BeatmapConverter<TauHitObject>
    {
        // TODO: Create a more robust system.
        public override bool CanConvert() => true;

        protected override Beatmap<TauHitObject> CreateBeatmap() => new TauBeatmap();

        public bool CanConvertToHardBeats { get; set; } = true;
        public bool CanConvertToSliders { get; set; } = true;
        public bool CanConvertImpossibleSliders { get; set; }
        public int SliderDivisor { get; set; } = 4;

        public static readonly Vector2 STANDARD_PLAYFIELD_SIZE = new(512, 384);
        public static readonly Vector2 STANDARD_PLAYFIELD_CENTER = STANDARD_PLAYFIELD_SIZE / 2;

        public TauBeatmapConverter(Ruleset ruleset, IBeatmap beatmap)
            : base(beatmap, ruleset)
        {
        }

        protected override IEnumerable<TauHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap, CancellationToken cancellationToken)
        {
            var comboData = original as IHasCombo;

            return original switch
            {
                IHasPathWithRepeats path => convertToSlider(original, comboData, path, beatmap).Yield(),
                IHasDuration duration => convertToSliderSpinner(original, comboData, duration, beatmap).Yield(),
                _ => convertToNonSlider(original).Yield()
            };
        }

        private TauHitObject convertToNonSlider(HitObject original)
        {
            bool isHard = (original is IHasPathWithRepeats tmp ? tmp.NodeSamples[0] : original.Samples).Any(s => s.Name == HitSampleInfo.HIT_FINISH);
            var comboData = original as IHasCombo;
            var sample = original is IHasPathWithRepeats c ? c.NodeSamples[0] : null;

            if (isHard && CanConvertToHardBeats)
                return convertToHardBeat(original, comboData, sample);

            return convertToBeat(original, comboData, sample);
        }

        private float getHitObjectAngle(HitObject original)
            => original switch
            {
                IHasPosition pos => pos.Position.GetHitObjectAngle(),
                IHasXPosition xPos => xPos.X.Remap(0, STANDARD_PLAYFIELD_SIZE.X, 0, 360),
                IHasYPosition yPos => yPos.Y.Remap(0, STANDARD_PLAYFIELD_SIZE.Y, 0, 360),
                IHasAngle ang => ang.Angle,
                _ => 0
            };

        private TauHitObject convertToBeat(HitObject original, IHasCombo comboData, IList<HitSampleInfo> samples = null)
            => new Beat
            {
                Samples = samples ?? original.Samples,
                StartTime = original.StartTime,
                Angle = getHitObjectAngle(original),
                NewCombo = comboData?.NewCombo ?? false,
                ComboOffset = comboData?.ComboOffset ?? 0,
            };

        private TauHitObject convertToHardBeat(HitObject original, IHasCombo comboData, IList<HitSampleInfo> samples = null)
            => new HardBeat
            {
                Samples = samples ?? original.Samples,
                StartTime = original.StartTime,
                NewCombo = comboData?.NewCombo ?? false,
                ComboOffset = comboData?.ComboOffset ?? 0,
            };

        private TauHitObject convertToSlider(HitObject original, IHasCombo comboData, IHasPathWithRepeats data, IBeatmap beatmap)
        {
            if (!CanConvertToSliders)
                return convertToNonSlider(original);

            var difficultyInfo = beatmap.Difficulty;

            if (data.Duration < IBeatmapDifficultyInfo.DifficultyRange(difficultyInfo.ApproachRate, 1800, 1200, 450) / SliderDivisor)
                return convertToNonSlider(original);

            var nodes = new List<SliderNode>();

            float? lastAngle = null;
            float? lastTime = null;
            float firstAngle = 0f;

            for (int t = 0; t < data.Duration; t += 20)
            {
                float angle = (((IHasPosition)original).Position + data.CurvePositionAt(t / data.Duration)).GetHitObjectAngle();

                if (t == 0)
                    firstAngle = angle;

                angle = Extensions.GetDeltaAngle(angle, firstAngle);

                // We don't want sliders that switch angles too fast. We would default to a normal note in this case
                if (!CanConvertImpossibleSliders)
                    if (lastAngle.HasValue && MathF.Abs(Extensions.GetDeltaAngle(lastAngle.Value, angle)) / MathF.Abs(lastTime.Value - t) > 0.6)
                        return convertToNonSlider(original);

                lastAngle = angle;
                lastTime = t;
                nodes.Add(new SliderNode(t, angle));
            }

            var finalAngle = (((IHasPosition)original).Position + data.CurvePositionAt(1)).GetHitObjectAngle();
            finalAngle = Extensions.GetDeltaAngle(finalAngle, firstAngle);

            if (!CanConvertImpossibleSliders)
                if (lastAngle.HasValue && MathF.Abs(Extensions.GetDeltaAngle(lastAngle.Value, finalAngle)) / Math.Abs(lastTime.Value - data.Duration) > 0.6)
                    return convertToNonSlider(original);

            nodes.Add(new SliderNode((float)data.Duration, finalAngle));

            var slider = new Slider
            {
                Samples = original.Samples,
                StartTime = original.StartTime,
                NodeSamples = data.NodeSamples,
                RepeatCount = data.RepeatCount,
                Angle = firstAngle,
                Path = new PolarSliderPath(nodes),
                NewCombo = comboData?.NewCombo ?? false,
                ComboOffset = comboData?.ComboOffset ?? 0,
            };

            if (beatmap.ControlPointInfo is LegacyControlPointInfo legacyControlPointInfo)
            {
                // prior to v8, speed multipliers don't adjust for how many ticks are generated over the same distance.
                // this results in more (or less) ticks being generated in <v8 maps for the same time duration.
                slider.TickDistanceMultiplier = beatmap.BeatmapInfo.BeatmapVersion < 8
                                                    ? 2f / legacyControlPointInfo.DifficultyPointAt(original.StartTime).SliderVelocity
                                                    : 2;
            }

            return slider;
        }

        private TauHitObject convertToSliderSpinner(HitObject original, IHasCombo comboData, IHasDuration duration, IBeatmap beatmap)
        {
            if (!CanConvertToSliders)
                return convertToNonSlider(original);

            var difficultyInfo = beatmap.Difficulty;

            if (duration.Duration < IBeatmapDifficultyInfo.DifficultyRange(difficultyInfo.ApproachRate, 1800, 1200, 450) / SliderDivisor)
                return convertToNonSlider(original);

            var nodes = new List<SliderNode>();
            var direction = Math.Sign(getHitObjectAngle(beatmap.HitObjects.GetPrevious(original)));

            if (direction == 0)
                direction = 1; // Direction should always default to Clockwise.

            var controlPoint = beatmap.ControlPointInfo.TimingPointAt(original.StartTime);

            var revolutions = (int)(duration.Duration / (controlPoint.BeatLength * controlPoint.TimeSignature.Numerator));
            var revDuration = duration.Duration / revolutions;

            if (revolutions == 0)
                return convertToNonSlider(original);

            var currentAngle = 0f;

            for (int i = 0; i < revolutions; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    nodes.Add(new SliderNode((float)((revDuration / 4) * (j + 4 * i)), currentAngle));
                    currentAngle += 90 * direction;
                }
            }

            // For some reason, while this does solve the wrong duration for "spinners", this also somehow "slows down" the path of the slider.
            // Will need to do some investigation as to why this is the case.
            nodes.Add(new SliderNode((float)duration.Duration, 0));

            var slider = new Slider
            {
                Samples = original.Samples,
                StartTime = original.StartTime,
                Path = new PolarSliderPath(nodes.ToArray()),
                NewCombo = comboData?.NewCombo ?? false,
                ComboOffset = comboData?.ComboOffset ?? 0,
            };

            if (beatmap.ControlPointInfo is LegacyControlPointInfo legacyControlPointInfo)
            {
                // prior to v8, speed multipliers don't adjust for how many ticks are generated over the same distance.
                // this results in more (or less) ticks being generated in <v8 maps for the same time duration.
                slider.TickDistanceMultiplier = beatmap.BeatmapInfo.BeatmapVersion < 8
                                                    ? 2f / legacyControlPointInfo.DifficultyPointAt(original.StartTime).SliderVelocity
                                                    : 2;
            }

            return slider;
        }
    }

    public static class TauBeatmapConverterExtensions
    {
        /// <summary>
        /// Gets the theta angle from the playfield's center.
        /// Note that this only works for legacy sizing (512x384)
        /// </summary>
        /// <param name="target">The target <see cref="HitObject"/> position.</param>
        public static float GetHitObjectAngle(this Vector2 target)
            => TauBeatmapConverter.STANDARD_PLAYFIELD_CENTER.GetDegreesFromPosition(target);
    }
}
