using System.Collections.Generic;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osu.Game.Graphics;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.UI.Cursor;
using osuTK;

namespace osu.Game.Rulesets.Tau.UI.Effects
{
    public class TurbulenceKiaiContainer : DrawablePool<TurbulenceEmitter>, INeedsNewResult
    {
        public List<Vortex> Vortices = new();

        public TurbulenceKiaiContainer()
            : base(20, 50)
        {
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }

        [Resolved]
        private OsuColour colour { get; set; }

        [Resolved(canBeNull: true)]
        private TauCachedProperties properties { get; set; }

        [Resolved(canBeNull: true)]
        private TauPlayfield playfield { get; set; }

        [CanBeNull]
        private TauCursor cursor;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            if (playfield != null)
            {
                cursor = playfield.Cursor;

                // TODO: This should probably not be here.
                Vortices.Add(new Vortex
                {
                    Position = new Vector2(0, -((TauPlayfield.BaseSize.X / 2) + 105)),
                    Velocity = new Vector2(20, -20),
                    Scale = 0.01f,
                    Speed = 10f,
                });
            }
        }

        protected override void Update()
        {
            base.Update();

            if (cursor == null)
                return;

            Vortices[0].Position = Extensions.GetCircularPosition((properties?.InverseModEnabled?.Value ?? false) ? 120 : 420, cursor.DrawablePaddle.Rotation);
            Vortices[0].Speed = cursor.AngleDistanceFromLastUpdate * 5;
        }

        public void OnNewResult(DrawableHitObject judgedObject, JudgementResult result)
        {
            var emitter = judgedObject.HitObject switch
            {
                IHasAngle angle => getEmitterForAngle(angle, result),
                HardBeat => getEmitterForHardBeat(result),
                _ => new TurbulenceEmitter()
            };

            AddInternal(emitter);
            emitter.GoOff();
        }

        private TurbulenceEmitter getEmitterForAngle(IHasAngle angle, JudgementResult result)
            => Get(e => e.Apply(new TurbulenceEmitter.TurbulenceEmitterSettings
            {
                Amount = 10,
                Angle = angle.Angle,
                Inversed = properties?.InverseModEnabled?.Value ?? false,
                Colour = colour.ForHitResult(result.Type)
            }));

        private TurbulenceEmitter getEmitterForHardBeat(JudgementResult result)
            => Get(e => e.Apply(new TurbulenceEmitter.TurbulenceEmitterSettings
            {
                Amount = 64,
                IsCircular = true,
                Inversed = properties?.InverseModEnabled?.Value ?? false,
                Colour = colour.ForHitResult(result.Type)
            }));
    }

    public class TurbulenceEmitter : PoolableDrawable
    {
        private readonly List<TriangleWithVelocity> particles = new();
        private TurbulenceEmitterSettings settings;

        public TurbulenceKiaiContainer KiaiContainer => Parent as TurbulenceKiaiContainer;
        public List<Vortex> Vortices => KiaiContainer.Vortices;

        public TurbulenceEmitter()
        {
            // RelativePositionAxes = Axes.Both;
            RelativeSizeAxes = Axes.Both;
            Blending = BlendingParameters.Additive;
        }

        public void Apply(TurbulenceEmitterSettings settings)
        {
            this.settings = settings;
            particles.Clear();

            for (int i = 0; i < settings.Amount; i++)
            {
                particles.Add(createProperties(this.settings.IsCircular
                                                   ? createCircularParticle()
                                                   : createAngularParticle()));
            }
        }

        private float distance => settings.Inversed ? 0.5f - Paddle.PADDLE_RADIUS : 0.5f;

        private TriangleWithVelocity createAngularParticle()
            => new()
            {
                Position = Extensions.GetCircularPosition(distance, settings.Angle)
            };

        private TriangleWithVelocity createCircularParticle()
            => new()
            {
                Position = Extensions.GetCircularPosition(distance, RNG.NextSingle() * 360f)
            };

        private TriangleWithVelocity createProperties(TriangleWithVelocity drawable)
            => drawable.With(d =>
            {
                d.RelativePositionAxes = Axes.Both;
                d.Anchor = Anchor.Centre;
                d.Origin = Anchor.BottomCentre;

                d.Colour = settings.Colour;
                d.Rotation = (float)RNG.NextDouble() * 360f;
                d.Size = new Vector2(RNG.Next(5, 15));
                d.Alpha = RNG.NextSingle(0.25f, 1f);
            });

        private const double duration = 1500;

        public void GoOff()
        {
            AddRangeInternal(particles);

            foreach (var particle in particles)
            {
                particle.RotateTo(RNG.NextSingle(-720, 720), duration)
                        .ResizeTo(new Vector2(RNG.Next(0, 5)), duration, Easing.OutQuint)
                        .FadeOut(duration)
                        .Expire(true);

                particle.Velocity =
                    Extensions.GetCircularPosition(distance + (RNG.NextSingle(1, 5) * 0.15f),
                        settings.IsCircular
                            ? Vector2.Zero.GetDegreesFromPosition(particle.Position)
                            : Extensions.RandomBetween(settings.Angle - 10, settings.Angle + 10));

                if (settings.Inversed)
                    particle.Velocity = -particle.Velocity;
            }

            this.Delay(duration).Expire(true);
        }

        private class TriangleWithVelocity : Triangle
        {
            public Vector2 Velocity
            {
                get => staticVelocity;
                set => velocity = staticVelocity = value;
            }

            private Vector2 staticVelocity;
            private Vector2 velocity;

            public TurbulenceEmitter Emitter => Parent as TurbulenceEmitter;
            public List<Vortex> Vortices => Emitter.Vortices;

            protected override void Update()
            {
                base.Update();

                foreach (var vortex in Vortices)
                {
                    var distance = new Vector2(DrawPosition.X - vortex.Position.X, DrawPosition.Y - vortex.Position.Y);

                    var speed = vortex.Speed;
                    var (vx, vy) = (distance.Y * (speed + vortex.Velocity.X), distance.X * (speed + vortex.Velocity.Y));

                    double factor = 1 / (1 + ((distance.X * distance.X) + (distance.Y * distance.Y)) / vortex.Scale);
                    const float f = 0.5f;
                    factor *= (1 - f) * f * 4;

                    velocity.X += (float)((vx - velocity.X) * factor);
                    velocity.Y += (float)((vy - velocity.Y) * factor);
                }

                const float damping = 1 - 0.004f;
                velocity *= new Vector2(damping);

                var deltaTime = (float)(Time.Elapsed * 0.001);
                Position += new Vector2(velocity.X * deltaTime, velocity.Y * deltaTime);
            }
        }

        public struct TurbulenceEmitterSettings
        {
            public float Angle { get; set; }
            public int Amount { get; set; }
            public Colour4 Colour { get; set; }
            public bool IsCircular { get; set; }
            public bool Inversed { get; set; }
        }
    }

    public class Vortex
    {
        public Vector2 Velocity { get; set; }
        public Vector2 Position { get; set; }
        public float Speed { get; set; }
        public float Scale { get; set; }
    }
}
