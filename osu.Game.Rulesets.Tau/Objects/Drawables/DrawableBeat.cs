using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Objects.Drawables.Pieces;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableBeat : DrawableTauHitObject<Beat>, IKeyBindingHandler<TauAction>
    {
        public Drawable DrawableBox;

        /// <summary>
        /// Check to see whether or not this Hit object is in the paddle's range.
        /// Also returns the amount of difference from the center of the paddle this Hit object was validated at.
        /// </summary>
        public Func<Beat, ValidationResult> CheckValidation;

        public DrawableBeat(Beat hitObject)
            : base(hitObject)
        {
            Name = "Beat track";
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;
            Size = Vector2.One;

            AddInternal(DrawableBox = new Container
            {
                RelativePositionAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.TopCentre,
                Alpha = 0,
                AlwaysPresent = true,
                Size = new Vector2(16),
                Child = new BeatPiece()
            });

            angleBindable.BindValueChanged(r => Rotation = r.NewValue);
        }

        private readonly BindableFloat angleBindable = new BindableFloat();

        protected override void OnApply()
        {
            base.OnApply();
            angleBindable.BindTo(HitObject.AngleBindable);
        }

        protected override void OnFree()
        {
            base.OnFree();
            angleBindable.UnbindFrom(HitObject.AngleBindable);
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            DrawableBox.FadeIn(HitObject.TimeFadeIn);
            DrawableBox.MoveToY(-0.5f, HitObject.TimePreempt);
        }

        protected override bool CheckForValidation() =>
            CheckValidation != null && CheckValidation(HitObject).IsValid;

        public bool OnPressed(KeyBindingPressEvent<TauAction> e)
        {
            if (Judged)
                return false;

            return Actions.Contains(e.Action) && UpdateResult(true);
        }

        public void OnReleased(KeyBindingReleaseEvent<TauAction> e)
        {
        }

        [Resolved]
        private OsuColour colour { get; set; }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            base.UpdateHitStateTransforms(state);

            const double time_fade_hit = 250, time_fade_miss = 400;

            switch (state)
            {
                case ArmedState.Hit:
                    DrawableBox.ScaleTo(2f, time_fade_hit, Easing.OutQuint)
                               .FadeColour(colour.ForHitResult(Result.Type), time_fade_hit, Easing.OutQuint)
                               .MoveToOffset(new Vector2(0, -.1f), time_fade_hit, Easing.OutQuint)
                               .FadeOut(time_fade_hit);

                    this.Delay(time_fade_hit).Expire();

                    break;

                case ArmedState.Miss:
                    DrawableBox.ScaleTo(0.5f, time_fade_miss, Easing.InQuint)
                               .FadeColour(Color4.Red, time_fade_miss, Easing.OutQuint)
                               .MoveToOffset(new Vector2(0, -.1f), time_fade_hit, Easing.OutQuint)
                               .FadeOut(time_fade_miss);

                    this.Delay(time_fade_miss).Expire();

                    break;
            }
        }
    }
}
