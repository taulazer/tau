using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Objects.Drawables.Pieces;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableSliderRepeat : DrawableBeat
    {
        public DrawableSlider DrawableSlider => (DrawableSlider)ParentHitObject;

        public override bool DisplayResult => false;

        public DrawableSliderRepeat()
        {
        }

        public DrawableSliderRepeat(Beat hitObject)
            : base(hitObject)
        {
        }

        protected override void Update () {
            base.Update();

            Alpha = 0.0f;
            AlwaysPresent = true;
        }

        public Drawable InnerDrawableBox;
        protected override void LoadComplete()
        {
            base.LoadComplete();
            AddInternal( InnerDrawableBox = new Container {
                RelativePositionAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0,
                AlwaysPresent = true,
                Size = DrawableBox.Size,
                Child = new BeatPiece()
            } );

            DrawableBox.Size = Vector2.Multiply( DrawableBox.Size, 1.25f );
            DrawableBox.Rotation = 45;
            InnerDrawableBox.Rotation = 45;
        }

        protected override void UpdateAfterChildren () {
            base.UpdateAfterChildren();

            InnerDrawableBox.Position = DrawableBox.Position;
            InnerDrawableBox.Size = Vector2.Multiply( DrawableBox.Size, 0.65f );
            InnerDrawableBox.Scale = DrawableBox.Scale;
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (HitObject.StartTime <= Time.Current)
                ApplyResult(r => r.Type = DrawableSlider.Tracking.Value ? HitResult.Great : HitResult.Miss);
        }
    }
}
