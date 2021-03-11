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
        private Circle circle;
        private Vortex vortex;

        [BackgroundDependencyLoader]
        private void load()
        {
            Add(new Box
            {
                Colour = Color4Extensions.FromHSV(345, 0.2f, 0.1f),
                RelativeSizeAxes = Axes.Both
            });

            vortex = new Vortex
            {
                Speed = 5,
                Position = new Vector2(0.1f, 0),
                Scale = new Vector2(20)
            };

            var emitter = new ParticleEmitter
            {
                RelativeSizeAxes = Axes.Both,
                Vortices = new List<Vortex>
                {
                    vortex
                }
            };

            Add(emitter);

            Add(circle = new Circle
            {
                Position = emitter.Vortices[0].Position,
                Size = emitter.Vortices[0].Scale,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0.25f
            });

            AddSliderStep("Position X", -100f, 100f, 1f, p => { vortex.Position.X = p; });
            AddSliderStep("Position Y", -100f, 100f, 1f, p => { vortex.Position.Y = p; });
            AddSliderStep("Speed", -10, 10, 1f, p => { vortex.Speed = p; });
            AddSliderStep("Scale", -10, 10, 1f, p => { vortex.Scale.X = p; });
            AddSliderStep("Velocity X", -100f, 100f, 0f, p => { vortex.Velocity.X = p; });
            AddSliderStep("Velocity Y", -100f, 100f, 0f, p => { vortex.Velocity.Y = p; });

            Scheduler.AddDelayed(() =>
            {
                emitter.Add(new Particle
                {
                    Size = new Vector2(2),
                    Velocity = new Vector2
                    {
                        X = (RNG.NextSingle(0, 3f) - 1.5f) * 80,
                        Y = -(RNG.NextSingle(0, 5) + (8 * 20))
                    }
                });
            }, 5, true);
        }

        protected override void Update()
        {
            circle.Position = vortex.Position;
            circle.Size = new Vector2(vortex.Scale.X * 5);

            base.Update();
        }
    }
}
