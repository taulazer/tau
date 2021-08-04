using JetBrains.Annotations;
using osu.Framework.Allocation;

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

        private void updateAngle()
        {
            if (Slider != null)
                HitObject.Angle = Slider.Nodes[0].Angle;
        }
    }
}
