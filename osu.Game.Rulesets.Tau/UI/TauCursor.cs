using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Tau.UI.Cursor;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Tau.UI
{
    public class TauCursor : GameplayCursorContainer
    {
        public readonly Paddle DrawablePaddle;

        protected override Drawable CreateCursor() => new AbsoluteCursor();

        public TauCursor()
        {
            FillAspectRatio = 1;
            FillMode = FillMode.Fit;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            Add(DrawablePaddle = new Paddle());

            State.Value = Visibility.Hidden;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            using (BeginDelayedSequence(50))
                Show();
        }

        protected override bool OnMouseMove(MouseMoveEvent e)
        {
            DrawablePaddle.Rotation = ScreenSpaceDrawQuad.Centre.GetDegreesFromPosition(e.ScreenSpaceMousePosition);
            ActiveCursor.Position = ToLocalSpace(e.ScreenSpaceMousePosition);

            return false;
        }

        public override void Show()
        {
            this.FadeIn(250);
            DrawablePaddle.Show();
        }

        public override void Hide()
        {
            DrawablePaddle.Hide();

            using (BeginDelayedSequence(250))
            {
                this.FadeOut(250);
            }
        }
    }
}
