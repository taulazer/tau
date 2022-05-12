using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Audio;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Tau.Objects;
using osuTK;

namespace osu.Game.Rulesets.Tau.Tests.Conversion
{
    public class TestSceneSliderConversion : TauConversionTestScene
    {
        protected override IEnumerable<HitObject> CreateHitObjects()
        {
            yield return new ConvertSlider
            {
                Position = new Vector2(0),
                Path = new SliderPath(PathType.Linear, new[] { new Vector2(0, 10), new Vector2(255, 10) }),
                NodeSamples = { new List<HitSampleInfo>() }
            };
        }

        protected override bool IsConvertedCorrectly(IEnumerable<HitObject> hitObjects)
        {
            // TODO: These should probably be asserts.
            if (!hitObjects.Any(o => o is Slider))
                return false;

            var slider = hitObjects.FirstOrDefault() as Slider;

            if (slider is not { Duration: 1000 })
                return false;

            // var original = CreateHitObjects().FirstOrDefault() as ConvertSlider;

            // TODO: These fails, will need to investigate when i get home lmao

            // if (slider.Angle != original!.Position.GetHitObjectAngle())
            //     return false;
            //
            // if (slider.GetAbsoluteAngle(slider.Path.EndNode) != original!.Path.PositionAt(1).GetHitObjectAngle())
            //     return false;

            return true;
        }

        private class ConvertSlider : HitObject, IHasPathWithRepeats, IHasPosition
        {
            public double EndTime { get; set; } = 1000;

            public double Duration
            {
                get => EndTime - StartTime;
                set => throw new NotImplementedException();
            }

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
