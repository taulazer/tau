using osu.Game.Rulesets.Tau.UI;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModFadeIn : TauModHidden
    {
        public override string Name => "Fade in";
        public override string Acronym => "FI";
        public override string Description => @"Beats and sliders fade in";

        protected override MaskingMode Mode => MaskingMode.FadeIn;

        protected override float InitialCoverage => 0.3f;
    }
}
