using System.Collections.Generic;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Statistics;
using osu.Game.Tests.Beatmaps;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestScenePaddleDistribution : TauTestScene
    {
        private float angleRange => (float)Properties.AngleRange.Value;

        [Test]
        public void TestManyDistributionEvents()
        {
            var beatmap = new TestBeatmap(new TauRuleset().RulesetInfo);
            Properties.SetRange(beatmap.Difficulty.CircleSize);

            createTest(createDistributedHitEvents());
        }

        [Test]
        public void TestToCenter()
        {
            var beatmap = new TestBeatmap(new TauRuleset().RulesetInfo);
            Properties.SetRange(beatmap.Difficulty.CircleSize);

            var events = new List<HitEvent>();

            for (int i = 0; i < angleRange / 2; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    events.Add(new HitEvent(i, HitResult.Great, new Beat(), new Beat(),
                        new Vector2(i - (angleRange / 2), 0)));
                    events.Add(new HitEvent(i, HitResult.Great, new Beat(), new Beat(),
                        new Vector2((angleRange / 2) - i, 0)));
                }
            }

            createTest(events);
        }

        [Test]
        public void TestToEdges()
        {
            var beatmap = new TestBeatmap(new TauRuleset().RulesetInfo);
            Properties.SetRange(beatmap.Difficulty.CircleSize);

            var events = new List<HitEvent>();

            for (int i = 0; i < angleRange / 2; i++)
            {
                for (float j = angleRange / 2; j > i; j--)
                {
                    events.Add(new HitEvent(i, HitResult.Great, new Beat(), new Beat(),
                        new Vector2(i - (angleRange / 2), 0)));
                    events.Add(new HitEvent(i, HitResult.Great, new Beat(), new Beat(),
                        new Vector2((angleRange / 2) - i, 0)));
                }
            }

            createTest(events);
        }

        [Test]
        public void TestNoEvents()
        {
            createTest(new List<HitEvent>());
        }

        private void createTest(IReadOnlyList<HitEvent> events) => AddStep("create test", () =>
        {
            Children = new Drawable[]
            {
                new Box
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.X,
                    Height = 250,
                    Colour = Color4.Red,
                    Alpha = 0.25f
                },
                new PaddleDistributionGraph(events, new TestBeatmap(new TauRuleset().RulesetInfo))
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.X,
                    Height = 250,
                }
            };
        });

        private List<HitEvent> createDistributedHitEvents()
        {
            var hitEvents = new List<HitEvent>();

            for (int i = 0; i < 100; i++)
                hitEvents.Add(new HitEvent(i - 25, HitResult.Perfect, new Beat(), new Beat(),
                    new Vector2(RNG.NextSingle(-(angleRange / 2), angleRange / 2), 0)));

            return hitEvents;
        }
    }
}
