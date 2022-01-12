using System.Collections.Generic;
using System.Linq;
using System.Threading;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Beatmaps
{
    public class TauBeatmapConverter : BeatmapConverter<TauHitObject>
    {
        public override bool CanConvert() => Beatmap.HitObjects.All(h => h is IHasPosition || h is IHasXPosition || h is IHasYPosition);

        public TauBeatmapConverter(Ruleset ruleset, IBeatmap beatmap)
            : base(beatmap, ruleset)
        {
        }

        protected override IEnumerable<TauHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap, CancellationToken cancellationToken)
        {
            float angle = original switch
            {
                IHasPosition pos => pos.Position.GetHitObjectAngle(),
                IHasXPosition xPos => xPos.X.Remap(0, 512, 0, 360),
                IHasYPosition yPos => yPos.Y.Remap(0, 384, 0, 360),
                _ => 0
            };

            yield return new Beat
            {
                Samples = original.Samples,
                StartTime = original.StartTime,
                Angle = angle
            };
        }
    }
}

