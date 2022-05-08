using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osuTK;

namespace osu.Game.Rulesets.Tau.UI.Effects
{
    public class TurbulenceKiaiEffect : KiaiEffect<TurbulenceEmitter>
    {
        public List<Vortex> Vortices = new();

        [Resolved(canBeNull: true)]
        private TauCachedProperties properties { get; set; }

        [Resolved(canBeNull: true)]
        private TauPlayfield playfield { get; set; }

        [CanBeNull]
        private TauCursor cursor;

        public TurbulenceKiaiEffect(int initialSize)
            : base(initialSize)
        {
        }

        public TurbulenceKiaiEffect()
            : base(20)
        {
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            if (playfield == null)
                return;

            cursor = playfield.Cursor;

            // TODO: This is broken as shit.
            Vortices.Add(new Vortex
            {
                Position = new Vector2(0, -((TauPlayfield.BaseSize.X / 2) + 105)),
                Velocity = new Vector2(20, -20),
                Scale = 0.01f,
                Speed = 10f,
            });
        }

        protected override void Update()
        {
            base.Update();

            if (cursor == null)
                return;

            Vortices[0].Position = Extensions.FromPolarCoordinates((properties?.InverseModEnabled?.Value ?? false) ? 120 : 420, cursor.DrawablePaddle.Rotation);
            Vortices[0].Speed = cursor.AngleDistanceFromLastUpdate * 5;
        }
    }

    public class TurbulenceEmitter : Emitter
    {
        private TurbulenceKiaiEffect kiaiContainer => Parent as TurbulenceKiaiEffect;
        private List<Vortex> vortices => kiaiContainer.Vortices;

        [Resolved(canBeNull: true)]
        private IBeatmap beatmap { get; set; }

        protected override Drawable CreateAngularParticle()
            => new TriangleWithVelocity
            {
                Position = Extensions.FromPolarCoordinates(Distance, Settings.Angle)
            };

        protected override Drawable CreateCircularParticle()
            => new TriangleWithVelocity
            {
                Position = Extensions.FromPolarCoordinates(Distance, RNG.NextSingle() * 360f)
            };

        protected override void ApplyHitAnimation(Drawable drawable)
        {
            var particle = (TriangleWithVelocity)drawable;

            particle.RotateTo(RNG.NextSingle(-720, 720), Duration)
                    .ResizeTo(new Vector2(RNG.Next(0, 5)), Duration, Easing.OutQuint)
                    .FadeOut(Duration)
                    .Expire(true);

            particle.Velocity =
                Extensions.FromPolarCoordinates(Distance + (RNG.NextSingle(1, 5) * getVelocityAmplitude()),
                    Settings.IsCircular
                        ? Vector2.Zero.GetDegreesFromPosition(particle.Position)
                        : RNG.NextSingle(Settings.Angle - 10, Settings.Angle + 10));

            if (Settings.Inversed)
                particle.Velocity = -particle.Velocity;
        }

        private float getVelocityAmplitude()
        {
            if (beatmap == null)
                return 0.15f;

            var timingPoint = beatmap.ControlPointInfo.TimingPointAt(Time.Current);
            return Interpolation.ValueAt(Math.Max(200, timingPoint.BPM), 0.05f, 0.3f, 60f, 300f);
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

            private TurbulenceEmitter emitter => Parent as TurbulenceEmitter;
            private List<Vortex> vortices => emitter.vortices;

            protected override void Update()
            {
                base.Update();

                foreach (var vortex in vortices)
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
    }

    public class Vortex
    {
        public Vector2 Velocity { get; set; }
        public Vector2 Position { get; set; }
        public float Speed { get; set; }
        public float Scale { get; set; }
    }
}
