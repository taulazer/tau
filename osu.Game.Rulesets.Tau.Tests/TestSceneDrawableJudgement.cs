using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Testing;
using osu.Game.Configuration;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestSceneDrawableJudgement : TauSkinnableTestScene
    {
        [Resolved]
        private OsuConfigManager config { get; set; }

        private readonly List<DrawablePool<TestDrawableTauJudgement>> pools;

        public TestSceneDrawableJudgement()
        {
            pools = new List<DrawablePool<TestDrawableTauJudgement>>();

            foreach (var result in Enum.GetValues(typeof(HitResult)).OfType<HitResult>().Skip(1))
            {
                showResult(result);
            }
        }

        [Test]
        public void TestHitLightingDisabled()
        {
            AddStep("hit Lighting disabled", () => config.SetValue(OsuSetting.HitLighting, false));

            showResult(HitResult.Great);

            AddUntilStep("judgements shown", () => this.ChildrenOfType<TestDrawableTauJudgement>().Any());
            AddAssert("hit Lighting has no transforms", () => this.ChildrenOfType<TestDrawableTauJudgement>().All(judgement => !judgement.Lighting.Transforms.Any()));
            AddAssert("hit Lighting hidden", () => this.ChildrenOfType<TestDrawableTauJudgement>().All(judgement => judgement.Lighting.Alpha == 0));
        }

        [Test]
        public void TestHitLightingEnabled()
        {
            AddStep("hit Lighting enabled", () => config.SetValue(OsuSetting.HitLighting, true));

            showResult(HitResult.Great);

            AddUntilStep("judgements shown", () => this.ChildrenOfType<TestDrawableTauJudgement>().Any());
            AddUntilStep("hit Lighting shown", () => this.ChildrenOfType<TestDrawableTauJudgement>().Any(judgement => judgement.Lighting.Alpha > 0));
        }

        private void showResult(HitResult result)
        {
            AddStep("Show " + result.GetDescription(), () =>
            {
                int poolIndex = 0;

                SetContents(_ =>
                {
                    DrawablePool<TestDrawableTauJudgement> pool;

                    if (poolIndex >= pools.Count)
                        pools.Add(pool = new DrawablePool<TestDrawableTauJudgement>(1));
                    else
                    {
                        pool = pools[poolIndex];

                        ((Container)pool.Parent).Clear(false);
                    }

                    var container = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        RelativePositionAxes = Axes.Y,
                        Y = Extensions.GetCircularPosition(-.6f, 0).Y,
                        Children = new Drawable[]
                        {
                            pool,
                            pool.Get(j => j.Apply(new JudgementResult(new HitObject
                            {
                                StartTime = Time.Current
                            }, new Judgement())
                            {
                                Type = result
                            }, null)).With(j =>
                            {
                                j.Anchor = Anchor.Centre;
                                j.Origin = Anchor.Centre;
                            })
                        }
                    };

                    poolIndex++;

                    return container;
                });
            });
        }

        private class TestDrawableTauJudgement : DrawableTauJudgement
        {
            public new SkinnableSprite Lighting => base.Lighting;
            public new SkinnableDrawable JudgementBody => base.JudgementBody;
        }
    }
}
