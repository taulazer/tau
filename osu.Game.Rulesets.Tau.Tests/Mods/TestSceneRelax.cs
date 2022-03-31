using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Tau.Tests.Mods
{
    public class TestSceneRelax : TestSceneOsuPlayer
    {
        protected override TestPlayer CreatePlayer(Ruleset ruleset)
        {
            SelectedMods.Value = new Mod[] { new TauModRelax() };

            return base.CreatePlayer(ruleset);
        }
    }
}
