using osu.Framework.Localisation;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Localisation;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModTraceable : Mod, IApplicableToDrawableRuleset<TauHitObject>
    {
        public override string Name => "Traceable";
        public override string Acronym => "TC";
        public override ModType Type => ModType.Fun;
        public override LocalisableString Description => ModStrings.TraceableDescription;
        public override double ScoreMultiplier => 1;

        public void ApplyToDrawableRuleset(DrawableRuleset<TauHitObject> drawableRuleset)
        {
            if ((drawableRuleset as TauDrawableRuleset)?.Playfield is TauPlayfield playfield)
                playfield.PlayfieldPiece.Alpha = 0;
        }
    }
}
