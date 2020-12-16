using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.UI
{
    public class TauHitObjectLifetimeEntry : HitObjectLifetimeEntry
    {
        protected override double InitialLifetimeOffset => ((TauHitObject)HitObject).TimePreempt;

        public TauHitObjectLifetimeEntry(HitObject hitObject)
            : base(hitObject)
        {
        }
    }
}
