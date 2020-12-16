using System;
using System.Collections.Generic;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace osu.Game.Rulesets.Tau.UI.Particles
{
    public class Particle : Box
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

        public Particle()
        {
            Velocity = Velocity;
        }

        protected override void Update()
        {
            base.Update();

            foreach (var vortex in Vortices)
            {
                var (dx, dy) = (X - vortex.DrawPosition.X, Y - vortex.DrawPosition.Y);

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