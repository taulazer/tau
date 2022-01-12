using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Tau.UI
{
    public class TauPlayfieldAdjustmentContainer : PlayfieldAdjustmentContainer
    {
        protected override Container<Drawable> Content => content;
        private readonly Container content;

        public TauPlayfieldAdjustmentContainer()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Size = new Vector2(.6f);
            FillMode = FillMode.Fit;
            FillAspectRatio = 1;

            InternalChild = content = new ScalingContainer { RelativeSizeAxes = Axes.Both };
        }

        /// <summary>
        /// A <see cref="Container"/> which scales its content relative to a target width.
        /// </summary>
        private class ScalingContainer : Container
        {
            protected override void Update()
            {
                base.Update();

                Scale = new Vector2(Parent.ChildSize.X / TauPlayfield.BASE_SIZE.X);

                Size = Vector2.Divide(Vector2.One, Scale);
            }
        }
    }
}
