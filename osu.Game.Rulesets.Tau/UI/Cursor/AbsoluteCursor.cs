using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
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
                new CircularProgress
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Current = new BindableDouble(.33f),
                    InnerRadius = 0.1f,
                    Rotation = -150
                },
                new CircularProgress
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Current = new BindableDouble(.33f),
                    InnerRadius = 0.1f,
                    Rotation = 30
                },
            };
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();
            Rotation += (float)Time.Elapsed / 5;
        }
    }
}
