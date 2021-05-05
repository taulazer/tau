using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Bindings;
using osu.Game.Beatmaps;
using osu.Game.Screens.Play;
using osuTK;
using osuTK.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace osu.Game.Rulesets.Tau.UI
{
    public class TauResumeOverlay : ResumeOverlay
    {
        private readonly float angleRange;

        protected override string Message => "Move the paddle to the highlighted area.";

        public TauResumeOverlay(BeatmapDifficulty difficulty)
        {
            angleRange = (float)BeatmapDifficulty.DifficultyRange(difficulty.CircleSize, 75, 25, 10);
        }

        [BackgroundDependencyLoader]
        private void load()
        {
        }

        protected override void PopIn()
        {
            base.PopIn();

            TauClickToResumeContainer t;

            Add(new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.63f),
                FillAspectRatio = 1, // 1:1
                FillMode = FillMode.Fit,
                Children = new Drawable[]
                {
                    t = new TauClickToResumeContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Texture = createTexture(),
                        Colour = Color4.Orange,
                        InnerRadius = 0.4f,
                        Current =
                        {
                            Value = .5f
                        }
                    }
                }
            });
        }

        private Texture createTexture()
        {
            const int width = 128;
            const int border_thickness = 15;

            var image = new Image<Rgba32>(width, width);

            var gradientTextureBoth = new Texture(width, width, true);

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    byte brightness = (byte)(0.25f * 255); // 25%

                    if (j < border_thickness || j > (width - border_thickness))
                        brightness = 255;

                    image[i, j] = new Rgba32(
                        255,
                        255,
                        255,
                        brightness);
                }
            }

            gradientTextureBoth.SetData(new TextureUpload(image));

            return gradientTextureBoth;
        }

        private class TauClickToResumeContainer : CircularProgress, IKeyBindingHandler<TauAction>
        {
            public override bool HandlePositionalInput => true;

            public Action ResumeRequested;

            public TauClickToResumeContainer()
            {
                RelativeSizeAxes = Axes.Both;
            }

            public bool OnPressed(TauAction action)
            {
                switch (action)
                {
                    case TauAction.LeftButton:
                    case TauAction.RightButton:

                        ResumeRequested?.Invoke();

                        return true;
                }

                return false;
            }

            public void OnReleased(TauAction action)
            {
            }
        }
    }
}
