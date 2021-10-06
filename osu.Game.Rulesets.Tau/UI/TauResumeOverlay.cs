using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Tau.UI.Cursor;
using osu.Game.Screens.Play;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.UI
{
    public class TauResumeOverlay : ResumeOverlay
    {
        private readonly float angleRange;
        private TauClickToResumeContainer clickContainer;
        private Container container;
        private TauCursor.AbsoluteCursor absoluteCursor;

        protected override string Message => "Move the cursor to the highlighted area.";

        public TauResumeOverlay(BeatmapDifficulty difficulty)
        {
            angleRange = (float)IBeatmapDifficultyInfo.DifficultyRange(difficulty.CircleSize, 75, 25, 10);
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Add(container = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(4),
                FillAspectRatio = 1, // 1:1
                FillMode = FillMode.Fit,
                Alpha = 0f,
                Children = new Drawable[]
                {
                    clickContainer = new TauClickToResumeContainer(angleRange)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Colour = Color4.Orange,
                        Current = new BindableDouble((angleRange / 360) * 0.25f),
                        ResumeRequested = Resume
                    }
                }
            });

            Add(absoluteCursor = new TauCursor.AbsoluteCursor
            {
                Alpha = 0
            });
        }

        protected override void PopIn()
        {
            base.PopIn();

            absoluteCursor.Show();
            clickContainer.Rotation = ScreenSpaceDrawQuad.Centre.GetDegreesFromPosition(ToScreenSpace(GameplayCursor.ActiveCursor.DrawPosition)) - ((angleRange * 0.25f) / 2);
            container.FadeIn(200);
        }

        protected override void PopOut()
        {
            base.PopOut();
            container.FadeOut(200);
            absoluteCursor.Hide();
        }

        private class TauClickToResumeContainer : CircularProgress, IKeyBindingHandler<TauAction>
        {
            public override bool Contains(Vector2 screenSpacePos) => CheckForValidation(ScreenSpaceDrawQuad.Centre.GetDegreesFromPosition(screenSpacePos) - 90);

            public bool CheckForValidation(float angle)
            {
                var rotation = Rotation + 270;

                if (rotation >= 360)
                    rotation -= 360;

                if (angle < rotation)
                    return false;

                var range = Rotation + 270 + (AngleRange * 0.25f);

                if (range >= 360)
                    range -= 360;

                if (angle > range)
                    return false;

                return true;
            }

            public Action ResumeRequested;
            public readonly float AngleRange;

            public TauClickToResumeContainer(float angleRange)
            {
                AngleRange = angleRange;
                RelativeSizeAxes = Axes.Both;

                Alpha = 0.25f;
            }

            protected override bool OnHover(HoverEvent e)
            {
                this.FadeTo(0.5f, 200);

                return base.OnHover(e);
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                this.FadeTo(0.25f, 200);

                base.OnHoverLost(e);
            }

            public bool OnPressed(KeyBindingPressEvent<TauAction> e)
            {
                switch (e.Action)
                {
                    case TauAction.LeftButton:
                    case TauAction.RightButton:
                        if (!IsHovered)
                            return false;

                        ResumeRequested?.Invoke();

                        return true;
                }

                return false;
            }

            public void OnReleased(KeyBindingReleaseEvent<TauAction> e)
            {
            }
        }
    }
}
