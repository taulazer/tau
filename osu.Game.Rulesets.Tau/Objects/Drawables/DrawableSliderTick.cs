using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.UI;
using osuTK;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableSliderTick : DrawableAngledTauHitObject<SliderTick>
    {
        public DrawableSlider DrawableSlider => (DrawableSlider)ParentHitObject;

        private readonly BindableFloat size = new(12f);

        public override bool DisplayResult => false;

        public DrawableSliderTick(SliderTick hitObject)
            : base(hitObject)
        {
            Name = "Tick track";
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;
            Size = Vector2.One;

            AddInternal(new Container
            {
                RelativePositionAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                AlwaysPresent = true,
                Size = new Vector2(size.Default),
                Children = new Drawable[]
                {
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

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (HitObject.StartTime <= Time.Current)
                ApplyResult(r => r.Type = DrawableSlider.Tracking.Value ? HitResult.Great : HitResult.Miss);
        }
    }
}
