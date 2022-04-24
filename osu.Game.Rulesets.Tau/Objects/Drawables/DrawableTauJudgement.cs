using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Configuration;
using osu.Game.Rulesets.Tau.UI;
using osuTK;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableTauJudgement : DrawableJudgement
    {
        protected SkinnableLighting Lighting { get; private set; }

        [Resolved]
        private TauRulesetConfigManager config { get; set; }

        [Resolved(canBeNull: true)]
        private TauPlayfield playfield { get; set; }

        [BackgroundDependencyLoader]
        private void load()
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

        protected override void PrepareForUse()
        {
            base.PrepareForUse();

            Lighting.ResetAnimation();
            Lighting.SetColourFrom(JudgedObject, Result);

            var angle = 0f;

            if (JudgedObject is DrawableBeat b)
                angle = b.HitObject.Angle;

            if (JudgedObject is DrawableSlider s)
                angle = s.HitObject.Nodes.Last().Angle;

            Position = Extensions.GetCircularPosition((playfield?.Inversed ?? false) ? .3f : .6f, angle);
            Rotation = angle;
        }

        protected override void ApplyHitAnimations()
        {
            var hitLightingEnabled = config.Get<bool>(TauRulesetSettings.HitLighting);

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

        private class TauJudgementPiece : DefaultJudgementPiece
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
