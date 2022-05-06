using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Objects.Drawables.Pieces;
using osuTK;
using System;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableSliderRepeat : DrawableBeat
    {
        public DrawableSlider DrawableSlider => (DrawableSlider)ParentHitObject;

        public override bool DisplayResult => false;

        public DrawableSliderRepeat()
        {
        }

        public DrawableSliderRepeat(Beat hitObject)
            : base(hitObject)
        {
        }

        protected override void Update()
        {
            base.Update();

            Alpha = 0.0f;
            AlwaysPresent = true;
        }

        public Drawable InnerDrawableBox = new Container {
            RelativePositionAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Alpha = 0,
            AlwaysPresent = true,
            Child = new BeatPiece()
        };

        protected override void LoadComplete()
        {
            base.LoadComplete();
            InnerDrawableBox.Size = DrawableBox.Size;
            AddInternal(InnerDrawableBox);

            DrawableBox.Size = Vector2.Multiply(DrawableBox.Size, 15f / 16f);
            DrawableBox.Rotation = 45;
            InnerDrawableBox.Rotation = 45;
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            InnerDrawableBox.Position = DrawableBox.Position;
            InnerDrawableBox.Size = Vector2.Multiply(DrawableBox.Size, 7.5f / 15f);
            InnerDrawableBox.Scale = DrawableBox.Scale;
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (HitObject.StartTime <= Time.Current)
                ApplyResult(r => r.Type = DrawableSlider.Tracking.Value ? HitResult.Great : HitResult.Miss);
        }

        protected override void OnApply()
        {
            base.OnApply();

            DrawableBox.Y = Properties?.InverseModEnabled.Value == true ? -1.0f : 0;

            DrawableBox.Alpha = 0;
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            DrawableBox.FadeIn(HitObject.TimeFadeIn);

            DrawableBox.MoveToY(-0.5f, HitObject.TimePreempt);
        }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            var velocity = -0.5f / (float)HitObject.TimePreempt;
            var timeFadeHit = DrawableSlider.FADE_RANGE / Math.Abs(velocity);

            if (Properties?.InverseModEnabled.Value == true)
                velocity *= -1;

            switch (state)
            {
                case ArmedState.Hit or ArmedState.Miss:
                    DrawableBox.MoveToY(-0.5f + velocity * timeFadeHit, timeFadeHit);

                    this.Delay(timeFadeHit).Expire();
                    break;
            }
        }
    }
}
