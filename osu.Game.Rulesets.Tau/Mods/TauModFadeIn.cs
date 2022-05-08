using osu.Framework.Graphics.Sprites;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModFadeIn : TauModHidden
    {
        public override IconUsage? Icon => TauIcons.ModFadeIn;
        protected override MaskingMode Mode => MaskingMode.FadeIn;
        protected override float InitialCoverage => 0.25f;
    }
}
