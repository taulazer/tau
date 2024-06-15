using osu.Framework.Bindables;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;
using System;

namespace osu.Game.Rulesets.Tau.UI
{
    /// <summary>
    /// Cached properties for use during gameplay.
    /// </summary>
    public class TauCachedProperties : IDisposable
    {
        public readonly BindableDouble AngleRange = new(25);
        public readonly BindableBool InverseModEnabled = new();
        public Texture SliderTexture;
        public Texture VisuallyDistinctSliderTexture;

        /// <summary>
        /// Sets the range for the paddle.
        /// </summary>
        /// <param name="cs">The Circle Size of the beatmap.</param>
        public void SetRange(float cs)
        {
            AngleRange.Value = IBeatmapDifficultyInfo.DifficultyRange(cs, 75, 25, 10);
        }

        public void Dispose() {
            SliderTexture?.Dispose();
            VisuallyDistinctSliderTexture?.Dispose();
            SliderTexture = VisuallyDistinctSliderTexture = null;
        }
    }
}
