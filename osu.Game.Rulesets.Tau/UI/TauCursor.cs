using System.Collections.Generic;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Rulesets.Tau.UI.Cursor;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Tau.UI
{
    public partial class TauCursor : GameplayCursorContainer
    {
        protected override Drawable CreateCursor() => new AbsoluteCursor();

        private readonly BindableDouble angleRange = new BindableDouble(TauDefaults.PADDLE_ANGLE);
        public IBindable<double> AngleRange => angleRange;

        private readonly List<Paddle> paddles;
        public IReadOnlyList<Paddle> Paddles => paddles;

        protected IReadOnlyList<Mod> Mods = [];

        public TauCursor()
        {
            FillAspectRatio = 1f / 1f;
            FillMode = FillMode.Fill;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            // Rotate 90° as the getAngleFromPosition method expects a 0° -> 360° clockwise range starting from the 3 o'clock position.
            Rotation = 90;

            paddles = [new Paddle(angleRange)];
            Add(paddles[0]);
        }

        [BackgroundDependencyLoader(true)]
        private void load([CanBeNull] IReadOnlyList<Mod> mods)
        {
            if (mods is null)
                return;

            Mods = mods;

            if (mods.GetMod(out TauModDual dual))
            {
                for (int i = 1; i < dual.PaddleCount.Value; i++)
                {
                    var paddle = new Paddle(angleRange);
                    paddles.Add(paddle);
                    Add(paddle);
                }

                for (int i = 0; i < paddles.Count; i++)
                    paddles[i].Rotation = 360f / paddles.Count * i;
            }
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            using (BeginDelayedSequence(50))
                Show();
        }

        protected override bool OnMouseMove(MouseMoveEvent e)
        {
            Rotation = Extensions.GetAngleFromPosition(ScreenSpaceDrawQuad.Centre, e.ScreenSpaceMousePosition) + 90;
            ActiveCursor.Position = ToLocalSpace(e.ScreenSpaceMousePosition);

            return false;
        }

        public Paddle.AngleValidationResult ValidateAngle(float angle)
        {
            Paddle.AngleValidationResult result = default;

            foreach (var paddle in paddles)
            {
                result = paddle.ValidateAngle(Rotation, angle);

                if (result.IsValid)
                    return result;
            }

            return result;
        }

        public void SetAngleRange(float circleSize)
        {
            angleRange.Value = IBeatmapDifficultyInfo.DifficultyRange(circleSize, 75, 25, 15);
        }

        public override void Show()
        {
            base.Show();
            this.FadeIn(250);
        }
    }
}
