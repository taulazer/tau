using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osuTK;

namespace osu.Game.Rulesets.Tau.UI.Effects
{
    public class ClassicKiaiEffect : KiaiEffect<ClassicEmitter>
    {
        public ClassicKiaiEffect(int initialSize)
            : base(initialSize)
        {
        }

        public ClassicKiaiEffect()
            : base(20)
        {
        }
    }

    public class ClassicEmitter : Emitter
    {
        protected override Drawable CreateAngularParticle()
            => new Triangle
            {
                Position = Extensions.FromPolarCoordinates(Distance, Settings.Angle)
            };

        protected override Drawable CreateCircularParticle()
            => new Triangle
            {
                Position = Extensions.FromPolarCoordinates(
                    (RNG.NextSingle() * 0.15f) * 0.15f + Distance,
                    RNG.NextSingle() * 360f)
            };

        protected override void ApplyHitAnimation(Drawable particle)
        {
            particle.RotateTo(RNG.NextSingle(-720, 720), Duration)
                    .FadeOut(Duration, Easing.OutQuint)
                    .Expire(true);

            if (Settings.IsCircular)
                addTransformsForCircular(particle);
            else
                addTransformsForAngled(particle);
        }

        private void addTransformsForAngled(Drawable particle)
        {
            particle.MoveTo(Extensions.FromPolarCoordinates(
                             (RNG.NextSingle() * 0.15f) * (Settings.Inversed ? -1f : 1f) + Distance,
                             RNG.NextSingle(Settings.Angle - 10, Settings.Angle + 10)),
                         Duration, Easing.OutQuint)
                    .ResizeTo(new Vector2(RNG.Next(0, 5)), Duration, Easing.OutQuint);
        }

        private void addTransformsForCircular(Drawable particle)
        {
            particle.MoveTo(
                         Extensions.FromPolarCoordinates(
                             (RNG.NextSingle() * 0.15f) * (Settings.Inversed ? -1f : 2f) + Distance,
                             Vector2.Zero.GetDegreesFromPosition(particle.Position)), Duration,
                         Easing.OutQuint)
                    .ScaleTo(new Vector2(RNG.Next(1, 2)), Duration, Easing.OutQuint);
        }
    }
}
