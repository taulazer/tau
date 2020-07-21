using osu.Framework.Graphics;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osuTK;

namespace osu.Game.Rulesets.Tau.Edit.Blueprints
{
    public class TauSelectionBlueprint : OverlaySelectionBlueprint
    {
        public TauSelectionBlueprint(DrawableHitObject drawableObject)
            : base(drawableObject)
        {
            Rotation = drawableObject.Rotation;
            RelativeSizeAxes = Axes.None;

            AddInternal(new HitPiece
            {
                RelativeSizeAxes = Axes.Both,
                Origin = Anchor.TopLeft
            });
        }

        protected override void Update()
        {
            base.Update();

            // Move the rectangle to cover the hitobjects
            var topLeft = new Vector2(float.MaxValue, float.MaxValue);
            var bottomRight = new Vector2(float.MinValue, float.MinValue);

            topLeft = Vector2.ComponentMin(topLeft, Parent.ToLocalSpace(DrawableObject.ScreenSpaceDrawQuad.TopLeft));
            bottomRight = Vector2.ComponentMax(bottomRight, Parent.ToLocalSpace(DrawableObject.ScreenSpaceDrawQuad.BottomRight));

            Size = DrawableObject is DrawableHardBeat ? bottomRight - topLeft : new Vector2(16);
            Position = topLeft;
        }
    }
}
