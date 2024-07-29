using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Configuration;
using osu.Game.Rulesets.Tau.Objects.Drawables.Pieces;
using osu.Game.Rulesets.Tau.UI;
using osuTK;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public partial class DrawableTauJudgement : DrawableJudgement
    {
        internal Colour4 AccentColour { get; private set; }

        protected SkinnableLighting Lighting { get; private set; }

        [Resolved]
        private TauRulesetConfigManager config { get; set; }

        private float distance = 0.6f;
        private float angle;

        public DrawableTauJudgement()
        {
            RelativePositionAxes = Axes.Both;
            Scale = new Vector2(1.66f);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            AddInternal(Lighting = new SkinnableLighting
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Blending = BlendingParameters.Additive,
                Depth = float.MaxValue,
                Alpha = 0
            });
        }

        [BackgroundDependencyLoader(true)]
        private void load(TauCachedProperties properties)
        {
            if (properties != null && properties.InverseModEnabled.Value)
                distance = 0.4f;
        }

        public override void Apply(JudgementResult result, DrawableHitObject judgedObject)
        {
            base.Apply(result, judgedObject);

            if (judgedObject is not { } hitObj)
                return;

            AccentColour = hitObj.AccentColour.Value;

            angle = hitObj switch
            {
                DrawableAngledTauHitObject<Slider> { HitObject: IHasOffsetAngle ang } => ang.GetAbsoluteAngle(),
                // TODO: This should NOT be here.
                DrawableAngledTauHitObject<Beat> { HitObject: IHasOffsetAngle ang } => ang.GetAbsoluteAngle(),
                DrawableSliderHardBeat s => s.GetAbsoluteAngle(),
                DrawableStrictHardBeat s => s.HitObject.Angle,
                DrawableBeat b => b.HitObject.Angle,
                _ => 0f
            };
        }

        protected override void PrepareForUse()
        {
            base.PrepareForUse();

            Lighting.ResetAnimation();
            Lighting.SetColourFrom(this, Result);

            Position = Extensions.FromPolarCoordinates(distance, angle);
            Rotation = angle;
        }

        protected override void ApplyHitAnimations()
        {
            bool hitLightingEnabled = config.Get<bool>(TauRulesetSettings.HitLighting);

            Lighting.Alpha = 0;

            if (hitLightingEnabled && Lighting.Drawable != null)
            {
                Lighting.ScaleTo(0.8f).ScaleTo(1.2f, 600, Easing.Out);
                Lighting.FadeIn(200).Then().Delay(200).FadeOut(1000);

                LifetimeEnd = Lighting.LatestTransformEndTime;
            }

            base.ApplyHitAnimations();
        }

        protected override Drawable CreateDefaultJudgement(HitResult result) => new TauJudgementPiece(result);

        private partial class TauJudgementPiece : DefaultJudgementPiece
        {
            public TauJudgementPiece(HitResult result)
                : base(result)
            {
            }

            public override void PlayAnimation()
            {
                base.PlayAnimation();

                if (Result != HitResult.Miss)
                    JudgementText.TransformSpacingTo(Vector2.Zero).Then().TransformSpacingTo(new Vector2(14, 0), 1800, Easing.OutQuint);
            }
        }
    }
}
