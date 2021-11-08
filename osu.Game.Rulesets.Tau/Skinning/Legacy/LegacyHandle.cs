using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Rulesets.Tau.Skinning.Legacy
{
    public class LegacyHandle : CompositeDrawable
    {
        private Sprite line;
        private Sprite handle;

        [BackgroundDependencyLoader]
        private void load(ISkinSource skin)
        {
            RelativeSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                line = new Sprite
                {
                    EdgeSmoothness = new Vector2(1f),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    FillMode = FillMode.Fit,
                    RelativeSizeAxes = Axes.Both,
                    Texture = skin.GetTexture($"{TauRuleset.SHORT_NAME}-handle-line")
                },
                handle = new Sprite
                {
                    RelativePositionAxes = Axes.Both,
                    RelativeSizeAxes = Axes.Both,
                    Y = -.25f,
                    Size = new Vector2(.03f),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    FillMode = FillMode.Fit,
                    Texture = skin.GetTexture($"{TauRuleset.SHORT_NAME}-handle")
                }
            };
        }

        protected override bool OnMouseMove(MouseMoveEvent e)
        {
            handle.Y = -Math.Clamp(Vector2.Distance(AnchorPosition, e.MousePosition) / DrawHeight, 0.15f, .45f);

            return base.OnMouseMove(e);
        }
    }
}
