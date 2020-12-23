using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Timing;
using osu.Framework.Utils;
using osu.Game.Rulesets.Tau.Skinning;
using osu.Game.Skinning;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.UI
{
    public class KiaiHitExplosion : CompositeDrawable
    {
        public override bool RemoveWhenNotAlive => true;
        private readonly bool circular;
        private readonly Color4 colour;
        private readonly int particleAmount;

        /// <summary>
        /// Used whenever circular isn't set to True.
        /// </summary>
        public float Angle;

        public KiaiHitExplosion(Color4 colour, bool circular = false, int particleAmount = 10)
        {
            this.colour = colour;
            this.circular = circular;
            this.particleAmount = particleAmount;

            RelativePositionAxes = Axes.Both;
            RelativeSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load(ISkinSource skin)
        {
            var animated = skin.GetConfig<TauSkinConfiguration, bool>(TauSkinConfiguration.AnimateKiai)?.Value ?? false;

            if (animated)
            {
                var container = new Container
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Child = skin.GetAnimation("kiai" + (circular ? "-big" : string.Empty), true, false, true).With(a =>
                    {
                        a.FillMode = FillMode.Fit;
                        a.RelativeSizeAxes = Axes.Both;
                    }),
                    Clock = new FramedClock()
                };

                if (circular)
                    container.RelativeSizeAxes = Axes.Both;
                else
                    container.Size = new Vector2(RNG.NextSingle(25, 25));

                container.Delay(1500).Expire();

                AddInternal(container);
            }
            else
                AddInternal(new KiaiHitExplosionEmitter(colour, circular, particleAmount) { Angle = Angle });
        }

        private class KiaiHitExplosionEmitter : CompositeDrawable
        {
            public override bool RemoveWhenNotAlive => true;
            private readonly List<Drawable> particles;
            private readonly bool circular;
            private readonly Color4 colour;
            private readonly int particleAmount;
            public float Angle;

            public KiaiHitExplosionEmitter(Color4 colour, bool circular, int particleAmount)
            {
                this.colour = colour;
                this.circular = circular;
                this.particleAmount = particleAmount;
                particles = new List<Drawable>();

                RelativePositionAxes = Axes.Both;
                RelativeSizeAxes = Axes.Both;
            }

            [BackgroundDependencyLoader]
            private void load(ISkinSource skin)
            {
                var rng = new Random();
                Texture kiai;
                Texture kiaiBig;

                if ((kiai = skin.GetTexture("kiai")) == null)
                    kiai = Texture.WhitePixel;

                if ((kiaiBig = skin.GetTexture("kiai-big")) == null)
                    kiaiBig = Texture.WhitePixel;

                if (circular)
                {
                    for (int i = 0; i < particleAmount; i++)
                    {
                        particles.Add(new Sprite
                        {
                            RelativePositionAxes = Axes.Both,
                            Position = Extensions.GetCircularPosition(((float)(rng.NextDouble() * 0.15f) * 0.15f) + 0.5f, (float)rng.NextDouble() * 360f),
                            Rotation = (float)rng.NextDouble() * 360f,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.BottomCentre,
                            Size = new Vector2(rng.Next(1, 15)),
                            Colour = colour,
                            Texture = kiaiBig
                        });
                    }
                }
                else
                {
                    for (int i = 0; i < particleAmount; i++)
                    {
                        particles.Add(new Sprite
                        {
                            RelativePositionAxes = Axes.Both,
                            Position = Extensions.GetCircularPosition((float)(rng.NextDouble() * 0.15f) * 0.15f, ((float)rng.NextDouble() / 10 * 10) + (Angle - 20)),
                            Rotation = (float)rng.NextDouble() * 360f,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.BottomCentre,
                            Size = new Vector2(rng.Next(1, 15)),
                            Colour = colour,
                            Texture = kiai
                        });
                    }
                }

                AddRangeInternal(particles.ToArray());
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                foreach (var particle in particles)
                {
                    const double duration = 1500;
                    var rng = new Random();

                    if (circular)
                    {
                        particle.MoveTo(Extensions.GetCircularPosition(((float)(rng.NextDouble() * 0.15f) * 2f) + 0.5f, Vector2.Zero.GetDegreesFromPosition(particle.Position)), duration,
                                     Easing.OutQuint)
                                .ScaleTo(new Vector2(rng.Next(1, 2)), duration, Easing.OutQuint)
                                .FadeOut(duration, Easing.OutQuint);
                    }
                    else
                    {
                        particle.MoveTo(Extensions.GetCircularPosition((float)(rng.NextDouble() * 0.15f) * 1f, randomBetween(Angle - 40, Angle + 40)), duration, Easing.OutQuint)
                                .ResizeTo(new Vector2(rng.Next(0, 5)), duration, Easing.OutQuint)
                                .FadeOut(duration, Easing.OutQuint);

                        float randomBetween(float smallNumber, float bigNumber)
                        {
                            float diff = bigNumber - smallNumber;

                            return ((float)rng.NextDouble() * diff) + smallNumber;
                        }
                    }

                    particle.Expire(true);
                    this.Delay(duration).Expire(true);
                }
            }
        }
    }
}
