using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace osu.Game.Rulesets.Tau.UI.Cursor
{
    public partial class AbsoluteCursor : Container
    {
        public AbsoluteCursor()
        {
            Size = new Vector2(40);
            Origin = Anchor.Centre;

            Child = new CursorPiece();
        }
    }
}
