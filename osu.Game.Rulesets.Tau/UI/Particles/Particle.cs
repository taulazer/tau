using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osu.Game.Graphics;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Tau.UI.Particles
{
    public class Particle : PoolableDrawable
    {
        public Vector2 Velocity
        {
            get => staticVelocity;
            set => velocity = staticVelocity = value;
        }

        private Vector2 staticVelocity;
        private Vector2 velocity;

        public ParticleEmitter Emitter => Parent as ParticleEmitter;
        public List<Vortex> Vortices => Emitter.Vortices;

        public override bool RemoveCompletedTransforms => false;

        [Resolved]
        private OsuColour colour { get; set; }

        public Particle()
        {
            Blending = BlendingParameters.Additive;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            InternalChild = new Box
            {
                RelativeSizeAxes = Axes.Both
            };
        }

        public void Apply(float angle, HitResult? result = null)
        {
            Position = Extensions.GetCircularPosition(RNG.NextSingle(380, 400), angle);
            Velocity = Extensions.GetCircularPosition(RNG.NextSingle(380, 400), RNG.NextSingle(angle - 40, angle + 40));
            Size = new Vector2(RNG.NextSingle(1, 3));
            Rotation = RNG.NextSingle(0, 360);
            Colour = result.HasValue ? colour.ForHitResult(result.Value) : TauPlayfield.ACCENT_COLOR;
        }

        protected override void PrepareForUse()
        {
            base.PrepareForUse();

            ApplyTransformsAt(double.MinValue, true);
            ClearTransforms();

            this.FadeOut(1500).Expire(true);
        }

        protected override void Update()
        {
            base.Update();

            foreach (var vortex in Vortices)
            {
                var (dx, dy) = (X - vortex.Position.X, Y - vortex.Position.Y);

                var speed = vortex.Speed;
                var (vx, vy) = (dy * speed + vortex.Velocity.X, dx * speed + vortex.Velocity.Y);

                double factor = 1 / (1 + (dx * dx + dy * dy) / vortex.Scale.X);
                const float f = 0.5f;
                factor *= (1 - f) * f * 4;

                velocity.X += (float)((vx - velocity.X) * factor);
                velocity.Y += (float)((vy - velocity.Y) * factor);
            }

            const float damping = 1 - 0.001f;
            velocity.X *= damping;
            velocity.Y *= damping;

            var deltaTime = (float)TimeSpan.FromMilliseconds(Time.Elapsed).TotalSeconds;
            X += velocity.X * deltaTime;
            Y += velocity.Y * deltaTime;
        }
    }
}
