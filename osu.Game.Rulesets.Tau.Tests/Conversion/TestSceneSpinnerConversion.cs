using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Tests.Conversion
{
    public partial class TestSceneSpinnerConversion : TauConversionTestScene
    {
        protected override IEnumerable<HitObject> CreateHitObjects()
        {
            yield return new DurationHitObject { Duration = 7500 };
        }

        protected override void CreateAsserts(IEnumerable<HitObject> hitObjects)
        {
            AddAssert("Hitobjects are of correct type", () => hitObjects.Any(o => o is Slider));

            var slider = hitObjects.FirstOrDefault() as Slider;

            AddAssert("Slider is not null", () => slider != null);
            AddAssert("Slider has correct duration", () => slider!.Duration == 7500);
        }

        private class DurationHitObject : HitObject, IHasDuration
        {
            public double EndTime => StartTime + Duration;
            public double Duration { get; set; }
        }
    }
}
