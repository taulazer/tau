using osu.Framework.Graphics.Sprites;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModFadeOut : TauModHidden
    {
        public override IconUsage? Icon => TauIcons.ModFadeOut;
        public override string Acronym => "FO";

        // Modification from osu!mania's description of Hidden mod.
        public override string Description => "Beats fade out before you hit them!";
        protected override MaskingMode Mode => MaskingMode.FadeOut;
        protected override float InitialCoverage => 0.4f;
    }
}
