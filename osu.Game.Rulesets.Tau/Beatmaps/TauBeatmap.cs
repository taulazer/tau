using System.Collections.Generic;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Tau.Objects;
using System.Linq;
using osuTK;

namespace osu.Game.Rulesets.Tau.Beatmaps
{
    public class TauBeatmap : Beatmap<TauHitObject>
    {
        public override IEnumerable<BeatmapStatistic> GetStatistics()
        {
            int beats = HitObjects.Count(b => b is Beat);
            int sliders = HitObjects.Count(b => b is Slider);
            int hardbeats = HitObjects.Count(b => b is HardBeat);

            return new[]
            {
                new BeatmapStatistic
                {
                    Name = "Beat count",
                    Content = beats.ToString(),
                    CreateIcon = () => new SpriteIcon
                    {
                        Icon = FontAwesome.Solid.Square,
                        Scale = new Vector2(.7f)
                    },
                },
                new BeatmapStatistic
                {
                    Name = "Slider count",
                    Content = sliders.ToString(),
                    CreateIcon = () => new BeatmapStatisticIcon(BeatmapStatisticsIconType.Sliders)
                },
                new BeatmapStatistic
                {
                    Name = "HardBeat count",
                    Content = hardbeats.ToString(),
                    CreateIcon = () => new SpriteIcon
                    {
                        Icon = FontAwesome.Regular.Circle,
                        Scale = new Vector2(.7f)
                    },
                }
            };
        }
    }
}
