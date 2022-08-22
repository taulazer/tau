using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Tau.Localisation;
using osu.Game.Screens.Play;
using osuTK;
using osuTK.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace osu.Game.Rulesets.Tau.UI
{
    public class TauResumeOverlay : ResumeOverlay
    {
        [Resolved]
        private TauCachedProperties properties { get; set; }

        private double angleRange => properties.AngleRange.Value;

        private TauClickToResumeContainer clickContainer;
        private Container container;
        private TauCursor cursor;

        protected override LocalisableString Message => UiStrings.ResumeMessage;

        [BackgroundDependencyLoader]
        private void load()
        {
            Add(container = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(4f),
                FillAspectRatio = 1,
                FillMode = FillMode.Fit,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Black,
                        Alpha = 0.25f
                    },
                    clickContainer = new TauClickToResumeContainer()
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Current = new BindableDouble((angleRange / 360) * 0.25f),
                        ResumeRequested = Resume
                    }
                }
            });

            Add(cursor = new TauCursor());
        }

        protected override void PopIn()
        {
            base.PopIn();

            GameplayCursor.ActiveCursor.Hide();
            cursor.Show();
            clickContainer.Rotation = ScreenSpaceDrawQuad.Centre.GetDegreesFromPosition(ToScreenSpace(GameplayCursor.ActiveCursor.DrawPosition)) -
                                      (((float)angleRange * 0.25f) / 2);
            container.FadeIn(200);
        }

        protected override void PopOut()
        {
            base.PopOut();
            container.FadeOut(200);
            cursor.Hide();
            GameplayCursor?.ActiveCursor?.Show();
        }

        private class TauClickToResumeContainer : CircularProgress, IKeyBindingHandler<TauAction>
        {
            public override bool Contains(Vector2 screenSpacePos)
                => checkForValidation(ScreenSpaceDrawQuad.Centre.GetDegreesFromPosition(screenSpacePos) - 90);

            private bool checkForValidation(float angle)
            {
                var rotation = Rotation - 90;
                rotation.NormalizeAngle();

                var range = ((float)angleRange / 2) * 0.25;
                var delta = Math.Abs(Extensions.GetDeltaAngle((float)(rotation + (range)), angle));

                return !(delta > range);
            }

            [Resolved]
            private TauCachedProperties properties { get; set; }

            private double angleRange => properties.AngleRange.Value;

            public Action ResumeRequested;

            public TauClickToResumeContainer()
            {
                RelativeSizeAxes = Axes.Both;
                Colour = Color4Extensions.FromHex(@"FF0040");
            }

            [BackgroundDependencyLoader]
            private void load(IRenderer renderer)
            {
                Texture = generateTexture(renderer, 0.25f);
            }

            private Texture generateTexture(IRenderer renderer, float opacity)
            {
                const int width = 128;

                var image = new Image<Rgba32>(width, 1);
                var gradientTextureHorizontal = renderer.CreateTexture(1, width, true);

                image.ProcessPixelRows(rows =>
                {
                    var row = rows.GetRowSpan(0);

                    for (int i = 0; i < width; i++)
                    {
                        var alpha = (float)i / (width - 1);
                        alpha = 1 - alpha;
                        alpha *= opacity;
                        alpha += 0.025f;
                        row[i] = new Rgba32(1f, 1f, 1f, alpha);
                    }
                });

                gradientTextureHorizontal.SetData(new TextureUpload(image));

                return gradientTextureHorizontal;
            }

            protected override bool OnHover(HoverEvent e)
            {
                this.FadeColour(Color4Extensions.FromHex(@"00FFAA"), 200);

                return base.OnHover(e);
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                this.FadeColour(Color4Extensions.FromHex(@"FF0040"), 200);

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
