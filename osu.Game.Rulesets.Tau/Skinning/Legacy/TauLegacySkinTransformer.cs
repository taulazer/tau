using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Tau.Skinning.Legacy
{
    public class TauLegacySkinTransformer : LegacySkinTransformer
    {
        public TauLegacySkinTransformer(ISkin source)
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
                    return Skin.GetTexture("beat") != null ? new LegacyBeat() : null;

                case TauSkinComponents.HardBeat:
                    return Skin.GetTexture("hard-beat") != null ? new LegacyHardBeat() : null;

                case TauSkinComponents.Handle:
                    return Skin.GetTexture("handle") != null ? new LegacyHandle() : null;

                case TauSkinComponents.Ring:
                    return Skin.GetTexture("ring-overlay") != null ? new LegacyPlayfield() : null;
            }

            return null;
        }

        public override IBindable<TValue> GetConfig<TLookup, TValue>(TLookup lookup)
        {
            switch (lookup)
            {
                case TauSkinColour colour:
                    return Skin.GetConfig<SkinCustomColourLookup, TValue>(new SkinCustomColourLookup(colour));
            }

            return Skin.GetConfig<TLookup, TValue>(lookup);
        }
    }
}
