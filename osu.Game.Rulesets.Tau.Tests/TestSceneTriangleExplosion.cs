using osu.Framework.Graphics;
using osu.Framework.Testing;
using osu.Game.Rulesets.Tau.UI.Cursor;
using osuTK;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestTriangleExplosion : TestScene
    {
        public TestTriangleExplosion()
        {
            var explosion = new TriangleExplosion(20)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            };

            Add(explosion.With(e => { e.Scale = new Vector2(3); }));
            AddStep("Explode", () => explosion.Show());
        }
    }
}
