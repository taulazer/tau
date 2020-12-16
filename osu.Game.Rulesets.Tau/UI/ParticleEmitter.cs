using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Tau.UI.Particles;

namespace osu.Game.Rulesets.Tau.UI
{
    public class ParticleEmitter : Container
    {
        public List<Vortex> Vortices = new List<Vortex>();

        public ParticleEmitter()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }
    }
}
