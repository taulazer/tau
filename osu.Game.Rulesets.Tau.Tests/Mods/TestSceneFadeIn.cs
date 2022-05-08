using System.Linq;
using NUnit.Framework;
using osu.Framework.Testing;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Tau.Tests.Mods
{
    public class TestSceneFadeIn : TestSceneOsuPlayer
    {
        protected override TestPlayer CreatePlayer(Ruleset ruleset)
        {
            SelectedMods.Value = new Mod[] { new TauModAutoplay(), new TauModFadeIn() };

            return base.CreatePlayer(ruleset);
        }

        protected override bool HasCustomSteps => true;

        [Test]
        public void TestFadeOutMod()
        {
            CreateTest(null);

            PlayfieldMaskingContainer pmc = null;

            AddAssert("Player has PMC", () =>
            {
                var playfield = Player.DrawableRuleset.Playfield;
                var pmcs = playfield.ChildrenOfType<PlayfieldMaskingContainer>();

                if (!pmcs.Any())
                    return false;

                pmc = pmcs.First();
                return true;
            });

            AddAssert("pmc is of correct mode", () => pmc is { Mode: MaskingMode.FadeIn });

            AddAssert("Positional effects are hidden", () =>
            {
                var playfield = (TauPlayfield)Player.DrawableRuleset.Playfield;

                return !playfield.ShouldShowPositionalEffects.Value;
            });
        }
    }
}
