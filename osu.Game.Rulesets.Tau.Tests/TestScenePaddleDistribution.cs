using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Tests.Beatmaps;
using osu.Game.Tests.Visual;
using osuTK;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestScenePaddleDistribution : OsuTestScene
    {
        [Test]
        public void TestManyDistributionEvents()
        {
            createTest(CreateDistributedHitEvents());
        }

        [Test]
        public void TestAroundCentre()
        {
            var beatmap = new TestBeatmap(new TauRuleset().RulesetInfo);
            var angleRange = (float)BeatmapDifficulty.DifficultyRange(beatmap.BeatmapInfo.BaseDifficulty.CircleSize, 75, 25, 10);
            createTest(Enumerable.Range(0, (int)angleRange).Select(i => new HitEvent(i / 50f, HitResult.Perfect, new Beat(), new Beat(), new Vector2((i - (angleRange / 2)), 0))).ToList());
        }

        [Test]
        public void TestNoEvents()
        {
            createTest(new List<HitEvent>());
        }

        private void createTest(List<HitEvent> events) => AddStep("create test", () =>
        {
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4Extensions.FromHex("#333")
                },
                new PaddleDistributionGraph(events, new TestBeatmap(new TauRuleset().RulesetInfo))
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = new Vector2(600, 250)
                }
            };
        });

        public static List<HitEvent> CreateDistributedHitEvents()
        {
            var hitEvents = new List<HitEvent>();
            var beatmap = new TestBeatmap(new TauRuleset().RulesetInfo);
            var angleRange = (float)BeatmapDifficulty.DifficultyRange(beatmap.BeatmapInfo.BaseDifficulty.CircleSize, 75, 25, 10);

            for (int i = 0; i < 100; i++)
            {
                hitEvents.Add(new HitEvent(i - 25, HitResult.Perfect, new Beat(), new Beat(), new Vector2(RNG.NextSingle(-(angleRange / 2), angleRange / 2), 0)));
            }

            return hitEvents;
        }
    }
}
