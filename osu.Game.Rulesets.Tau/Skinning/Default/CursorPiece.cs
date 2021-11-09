using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;

namespace osu.Game.Rulesets.Tau.Skinning.Default
{
    public class CursorPiece : CompositeDrawable
    {
        public CursorPiece()
        {
            RelativeSizeAxes = Axes.Both;
            InternalChildren = new Drawable[]
            {
                new CircularProgress
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Current = new BindableDouble(.33f),
                    InnerRadius = 0.1f,
                    Rotation = -150
                },
                new CircularProgress
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Current = new BindableDouble(.33f),
                    InnerRadius = 0.1f,
                    Rotation = 30
                }
            };
        }
    }
}
