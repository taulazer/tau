using osu.Framework.Graphics;
using osu.Framework.Testing;
using osu.Game.Rulesets.Tau.UI;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestSceneKiai : TestScene
    {
        public TestSceneKiai()
        {
            AddStep("Hit Single", () => testSingle());
            AddStep("Hit Stream", testMultiple);

            AddStep("Hit hard beat", () => Add(new KiaiHitExplosion(Color4.White, true)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(0.5f)
            }));
        }

        private void testSingle(float angle = 0)
        {
            Add(new KiaiHitExplosion(Color4.White)
            {
                Position = Extensions.GetCircularPosition(0.15f, angle),
                Angle = angle,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            });
        }

        private void testMultiple()
        {
            for (int i = 0; i <= 1000; i += 100)
            {
                var angle = i / 5f;
                Scheduler.AddDelayed(() => testSingle(angle), i);
            }
        }
    }
}
