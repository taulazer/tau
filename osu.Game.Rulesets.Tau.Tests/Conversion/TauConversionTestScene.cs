using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Tau.Tests.Conversion
{
    public abstract partial class TauConversionTestScene : TauModTestScene
    {
        protected abstract IEnumerable<HitObject> CreateHitObjects();

        protected abstract void CreateAsserts(IEnumerable<HitObject> hitObjects);

        protected virtual bool PassCondition(IEnumerable<HitObject> hitObjects)
            => Player.Results.Count(r => r.IsHit) >= hitObjects.Count();

        [Test]
        public void TestConversion()
        {
            var hitObjects = CreateHitObjects();

            CreateModTest(new ModTestData
            {
                Autoplay = true,
                Beatmap = new Beatmap
                {
                    HitObjects = hitObjects.ToList()
                },
                PassCondition = () => PassCondition(hitObjects)
            });

            AddStep("Create converts asserts", () =>
            {
                var beatmap = CreateBeatmap(Ruleset.Value);
                var converter = Ruleset.Value.CreateInstance().CreateBeatmapConverter(beatmap);
                var converted = converter.Convert();

                CreateAsserts(converted.HitObjects);
            });
        }
    }
}
