using osu.Framework.Allocation;
using osu.Framework.Testing;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Tau.Tests
{
    [ExcludeFromDynamicCompile]
    public abstract partial class TauTestScene : OsuTestScene
    {
        [Cached]
        private TauCachedProperties properties { get; set; } = new();

        protected TauCachedProperties Properties => properties;

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            properties.Dispose();
        }

        protected override Ruleset CreateRuleset()
            => new TauRuleset();
    }
}
