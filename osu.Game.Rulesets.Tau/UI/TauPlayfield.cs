using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.UI;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.UI
{
    [Cached]
    public class TauPlayfield : Playfield
    {
        public static readonly Vector2 BASE_SIZE = new Vector2(768);
        public static readonly Bindable<Color4> ACCENT_COLOUR = new Bindable<Color4>(Color4Extensions.FromHex(@"FF0040"));

        protected override GameplayCursorContainer CreateCursor() => new TauCursor();

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.None;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Size = BASE_SIZE;

            AddRangeInternal(new Drawable[]
            {
                new PlayfieldPiece(),
                HitObjectContainer
            });
        }

        private class PlayfieldPiece : CompositeDrawable
        {
            private readonly Bindable<float> playfieldDimLevel = new Bindable<float>(0.8f);

            public PlayfieldPiece()
            {
                Box background;
                RelativeSizeAxes = Axes.Both;

                AddInternal(new CircularContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    BorderThickness = 3,
                    BorderColour = ACCENT_COLOUR.Value,
                    Child = background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Black,
                        Alpha = playfieldDimLevel.Value,
                        AlwaysPresent = true
                    }
                });

                playfieldDimLevel.BindValueChanged(v => { background.FadeTo(v.NewValue, 100); }, true);
            }
        }
    }
}
