using osu.Game.Tests.Beatmaps;
using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using NUnit.Framework;
using osu.Framework.Utils;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Objects;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Tau.Objects.Drawables;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TauBeatmapConversionTest : BeatmapConversionTest<ConvertValue>
    {
        protected override string ResourceAssembly => "osu.Game.Rulesets.Tau";

        [TestCase("ItIsTheEnd")]
        public new void Test(string name) => base.Test(name);

        protected override IEnumerable<ConvertValue> CreateConvertValue(HitObject hitObject)
        {
            switch (hitObject)
            {
                default:
                    yield return new ConvertValue((TauHitObject) hitObject);
                    break;
            }
        }
        protected override Ruleset CreateRuleset() => new TauRuleset();
    }

    public struct ConvertValue : IEquatable<ConvertValue>
    {
        /// <summary>
        /// A sane value to account for osu!stable using ints everwhere.
        /// </summary>
        private const float conversion_lenience = 2;

        [JsonIgnore]
        public readonly TauHitObject HitObject;

        public ConvertValue(TauHitObject hitObject)
        {
            HitObject = hitObject;
            startTime = 0;
            position = 0;
        }

        private double startTime;
        public double StartTime
        {
            get => HitObject?.StartTime ?? startTime;
            set => startTime = value;
        }

        private float position;
        public float Position
        {
            get
            {
                var v = (HitObject?.PositionToEnd.GetDegreesFromPosition(new DrawabletauHitObject(HitObject).Box.AnchorPosition) * 4);
                return v * (float) (Math.PI / 180) ?? position;
            }
            set => position = value;
        }
        public bool Equals(ConvertValue other)
            => Precision.AlmostEquals(StartTime, other.StartTime, conversion_lenience)
               && Precision.AlmostEquals(Position, other.Position, conversion_lenience);
    }
}
