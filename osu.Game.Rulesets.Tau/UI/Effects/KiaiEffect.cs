using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Utils;
using osu.Game.Graphics;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.UI.Cursor;
using osuTK;

namespace osu.Game.Rulesets.Tau.UI.Effects
{
    public abstract class KiaiEffect<T> : DrawablePool<T>, INeedsNewResult, IFollowsSlider
        where T : Emitter, new()
    {
        protected KiaiEffect()
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
            var emitter = judgedObject.HitObject switch
            {
                IHasAngle angle => GetEmitterForAngle(angle, result),
                HardBeat => GetEmitterForHardBeat(result),
                _ => new T()
            };

            AddInternal(emitter);
            emitter.GoOff();
        }

        public void UpdateSliderPosition(float angle)
        {
            var emitter = Get(e => e.Apply(new Emitter.EmitterSettings
            {
                Amount = 2,
                Angle = angle,
                Inversed = properties?.InverseModEnabled?.Value ?? false,
                Colour = TauPlayfield.AccentColour.Value
            }));

            AddInternal(emitter);
            emitter.GoOff();
        }

        protected virtual Emitter GetEmitterForAngle(IHasAngle angle, JudgementResult result)
            => Get(e => e.Apply(new Emitter.EmitterSettings
            {
                Amount = 10,
                Angle = angle.Angle,
                Inversed = properties?.InverseModEnabled?.Value ?? false,
                Colour = colour.ForHitResult(result.Type)
            }));

        protected virtual Emitter GetEmitterForHardBeat(JudgementResult result)
            => Get(e => e.Apply(new Emitter.EmitterSettings
            {
                Amount = 64,
                IsCircular = true,
                Inversed = properties?.InverseModEnabled?.Value ?? false,
                Colour = colour.ForHitResult(result.Type)
            }));
    }

    public abstract class Emitter : PoolableDrawable
    {
        private readonly List<Drawable> particles = new();
        protected EmitterSettings Settings { get; private set; }

        protected Emitter()
        {
            RelativePositionAxes = Axes.Both;
            RelativeSizeAxes = Axes.Both;
            Blending = BlendingParameters.Additive;
        }

        public void Apply(EmitterSettings settings)
        {
            Settings = settings;
            particles.Clear();

            for (var i = 0; i < settings.Amount; i++)
            {
                particles.Add(createDefaultProperties(Settings.IsCircular
                                                          ? CreateCircularParticle()
                                                          : CreateAngularParticle()));
            }
        }

        protected abstract Drawable CreateAngularParticle();
        protected abstract Drawable CreateCircularParticle();

        protected float Distance => Settings.Inversed ? 0.5f - Paddle.PADDLE_RADIUS : 0.5f;

        protected const double Duration = 1500;

        public void GoOff()
        {
            AddRangeInternal(particles);

            foreach (var particle in particles)
            {
                ApplyHitAnimation(particle);
            }

            this.Delay(Duration).Expire(true);
        }

        protected abstract void ApplyHitAnimation(Drawable particle);

        private Drawable createDefaultProperties(Drawable drawable)
            => drawable.With(d =>
            {
                d.RelativePositionAxes = Axes.Both;
                d.Anchor = Anchor.Centre;
                d.Origin = Anchor.BottomCentre;

                d.Colour = Settings.Colour;
                d.Rotation = (float)RNG.NextDouble() * 360f;
                d.Size = new Vector2(RNG.Next(5, 15));
                d.Alpha = RNG.NextSingle(0.25f, 1f);
            });

        public struct EmitterSettings
        {
            public float Angle { get; set; }
            public int Amount { get; set; }
            public Colour4 Colour { get; set; }
            public bool IsCircular { get; set; }
            public bool Inversed { get; set; }
        }
    }
}
