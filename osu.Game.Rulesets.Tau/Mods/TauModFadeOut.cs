namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModFadeOut : TauModHidden
    {
        protected override MaskingMode Mode => MaskingMode.FadeOut;
        protected override float InitialCoverage => 0.4f;
    }
}
