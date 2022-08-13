using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Utils;
using osu.Game.Graphics;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.UI.Cursor;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.UI.Effects
{
    /// <summary>
    /// An abstracted Kiai effect that can be used to display different kinds of particles.
    /// </summary>
    /// <typeparam name="T">The effect emitter</typeparam>
    public abstract class KiaiEffect<T> : DrawablePool<T>
        where T : Emitter, new()
    {
        protected KiaiEffect(int initialSize)
            : base(initialSize)
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
                IHasOffsetAngle angle => getEmitterForAngle(angle.GetAbsoluteAngle(), result),
                IHasAngle angle => getEmitterForAngle(angle.Angle, result),
                HardBeat => getEmitterForHardBeat(result),
                _ => new T()
            };

            AddInternal(emitter);
            emitter.ApplyAnimations();
        }

        private Emitter getEmitterForAngle(float angle, JudgementResult result)
            => Get(e => e.Apply(new Emitter.EmitterSettings
            {
                Amount = 10,
                Angle = angle,
                Inversed = properties?.InverseModEnabled?.Value ?? false,
                Colour = hitResultColorOrDefault(result.Type, TauPlayfield.ACCENT_COLOUR.Value)
            }));

        private Emitter getEmitterForHardBeat(JudgementResult result)
            => Get(e => e.Apply(new Emitter.EmitterSettings
            {
                Amount = 64,
                IsCircular = true,
                Inversed = properties?.InverseModEnabled?.Value ?? false,
                Colour = hitResultColorOrDefault(result.Type, TauPlayfield.ACCENT_COLOUR.Value)
            }));

        private Color4 hitResultColorOrDefault(HitResult type, Color4 fallback)
        {
            var col = colour.ForHitResult(type);
            return col == Color4.White ? fallback : col;
        }
    }

    /// <summary>
    /// Creates, handles, and animates particles.
    /// </summary>
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

        /// <summary>
        /// Clears, creates, and applies default settings to particles.
        /// </summary>
        /// <param name="settings">The setting to apply to this emitter.</param>
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

        protected const double Duration = 1000;

        /// <summary>
        /// Applies animation to each particles.
        /// </summary>
        public void ApplyAnimations()
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
            /// <summary>
            /// The angle this emitter should be rotated at.
            /// </summary>
            public float Angle { get; init; }

            /// <summary>
            /// The amount of particles this emitter should produce.
            /// </summary>
            public int Amount { get; init; }

            /// <summary>
            /// The colour for all of the particles.
            /// </summary>
            public Colour4 Colour { get; init; }

            /// <summary>
            /// Whether or not the particles should be spread out circularly.
            /// Note that this will nullify <see cref="Angle"/>.
            /// </summary>
            public bool IsCircular { get; init; }

            /// <summary>
            /// Whether or not the particles should be spewed inwards or outwards.
            /// </summary>
            public bool Inversed { get; init; }
        }
    }
}
