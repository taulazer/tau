using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Tau.Skinning.Default;
using osu.Game.Skinning;
using osuTK;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace osu.Game.Rulesets.Tau.UI.Cursor
{
    public class TauCursor : CompositeDrawable
    {
        private readonly IBindable<WorkingBeatmap> beatmap = new Bindable<WorkingBeatmap>();
        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;

        private readonly float angleRange;

        public readonly Paddle PaddleDrawable;

        public TauCursor(BeatmapDifficulty difficulty)
        {
            angleRange = (float)BeatmapDifficulty.DifficultyRange(difficulty.CircleSize, 75, 25, 10);

            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;

            RelativeSizeAxes = Axes.Both;
            AddInternal(PaddleDrawable = new Paddle(angleRange));
            AddInternal(new AbsoluteCursor());
        }

        [BackgroundDependencyLoader]
        private void load(IBindable<WorkingBeatmap> beatmap)
        {
            this.beatmap.BindTo(beatmap);
        }

        public bool CheckForValidation(float angle)
        {
            var angleDiff = Extensions.GetDeltaAngle(PaddleDrawable.Rotation, angle);

            return Math.Abs(angleDiff) <= angleRange / 2;
        }

        protected override bool OnMouseMove(MouseMoveEvent e)
        {
            PaddleDrawable.Rotation = ScreenSpaceDrawQuad.Centre.GetDegreesFromPosition(e.ScreenSpaceMousePosition);

            return base.OnMouseMove(e);
        }

        public class Paddle : CompositeDrawable
        {
            public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;
            private readonly CircularContainer border;
            private readonly CircularProgress paddle;

            public readonly PaddleGlow Glow;

            public Paddle(float angleRange)
            {
                RelativeSizeAxes = Axes.Both;
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;
                FillMode = FillMode.Fit;
                FillAspectRatio = 1; // 1:1 Aspect Ratio.

                InternalChildren = new Drawable[]
                {
                    border = new CircularContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        //Masking = true,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Children = new Drawable[]
                        {
                            Glow = new PaddleGlow(angleRange)
                            {
                                Alpha = 0
                            },
                            paddle = new CircularProgress
                            {
                                RelativeSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Current = new BindableDouble(angleRange / 360),
                                InnerRadius = 0.05f,
                                Rotation = -angleRange / 2,
                            },
                            new SkinnableDrawable(new TauSkinComponent(TauSkinComponents.Handle), _ => new HandlePiece(), ConfineMode.ScaleToFit)
                        }
                    }
                };

                border.Colour = TauPlayfield.ACCENT_COLOR.Value;
            }

            [BackgroundDependencyLoader]
            private void load(ISkinSource skin)
            {
                Texture texture;

                if ((texture = skin.GetTexture("paddle")) != null)
                    paddle.Texture = texture;
            }
        }

        public class AbsoluteCursor : CursorContainer
        {
            public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;

            protected override Drawable CreateCursor() => new CircularContainer
            {
                Size = new Vector2(40),
                Origin = Anchor.Centre,
                Children = new Drawable[]
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
                    },
                }
            };

            protected override void UpdateAfterChildren()
            {
                base.UpdateAfterChildren();
                ActiveCursor.Rotation += (float)Time.Elapsed / 5;
            }
        }

        public class PaddleGlow : CompositeDrawable
        {
            public PaddleGlow(float angleRange)
            {
                const int width = 128;

                var image = new Image<Rgba32>(width, width);

                var gradientTextureBoth = new Texture(width, width, true);

                for (int i = 0; i < width; ++i)
                {
                    for (int j = 0; j < width; ++j)
                    {
                        float brightness = (float)i / (width - 1);
                        float brightness2 = (float)j / (width - 1);

                        image[i, j] = new Rgba32(
                            255,
                            255,
                            255,
                            (byte)(255 - (1 + brightness2 - brightness) / 2 * 255));
                    }
                }

                gradientTextureBoth.SetData(new TextureUpload(image));

                RelativeSizeAxes = Axes.Both;
                Size = new Vector2(1.5f);
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;

                InternalChildren = new Drawable[]
                {
                    Line = new CircularProgress
                    {
                        RelativeSizeAxes = Axes.Both,
                        Size = new Vector2(.68f),
                        InnerRadius = 0.01f,
                        Rotation = -5f,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Current = new BindableDouble(8f / 360),
                    },
                    Glow = new CircularProgress
                    {
                        RelativeSizeAxes = Axes.Both,
                        InnerRadius = 0.325f,
                        Rotation = -5f,
                        Texture = gradientTextureBoth,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Current = new BindableDouble(8f / 360),
                    }
                };
            }

            public CircularProgress Line;
            public CircularProgress Glow;
        }
    }
}
