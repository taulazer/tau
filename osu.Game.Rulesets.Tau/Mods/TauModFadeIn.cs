namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModFadeIn : TauModHidden
    {
        protected override MaskingMode Mode => MaskingMode.FadeIn;
        protected override float InitialCoverage => 0.3f;
    }
}
