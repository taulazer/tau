using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Pooling;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.UI.Particles;

namespace osu.Game.Rulesets.Tau.UI
{
    public class ParticleEmitter : Container
    {
        public List<Vortex> Vortices = new List<Vortex>();

        private readonly DrawablePool<Particle> particlePool;

        public ParticleEmitter()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Child = particlePool = new DrawablePool<Particle>(300);
        }

        public void AddParticle(float angle, HitResult? result = null)
        {
            Add(particlePool.Get(p => p.Apply(angle, result)));
        }
    }
}
