using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;
using System;

namespace osu.Game.Rulesets.Tau.UI
{
    public class TauDependencyContainer : DependencyContainer, IDisposable
    {
        private TauCachedProperties cachedProperties { get; } = new();
        private IBeatmapDifficultyInfo difficultyInfo { get; }

        public TauDependencyContainer(IBeatmap beatmap, IReadOnlyDependencyContainer parent)
            : base(parent)
        {
            difficultyInfo = beatmap.Difficulty;
            cachedProperties.SetRange(difficultyInfo.CircleSize);

            CacheAs(difficultyInfo, new CacheInfo("tau_difficulty_info"));
            Cache(cachedProperties);
        }

        public void Dispose () {
            cachedProperties.Dispose();
        }
    }

    public class TauCachedProperties : IDisposable
    {
        public readonly BindableDouble AngleRange = new(25);
        public readonly BindableBool InverseModEnabled = new();
        public Texture SliderTexture;

        public void SetRange(float cs)
        {
            AngleRange.Value = IBeatmapDifficultyInfo.DifficultyRange(cs, 75, 25, 10);
        }

        public void Dispose () {
            SliderTexture?.Dispose();
            SliderTexture = null;
        }
    }
}
