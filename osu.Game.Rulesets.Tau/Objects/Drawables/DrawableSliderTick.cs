using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Configuration;
using osu.Game.Rulesets.Tau.UI;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableSliderTick : DrawableAngledTauHitObject<SliderTick>
    {
        public DrawableSlider DrawableSlider => (DrawableSlider)ParentHitObject;

        public Drawable DrawableCircle;

        private readonly BindableFloat size = new(12f);

        public override bool DisplayResult => false;

        public DrawableSliderTick()
            : this(null)
        {
        }

        public DrawableSliderTick(SliderTick hitObject)
            : base(hitObject)
        {
            Name = "Tick track";
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;
            Size = Vector2.One;

            AddInternal(DrawableCircle = new Container
            {
                RelativePositionAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0,
                AlwaysPresent = true,
                Size = new Vector2(size.Default),
                Children = new Drawable[]
                {
                    new Circle
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both
                    },
                    new Circle
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Colour = Color4.Black,
                        RelativeSizeAxes = Axes.Both,
                        Size = Vector2.Multiply(Size, 0.5f)
                    }
                }
            });

            angleBindable.BindValueChanged(r => Rotation = r.NewValue);
        }

        private readonly BindableFloat angleBindable = new();

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

        [Resolved(canBeNull: true)]
        private TauCachedProperties properties { get; set; }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            DrawableCircle.FadeIn(HitObject.TimeFadeIn);

            if (properties != null && properties.InverseModEnabled.Value)
                DrawableCircle.MoveToY(-1.0f);

            DrawableCircle.MoveToY(-0.5f, HitObject.TimePreempt);
        }

        [BackgroundDependencyLoader(true)]
        private void load(TauRulesetConfigManager config)
        {
            config?.BindWith(TauRulesetSettings.BeatSize, size);
            size.BindValueChanged(value => DrawableCircle.Size = new Vector2(value.NewValue), true);
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (HitObject.StartTime <= Time.Current)
                ApplyResult(r => r.Type = DrawableSlider.Tracking.Value ? HitResult.Great : HitResult.Miss);
        }

        [Resolved]
        private OsuColour colour { get; set; }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            base.UpdateHitStateTransforms(state);

            const double time_fade_hit = 250, time_fade_miss = 400;
            var offset = new Vector2(0, -.1f);

            if (properties != null && properties.InverseModEnabled.Value)
                offset.Y = -offset.Y;

            switch (state)
            {
                case ArmedState.Hit:
                    DrawableCircle.ScaleTo(2f, time_fade_hit, Easing.OutQuint)
                                  .FadeColour(colour.ForHitResult(Result.Type), time_fade_hit, Easing.OutQuint)
                                  .MoveToOffset(offset, time_fade_hit, Easing.OutQuint)
                                  .FadeOut(time_fade_hit);

                    this.Delay(time_fade_hit).Expire();

                    break;

                case ArmedState.Miss:
                    DrawableCircle.ScaleTo(0.5f, time_fade_miss, Easing.InQuint)
                                  .FadeColour(Color4.Red, time_fade_miss, Easing.OutQuint)
                                  .MoveToOffset(offset, time_fade_miss, Easing.OutQuint)
                                  .FadeOut(time_fade_miss);

                    this.Delay(time_fade_miss).Expire();

                    break;
            }
        }
    }
}
