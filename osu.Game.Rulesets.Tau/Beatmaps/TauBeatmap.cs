using System.Collections.Generic;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Beatmaps
{
    public class TauBeatmap : Beatmap<TauHitObject>
    {
        public override IEnumerable<BeatmapStatistic> GetStatistics()
        {
            int beats = HitObjects.Count;

            return new[]
            {
                new BeatmapStatistic
                {
                    Name = "Beat count",
                    Content = beats.ToString(),
                    Icon = FontAwesome.Solid.Square
                }
            };
        }
    }
}
