using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Tau.Objects;
using osuTK;

namespace osu.Game.Rulesets.Tau.Tests.Conversion
{
    public partial class TestSceneBeatConversion : TauConversionTestScene
    {
        protected override IEnumerable<HitObject> CreateHitObjects()
        {
            yield return new AngledHitObject { StartTime = 0, Angle = 90 };
            yield return new PositionalHitObject { StartTime = 500, Position = new Vector2(257, 192) };
            yield return new XPositionalHitObject { StartTime = 1000, X = 128 };
            yield return new YPositionalHitObject { StartTime = 1500, Y = 96 };
        }

        protected override void CreateAsserts(IEnumerable<HitObject> hitObjects)
        {
            AddAssert("Objects are of correct type", () => hitObjects.All(o => o is Beat));
            var beats = hitObjects.Cast<Beat>();
            AddAssert("Beats are all correct angle", () => beats.All(b => b.Angle == 90));
        }

        private class AngledHitObject : HitObject, IHasAngle
        {
            public float Angle { get; set; }
        }

        private class PositionalHitObject : HitObject, IHasPosition
        {
            public float X => Position.X;
            public float Y => Position.Y;
            public Vector2 Position { get; set; }
        }

        private class XPositionalHitObject : HitObject, IHasXPosition
        {
            public float X { get; set; }
        }

        private class YPositionalHitObject : HitObject, IHasYPosition
        {
            public float Y { get; set; }
        }
    }
}
