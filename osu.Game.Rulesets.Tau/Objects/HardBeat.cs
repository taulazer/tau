using System.Threading;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Tau.Objects
{
    public class HardBeat : TauHitObject
    {
        protected override void CreateNestedHitObjects(CancellationToken cancellationToken)
        {
            AddNested(new HardBeatNestedHitObject(this)
            {
                StartTime = this.GetEndTime(),
                Samples = Samples
            });
        }
    }
}
