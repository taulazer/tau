using osu.Framework.Bindables;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;
using System;

namespace osu.Game.Rulesets.Tau.UI
{
    public class TauCachedProperties : IDisposable
    {
        public readonly BindableDouble AngleRange = new(25);
        public readonly BindableBool InverseModEnabled = new();
        public Texture SliderTexture;

        public void SetRange(float cs)
        {
            AngleRange.Value = IBeatmapDifficultyInfo.DifficultyRange(cs, 75, 25, 10);
        }

        public void Dispose()
        {
            SliderTexture?.Dispose();
            SliderTexture = null;
        }
    }
}
