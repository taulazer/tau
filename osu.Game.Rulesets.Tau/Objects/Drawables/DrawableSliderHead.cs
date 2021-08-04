using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableSliderHead : DrawableBeat
    {
        public new SliderHeadBeat HitObject => (SliderHeadBeat)base.HitObject;

        [CanBeNull]
        public Slider Slider => DrawableSlider?.HitObject;

        protected DrawableSlider DrawableSlider => (DrawableSlider)ParentHitObject;

        public DrawableSliderHead()
        {
        }

        public DrawableSliderHead(SliderHeadBeat obj)
            : base(obj)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Slider?.Nodes.BindCollectionChanged((_, __) => updateAngle(), true);
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            // Since the beat box expects to be offset by an anchor,
            // we have to manually override the MoveToY transform so that it perfectly goes half a circle.
            Box.MoveToY(-0.5f, HitObject.TimePreempt);
        }

        private void updateAngle()
        {
            if (Slider != null)
                HitObject.Angle = Slider.Nodes[0].Angle;
        }
    }
}
