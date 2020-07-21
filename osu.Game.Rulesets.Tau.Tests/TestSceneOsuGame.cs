using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Platform;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestSceneOsuGame : OsuTestScene
    {
        [BackgroundDependencyLoader]
        private void load(GameHost host, OsuGameBase gameBase)
        {
            OsuGame game = new OsuGame();
            game.SetHost(host);

            Children = new Drawable[]
            {
                game
            };
        }
    }
}
