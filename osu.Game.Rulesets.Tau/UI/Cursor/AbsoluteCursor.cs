using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Tau.Skinning.Default;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Rulesets.Tau.UI.Cursor
{
    public class AbsoluteCursor : CircularContainer
    {
        public AbsoluteCursor()
        {
            Size = new Vector2(40);
            Origin = Anchor.Centre;

            Children = new Drawable[]
            {
                new SkinnableDrawable(new TauSkinComponent(TauSkinComponents.Cursor), _ => new CursorPiece(), ConfineMode.ScaleToFit)
            };
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();
            Rotation += (float)Time.Elapsed / 5;
        }
    }
}
