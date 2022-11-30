using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Tau.Tests.Mods
{
    public partial class TestSceneCinema : TestSceneOsuPlayer
    {
        protected override TestPlayer CreatePlayer(Ruleset ruleset)
        {
            SelectedMods.Value = new Mod[] { new TauModCinema() };

            return base.CreatePlayer(ruleset);
        }
    }
}
