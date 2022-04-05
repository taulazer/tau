using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.Tau.UI
{
    public class TauDependencyContainer : DependencyContainer
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
    }

    public class TauCachedProperties
    {
        public readonly BindableDouble AngleRange = new(25);
        public readonly BindableBool InverseModEnabled = new();

        public void SetRange(float cs)
        {
            AngleRange.Value = IBeatmapDifficultyInfo.DifficultyRange(cs, 75, 25, 10);
        }
    }
}
