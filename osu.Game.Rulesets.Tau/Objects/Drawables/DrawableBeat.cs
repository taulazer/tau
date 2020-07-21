using System.Diagnostics;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Configuration;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableBeat : DrawableTauHitObject, IKeyBindingHandler<TauAction>
    {
        public Container IntersectArea;

        private bool validActionPressed;

        public DrawableBeat(Beat hitObject)
            : base(hitObject)
        {
            RelativePositionAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            AddRangeInternal(new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both
                },
                IntersectArea = new Container
                {
                    Size = new Vector2(16),
                    RelativeSizeAxes = Axes.None,
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    AlwaysPresent = true
                }
            });

            Rotation = hitObject.Angle;
        }

        private readonly Bindable<float> size = new Bindable<float>(16); // Change as you see fit.

        [BackgroundDependencyLoader(true)]
        private void load(TauRulesetConfigManager config)
        {
            config?.BindWith(TauRulesetSettings.BeatSize, size);
            size.BindValueChanged(value => Size = new Vector2(value.NewValue), true);
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            this.FadeIn(HitObject.TimeFadeIn);
            this.MoveTo(Extensions.GetCircularPosition(0.485f, HitObject.Angle), HitObject.TimePreempt);
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            Debug.Assert(HitObject.HitWindows != null);

            if (CheckValidation == null) return;

            if (!userTriggered)
            {
                if (!HitObject.HitWindows.CanBeHit(timeOffset))
                    ApplyResult(r => r.Type = HitResult.Miss);

                return;
            }

            if (CheckValidation.Invoke(this))
            {
                var result = HitObject.HitWindows.ResultFor(timeOffset);

                if (result == HitResult.None)
                    return;

                if (!validActionPressed)
                    ApplyResult(r => r.Type = HitResult.Miss);
                else
                    ApplyResult(r => r.Type = result);
            }
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            base.UpdateStateTransforms(state);

            const double time_fade_hit = 250, time_fade_miss = 400;

            switch (state)
            {
                case ArmedState.Idle:
                    LifetimeStart = HitObject.StartTime - HitObject.TimePreempt;
                    HitAction = null;

                    break;

                case ArmedState.Hit:
                    this.ScaleTo(2f, time_fade_hit, Easing.OutQuint)
                        .FadeColour(Color4.Yellow, time_fade_hit, Easing.OutQuint)
                        .MoveToOffset(Extensions.GetCircularPosition(.075f, HitObject.Angle), time_fade_hit, Easing.OutQuint)
                        .FadeOut(time_fade_hit);

                    break;

                case ArmedState.Miss:
                    this.ScaleTo(0.5f, time_fade_miss, Easing.InQuint)
                        .FadeColour(Color4.Red, time_fade_miss, Easing.OutQuint)
                        .MoveToOffset(Extensions.GetCircularPosition(.075f, HitObject.Angle), time_fade_hit, Easing.OutQuint)
                        .FadeOut(time_fade_miss);

                    break;
            }
        }

        public bool OnPressed(TauAction action)
        {
            if (Judged)
                return false;

            validActionPressed = HitActions.Contains(action);

            var result = UpdateResult(true);

            if (IsHit)
                HitAction = action;

            return result;
        }

        public void OnReleased(TauAction action)
        {
        }
    }
}
