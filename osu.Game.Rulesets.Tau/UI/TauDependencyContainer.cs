using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.Tau.UI
{
    public class TauDependencyContainer : DependencyContainer
    {
        public TauCachedProperties CachedProperties { get; } = new();
        public IBeatmapDifficultyInfo DifficultyInfo { get; }

        public TauDependencyContainer(IBeatmap beatmap, IReadOnlyDependencyContainer parent)
            : base(parent)
        {
            DifficultyInfo = beatmap.Difficulty;
            CachedProperties.SetRange(DifficultyInfo.CircleSize);

            CacheAs(DifficultyInfo, new CacheInfo("tau_difficulty_info"));
            Cache(CachedProperties);
        }
    }

    public class TauCachedProperties
    {
        public readonly BindableDouble AngleRange = new();

        public void SetRange(float cs)
        {
            AngleRange.Value = IBeatmapDifficultyInfo.DifficultyRange(cs, 75, 25, 10);
        }
    }
}
