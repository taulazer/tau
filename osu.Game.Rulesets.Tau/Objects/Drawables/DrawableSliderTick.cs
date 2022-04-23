using osu.Game.Rulesets.Scoring;
using osu.Framework.Graphics;

#if SHOW_TICKS
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;
#endif

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableSliderTick : DrawableBeat
    {
        public DrawableSlider DrawableSlider => (DrawableSlider)ParentHitObject;

        public override bool DisplayResult => false;

        public DrawableSliderTick()
        {
        }

        public DrawableSliderTick(Beat hitObject)
            : base(hitObject)
        {
        }

        protected override Drawable CreateDrawable()
        {
#if SHOW_TICKS
            return new Container
            {
                RelativePositionAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0,
                AlwaysPresent = true,
                Size = new Vector2(NoteSize.Default),
                Child = new Circle
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = Color4.Red
                }
            };
#else
            return base.CreateDrawable();
#endif
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (HitObject.StartTime <= Time.Current)
                ApplyResult(r => r.Type = DrawableSlider.Tracking.Value ? HitResult.LargeTickHit : HitResult.LargeTickMiss);
        }
    }
}
