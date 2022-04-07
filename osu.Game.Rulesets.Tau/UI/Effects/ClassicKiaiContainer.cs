using System.Collections.Generic;
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
    public class ClassicKiaiContainer : DrawablePool<KiaiExplosionEmitter>, INeedsNewResult
    {
        public ClassicKiaiContainer()
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

        public void OnNewResult(DrawableHitObject judgedObject, JudgementResult result)
        {
            switch (judgedObject.HitObject)
            {
                case IHasAngle angle:
                    onAngledHit(angle, result);
                    break;

                case HardBeat:
                    onHardHit(result);
                    break;
            }
        }

        private void onAngledHit(IHasAngle angle, JudgementResult result)
        {
            var emitter = Get(e => e.Apply(new KiaiExplosionSettings
            {
                Amount = 10,
                Angle = angle.Angle,
                Inversed = properties?.InverseModEnabled?.Value ?? false,
                Colour = colour.ForHitResult(result.Type)
            }));

            AddInternal(emitter);
            emitter.GoOff();
        }

        private void onHardHit(JudgementResult result)
        {
            var emitter = Get(e => e.Apply(new KiaiExplosionSettings
            {
                Amount = 64,
                IsCircular = true,
                Inversed = properties?.InverseModEnabled?.Value ?? false,
                Colour = colour.ForHitResult(result.Type)
            }));

            AddInternal(emitter);
            emitter.GoOff();
        }
    }

    public class KiaiExplosionEmitter : PoolableDrawable
    {
        private readonly List<Drawable> particles = new();
        private KiaiExplosionSettings settings;

        public KiaiExplosionEmitter()
        {
            RelativePositionAxes = Axes.Both;
            RelativeSizeAxes = Axes.Both;
            Blending = BlendingParameters.Additive;
        }

        public void Apply(KiaiExplosionSettings settings)
        {
            this.settings = settings;

            particles.Clear();

            for (var i = 0; i < settings.Amount; i++)
            {
                if (this.settings.IsCircular)
                    createCircularParticle();
                else
                    createAngularParticle();
            }
        }

        private float distance => settings.Inversed ? 0.5f - Paddle.PADDLE_RADIUS : 0.5f;

        private void createAngularParticle()
        {
            var drawable = new Triangle
            {
                Position = Extensions.GetCircularPosition(distance, settings.Angle)
            };

            particles.Add(createProperties(drawable));
        }

        private void createCircularParticle()
        {
            var drawable = new Triangle
            {
                Position = Extensions.GetCircularPosition(
                    (RNG.NextSingle() * 0.15f) * 0.15f + distance,
                    RNG.NextSingle() * 360f)
            };

            particles.Add(createProperties(drawable));
        }

        private Drawable createProperties(Drawable drawable)
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
                        .FadeOut(duration, Easing.OutQuint)
                        .Expire(true);

                if (settings.IsCircular)
                    addTransformsForCircular(particle);
                else
                    addTransformsForAngled(particle);
            }

            this.Delay(duration).Expire(true);
        }

        private void addTransformsForAngled(Drawable particle)
        {
            particle.MoveTo(Extensions.GetCircularPosition(
                             (RNG.NextSingle() * 0.15f) * (settings.Inversed ? -1f : 1f) + distance,
                             Extensions.RandomBetween(settings.Angle - 10, settings.Angle + 10)),
                         duration, Easing.OutQuint)
                    .ResizeTo(new Vector2(RNG.Next(0, 5)), duration, Easing.OutQuint);
        }

        private void addTransformsForCircular(Drawable particle)
        {
            particle.MoveTo(
                         Extensions.GetCircularPosition(
                             (RNG.NextSingle() * 0.15f) * (settings.Inversed ? -1f : 2f) + distance,
                             Vector2.Zero.GetDegreesFromPosition(particle.Position)), duration,
                         Easing.OutQuint)
                    .ScaleTo(new Vector2(RNG.Next(1, 2)), duration, Easing.OutQuint);
        }
    }

    public struct KiaiExplosionSettings
    {
        public float Angle { get; set; }
        public int Amount { get; set; }
        public Colour4 Colour { get; set; }
        public bool IsCircular { get; set; }
        public bool Inversed { get; set; }
    }
}
