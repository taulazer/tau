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
                return base.GetDrawableComponent(component);

            return tauComponent.Component switch
            {
                TauSkinComponents.Beat => Skin.GetTexture($"{TauRuleset.SHORT_NAME}-beat") != null ? new LegacyBeat() : null,
                TauSkinComponents.HardBeat => Skin.GetTexture($"{TauRuleset.SHORT_NAME}-hard-beat") != null ? new LegacyHardBeat() : null,
                TauSkinComponents.Handle => Skin.GetTexture($"{TauRuleset.SHORT_NAME}-handle") != null ? new LegacyHandle() : null,
                TauSkinComponents.Ring => Skin.GetTexture($"{TauRuleset.SHORT_NAME}-ring-overlay") != null ? new LegacyPlayfield() : null,
                TauSkinComponents.Cursor => Skin.GetTexture($"{TauRuleset.SHORT_NAME}-cursor") != null ? new LegacyCursor() : null,
                _ => null
            };
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
