using osu.Framework.Graphics.Sprites;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModFadeOut : TauModHidden
    {
        public override IconUsage? Icon => TauIcons.ModFadeOut;
        protected override MaskingMode Mode => MaskingMode.FadeOut;
        protected override float InitialCoverage => 0.4f;
    }
}
