using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Testing;
using osu.Framework.Utils;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.Tau.UI.Particles;
using osuTK;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestSceneParticleSystem : TestScene
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Add(new Box
            {
                Colour = Color4Extensions.FromHSV(345, 0.2f, 0.1f),
                RelativeSizeAxes = Axes.Both
            });

            var emitter = new ParticleEmitter
            {
                RelativeSizeAxes = Axes.Both,
                Vortices = new List<Vortex>
                {
                    new Vortex
                    {
                        Speed = 5,
                        Position = new Vector2(0,200)
                    }
                }
            };

            Add(emitter);

            for (int i = 0; i < 1000; i++)
            {
                Scheduler.AddDelayed(() =>
                {
                    emitter.Add(new Particle
                    {
                        Size = new Vector2(2),
                        Velocity = new Vector2
                        {
                            X = (RNG.NextSingle(0, 3f) - 1.5f) * 50,
                            Y = -(RNG.NextSingle(0, 2) + 4 * 20)
                        }
                    });
                }, i * 5);
            }
        }
    }
}
