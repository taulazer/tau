using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Rulesets.Tau.Localisation;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModFadeOut : TauModHidden
    {
        public override IconUsage? Icon => OsuIcon.ModHidden;
        public override string Acronym => "FO";

        // Modification from osu!mania's description of Hidden mod.
        public override LocalisableString Description => ModStrings.FadeOutDescription;
        protected override MaskingMode Mode => MaskingMode.FadeOut;
        protected override float InitialCoverage => 0.4f;
    }
}
