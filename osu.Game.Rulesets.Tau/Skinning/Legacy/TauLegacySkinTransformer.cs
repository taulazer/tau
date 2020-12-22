using System;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Tau.Skinning.Legacy
{
    public class TauLegacySkinTransformer : LegacySkinTransformer
    {
        private Lazy<bool> hasBeat;

        public TauLegacySkinTransformer(ISkinSource source)
            : base(source)
        {
            source.SourceChanged += sourceChanged;
            sourceChanged();
        }

        private void sourceChanged()
        {
            hasBeat = new Lazy<bool>(() => Source.GetTexture("beat") != null);
        }

        public override Drawable GetDrawableComponent(ISkinComponent component)
        {
            if (!(component is TauSkinComponent tauComponent))
                return null;

            switch (tauComponent.Component)
            {
                case TauSkinComponents.Beat:
                    return hasBeat.Value ? new LegacyBeat() : null;
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
