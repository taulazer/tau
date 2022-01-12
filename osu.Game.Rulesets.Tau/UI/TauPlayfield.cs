using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Tau.UI
{
    [Cached]
    public class TauPlayfield : Playfield
    {
        public static readonly Vector2 BASE_SIZE = new Vector2(768);

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.None;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Size = BASE_SIZE;

            AddRangeInternal(new Drawable[]
            {
                HitObjectContainer
            });
        }
    }
}
