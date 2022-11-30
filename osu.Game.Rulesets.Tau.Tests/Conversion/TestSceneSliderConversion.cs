using System.Collections.Generic;
using System.Linq;
using osu.Game.Audio;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Tau.Beatmaps;
using osu.Game.Rulesets.Tau.Objects;
using osuTK;

namespace osu.Game.Rulesets.Tau.Tests.Conversion
{
    public partial class TestSceneSliderConversion : TauConversionTestScene
    {
        protected override IEnumerable<HitObject> CreateHitObjects()
        {
            yield return new ConvertSlider
            {
                Duration = 1000,
                Position = new Vector2(0),
                Path = new SliderPath(PathType.Linear, new[] { new Vector2(0, 10), new Vector2(255, 10) }),
                NodeSamples = { new List<HitSampleInfo>() }
            };
        }

        protected override void CreateAsserts(IEnumerable<HitObject> hitObjects)
        {
            AddAssert("Hitobjects are of correct type", () => hitObjects.Any(o => o is Slider));

            var slider = hitObjects.FirstOrDefault() as Slider;

            AddAssert("Slider is not null", () => slider != null);
            AddAssert("Slider has correct duration", () => slider!.Duration == 1000);

            var original = CreateHitObjects().FirstOrDefault() as ConvertSlider;

            AddAssert("Slider has correct starting angle", () => slider!.Angle == original.CurvePositionAt(0).GetHitObjectAngle());
            AddAssert("Slider has correct ending angle", () => slider!.GetAbsoluteAngle(slider.Path.EndNode) == original.CurvePositionAt(1).GetHitObjectAngle());
        }

        private class ConvertSlider : HitObject, IHasPathWithRepeats, IHasPosition
        {
            public double EndTime => StartTime + Duration;

            public double Duration { get; set; }

            public double Distance => Path.Distance;
            public SliderPath Path { get; set; }
            public int RepeatCount { get; set; }

            public IList<IList<HitSampleInfo>> NodeSamples { get; } = new List<IList<HitSampleInfo>>();

            public float X => Position.X;
            public float Y => Position.Y;
            public Vector2 Position { get; set; }
        }
    }
}
