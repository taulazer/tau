using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Tau.UI.Particles;

namespace osu.Game.Rulesets.Tau.UI
{
    public class ParticleEmitter : Container
    {
        public List<Vortex> Vortices = new List<Vortex>();

        protected override void LoadComplete()
        {
            base.LoadComplete();

            foreach (var vortex in Vortices)
            {
                Add(vortex.With(v =>
                {
                    v.AlwaysPresent = true;
                }));
            }
        }
    }
}
