using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;

namespace osu.Game.Rulesets.Tau.UI.Cursor
{
    public partial class CursorPiece : CompositeDrawable
    {
        private const float progress_amount = 1f / 3f;

        public CursorPiece()
        {
            RelativeSizeAxes = Axes.Both;
            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;

            InternalChildren =
            [
                createProgress().With(static c => c.Rotation = -150),
                createProgress().With(static c => c.Rotation = 30)
            ];
        }

        private static CircularProgress createProgress()
        {
            return new CircularProgress()
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Progress = progress_amount,
                InnerRadius = 0.1f,
            };
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();
            Rotation += (float)Time.Elapsed / 5;
        }
    }
}
