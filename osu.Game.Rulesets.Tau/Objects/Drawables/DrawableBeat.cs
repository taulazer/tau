using System.Diagnostics;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Configuration;
using osu.Game.Rulesets.Tau.Judgements;
using osu.Game.Rulesets.Tau.Skinning.Default;
using osu.Game.Skinning;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableBeat : DrawableTauHitObject, IKeyBindingHandler<TauAction>
    {
        public CompositeDrawable Box;
        public Container IntersectArea;
        public float HitDistance = -.1f;

        private bool validActionPressed;

        public DrawableBeat()
            : this(null)
        {
        }

        public DrawableBeat(Beat hitObject)
            : base(hitObject)
        {
        }

        private readonly Bindable<float> size = new Bindable<float>(16); // Change as you see fit.

        [BackgroundDependencyLoader(true)]
        private void load(TauRulesetConfigManager config)
        {
            config?.BindWith(TauRulesetSettings.BeatSize, size);

            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;
            Size = Vector2.One;

            AddRangeInternal(new Drawable[]
            {
                Box = new Container
                {
                    RelativePositionAxes = Axes.Both,
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Alpha = 0.05f,
                    Children = new Drawable[]
                    {
                        new SkinnableDrawable(new TauSkinComponent(TauSkinComponents.Beat), _ => new BeatPiece(), ConfineMode.ScaleToFit),
                        IntersectArea = new Container
                        {
                            Size = new Vector2(16),
                            RelativeSizeAxes = Axes.None,
                            Origin = Anchor.Centre,
                            Anchor = Anchor.Centre,
                            AlwaysPresent = true
                        }
                    }
                },
            });

            Position = Vector2.Zero;

            angleBindable.BindValueChanged(r => Rotation = r.NewValue);
            size.BindValueChanged(value => Box.Size = new Vector2(value.NewValue), true);
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

            Box.FadeIn(HitObject.TimeFadeIn);
            Box.MoveToY(-0.5f, HitObject.TimePreempt);
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            Debug.Assert(HitObject.HitWindows != null);

            if (CheckValidation == null)
                return;

            if (!userTriggered)
            {
                if (!HitObject.HitWindows.CanBeHit(timeOffset))
                    ApplyResult(r => r.Type = HitResult.Miss);

                return;
            }

            var validation = CheckValidation.Invoke(HitObject.Angle);

            if (validation.Item1)
            {
                var result = HitObject.HitWindows.ResultFor(timeOffset);

                if (result == HitResult.None || CheckHittable?.Invoke(this, Time.Current) == false)
                    return;

                if (!validActionPressed)
                    ApplyResult(r => r.Type = HitResult.Miss);
                else
                    ApplyResult(r =>
                    {
                        var beatResult = (TauJudgementResult)r;

                        if (result.IsHit())
                        {
                            beatResult.DeltaAngle = validation.Item2;
                        }

                        beatResult.Type = result;
                    });
            }
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
                    Box.ScaleTo(2f, time_fade_hit, Easing.OutQuint)
                       .FadeColour(colour.ForHitResult(Result.Type), time_fade_hit, Easing.OutQuint)
                       .MoveToOffset(new Vector2(0, HitDistance), time_fade_hit, Easing.OutQuint)
                       .FadeOut(time_fade_hit);

                    this.Delay(time_fade_hit).Expire();

                    break;

                case ArmedState.Miss:
                    Box.ScaleTo(0.5f, time_fade_miss, Easing.InQuint)
                       .FadeColour(Color4.Red, time_fade_miss, Easing.OutQuint)
                       .MoveToOffset(new Vector2(0, HitDistance), time_fade_hit, Easing.OutQuint)
                       .FadeOut(time_fade_miss);

                    this.Delay(time_fade_miss).Expire();

                    break;
            }
        }

        public bool OnPressed(KeyBindingPressEvent<TauAction> e)
        {
            if (Judged)
                return false;

            validActionPressed = HitActions.Contains(e.Action);

            var result = UpdateResult(true);

            if (IsHit)
                HitAction = e.Action;

            return result;
        }

        public void OnReleased(KeyBindingReleaseEvent<TauAction> e)
        {
        }
    }
}
