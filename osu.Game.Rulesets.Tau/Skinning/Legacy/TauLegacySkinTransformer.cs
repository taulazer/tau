using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Screens.Ranking.Statistics;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Tau.Skinning.Legacy
{
    public class TauLegacySkinTransformer : LegacySkinTransformer
    {
        public TauLegacySkinTransformer(ISkinSource source)
            : base(source)
        {
        }

        public override Drawable GetDrawableComponent(ISkinComponent component)
        {
            if (!(component is TauSkinComponent tauComponent))
                return null;

            switch (tauComponent.Component)
            {
                case TauSkinComponents.Beat:
                    return Source.GetTexture("beat") != null ? new LegacyBeat() : null;

                case TauSkinComponents.HardBeat:
                    return Source.GetTexture("hard-beat") != null ? new LegacyHardBeat() : null;

                case TauSkinComponents.Playfield:
                    return Source.GetTexture("field-overlay") != null ? new LegacyPlayfield() : null;
            }

            return null;
        }

        public override IBindable<TValue> GetConfig<TLookup, TValue>(TLookup lookup)
        {
            switch (lookup)
            {
                case TauSkinColour colour:
                    return Source.GetConfig<SkinCustomColourLookup, TValue>(new SkinCustomColourLookup(colour));
            }

            return Source.GetConfig<TLookup, TValue>(lookup);
        }
    }
}
