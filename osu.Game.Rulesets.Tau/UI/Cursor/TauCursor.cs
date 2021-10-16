using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Tau.UI.Cursor
{
    public class TauCursor : GameplayCursorContainer
    {
        public float AngleRange { get; }

        public readonly Paddle PaddleDrawable;

        protected override Drawable CreateCursor() => new AbsoluteCursor();

        public TauCursor(BeatmapDifficulty difficulty)
        {
            AngleRange = (float)IBeatmapDifficultyInfo.DifficultyRange(difficulty.CircleSize, 75, 25, 20);

            FillAspectRatio = 1; // 1:1
            FillMode = FillMode.Fit;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            Alpha = 0;

            Add(PaddleDrawable = new Paddle(AngleRange));

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
            PaddleDrawable.Rotation = ScreenSpaceDrawQuad.Centre.GetDegreesFromPosition(e.ScreenSpaceMousePosition);
            ActiveCursor.Position = ToLocalSpace(e.ScreenSpaceMousePosition);

            return Handle(e);
        }

        public override void Show()
        {
            this.FadeIn(250);
            PaddleDrawable.Show();
        }

        public override void Hide()
        {
            PaddleDrawable.Hide();

            using (BeginDelayedSequence(250))
            {
                this.FadeOut(250);
            }
        }
    }
}
