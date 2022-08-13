using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Tau.Localisation;
using osu.Game.Rulesets.Tau.Objects;
using osuTK;

namespace osu.Game.Rulesets.Tau.Beatmaps
{
    public class TauBeatmap : Beatmap<TauHitObject>
    {
        public override IEnumerable<BeatmapStatistic> GetStatistics()
        {
            int beats = HitObjects.Count(c => c is Beat);
            int sliders = HitObjects.Count(s => s is Slider);
            int hardBeats = HitObjects.Count(hb => hb is HardBeat);

            return new[]
            {
                new BeatmapStatistic
                {
                    Name = BeatmapStrings.BeatCount,
                    Content = beats.ToString(),
                    CreateIcon = () => new SpriteIcon
                    {
                        Icon = FontAwesome.Solid.Square,
                        Scale = new Vector2(.7f)
                    },
                },
                new BeatmapStatistic
                {
                    Name = BeatmapStrings.SliderCount,
                    Content = sliders.ToString(),
                    CreateIcon = () => new BeatmapStatisticIcon(BeatmapStatisticsIconType.Sliders)
                },
                new BeatmapStatistic
                {
                    Name = BeatmapStrings.HardBeatCount,
                    Content = hardBeats.ToString(),
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
