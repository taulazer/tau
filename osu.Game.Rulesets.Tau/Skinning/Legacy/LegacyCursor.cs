using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Tau.Skinning.Legacy
{
    public class LegacyCursor : CompositeDrawable
    {

        [BackgroundDependencyLoader]
        private void load(ISkinSource skin)
        {
            RelativeSizeAxes = Axes.Both;
            InternalChild = new Sprite
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                FillMode = FillMode.Fit,
                RelativeSizeAxes = Axes.Both,
                Texture = skin.GetTexture($"{TauRuleset.SHORT_NAME}-cursor")
            };
        }
    }
}
