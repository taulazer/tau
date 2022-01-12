using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;

namespace osu.Game.Rulesets.Tau.UI.Cursor
{
    public class CursorPiece : CompositeDrawable
    {
        public CursorPiece()
        {
            RelativeSizeAxes = Axes.Both;
            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;

            AddRangeInternal(new Drawable[]
            {
                createProgress(-150),
                createProgress(30)
            });
        }

        private CircularProgress createProgress(float rotation)
            => new CircularProgress
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Current = new BindableDouble(.33f),
                InnerRadius = 0.1f,
                Rotation = rotation
            };

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();
            Rotation += (float)Time.Elapsed / 5;
        }
    }
}
