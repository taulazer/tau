using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Tau.UI;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestSceneKiai : TauSkinnableTestScene
    {
        public TestSceneKiai()
        {
            AddStep("Hit Single", () => SetContents(() => testSingle()));
            AddStep("Hit Stream", () => SetContents(testMultiple));

            AddStep("Hit hard beat", () => SetContents(() => new Container
            {
                RelativeSizeAxes = Axes.Both,
                FillAspectRatio = 1,
                FillMode = FillMode.Fit,
                Children = new Drawable[]
                {
                    new KiaiHitExplosion(Color4.White, true)
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(0.5f)
                    }
                }
            }));
        }

        private Drawable testSingle(float angle = 0)
        {
            return new Container
            {
                RelativeSizeAxes = Axes.Both,
                FillAspectRatio = 1,
                FillMode = FillMode.Fit,
                Children = new Drawable[]
                {
                    new KiaiHitExplosion(Color4.White)
                    {
                        Position = Extensions.GetCircularPosition(0.15f, angle),
                        Angle = angle,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    }
                }
            };
        }

        private Drawable testMultiple()
        {
            var container = new Container
            {
                RelativeSizeAxes = Axes.Both,
                FillAspectRatio = 1,
                FillMode = FillMode.Fit,
            };

            for (int i = 0; i <= 1000; i += 100)
            {
                var angle = i / 5f;
                // TODO: Return to original test behavior
                container.Add(testSingle(angle));
            }

            return container;
        }
    }
}
