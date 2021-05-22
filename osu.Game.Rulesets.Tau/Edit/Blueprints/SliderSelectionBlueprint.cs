using osu.Framework.Graphics.Primitives;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osuTK;

namespace osu.Game.Rulesets.Tau.Edit.Blueprints
{
    public class SliderSelectionBlueprint : TauSelectionBlueprint<Slider>
    {
        protected new DrawableSlider DrawableObject => (DrawableSlider)base.DrawableObject;

        public SliderSelectionBlueprint(Slider hitObject)
            : base(hitObject)
        {
        }

        protected override void Update()
        {
            base.Update();
        }

        public override Vector2 ScreenSpaceSelectionPoint => DrawableObject.ScreenSpaceDrawQuad.Centre;

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => false;

        public override Quad SelectionQuad => DrawableObject.ScreenSpaceDrawQuad.AABB;
    }
}
