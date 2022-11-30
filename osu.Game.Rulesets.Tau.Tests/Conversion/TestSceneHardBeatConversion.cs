using System.Collections.Generic;
using System.Linq;
using osu.Game.Audio;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Tests.Conversion
{
    public partial class TestSceneHardBeatConversion : TauConversionTestScene
    {
        protected override IEnumerable<HitObject> CreateHitObjects()
        {
            yield return new HitObject { Samples = { new HitSampleInfo(HitSampleInfo.HIT_FINISH) } };
        }

        protected override void CreateAsserts(IEnumerable<HitObject> hitObjects)
        {
            AddAssert("Hitobjects are of correct type", () => hitObjects.Any(o => o is HardBeat));
        }
    }
}
