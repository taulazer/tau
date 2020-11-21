using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Configuration;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Skinning;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableTauJudgement : DrawableJudgement
    {
        private SkinnableSprite lighting;
        private Bindable<Color4> lightingColour;

        public DrawableTauJudgement(JudgementResult result, DrawableHitObject judgedObject)
            : base(result, judgedObject)
        {
            RelativePositionAxes = Axes.Both;
            Scale = new Vector2(1.66f);
        }

        protected override Drawable CreateDefaultJudgement(HitResult result) => new TauJudgementPiece(result);

        [BackgroundDependencyLoader]
        private void load(OsuConfigManager config)
        {
            if (config.Get<bool>(OsuSetting.HitLighting) && Result.Type != HitResult.Miss)
            {
                AddInternal(lighting = new SkinnableSprite("lighting")
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Blending = BlendingParameters.Additive,
                    Depth = float.MaxValue
                });

                if (JudgedObject != null)
                {
                    lightingColour = JudgedObject.AccentColour.GetBoundCopy();
                    lightingColour.BindValueChanged(colour => lighting.Colour = colour.NewValue, true);
                }
                else
                {
                    lighting.Colour = Color4.White;
                }
            }
        }

        protected override void ApplyHitAnimations()
        {
            if (lighting != null)
            {
                JudgementBody.Delay(100).FadeOut(400);

                lighting.ScaleTo(0.8f).ScaleTo(1.2f, 600, Easing.Out);
                lighting.FadeIn(200).Then().Delay(200).FadeOut(1000);
            }

            base.ApplyHitAnimations();
        }

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
