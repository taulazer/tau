using NUnit.Framework;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Tau.Tests.Mods
{
    public class TestSceneDual : TestSceneOsuPlayer
    {
        protected override TestPlayer CreatePlayer(Ruleset ruleset)
        {
            SelectedMods.Value = new Mod[] { new TauModAutoplay(), new TauModDual() };
            return base.CreatePlayer(ruleset);
        }

        protected override bool HasCustomSteps => true;

        private TauPlayfield playfield;
        private TauCursor cursor;

        [Test]
        public void TestTraceableMod()
        {
            CreateTest();

            AddStep("fetch playfield", () => { playfield = Player.DrawableRuleset.Playfield as TauPlayfield; });
            AddAssert("playfield is not null", () => playfield != null);
            AddStep("fetch cursor", () => cursor = playfield.Cursor);
            AddAssert("cursor has additional paddles", () => cursor.AdditionalPaddles != null);
            AddAssert("cursor amount is correct", () => cursor.AdditionalPaddles!.Count == 1);
        }
    }
}
