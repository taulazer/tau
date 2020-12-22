using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Tau.Skinning.Legacy
{
    public class LegacyHardBeat : Sprite
    {
        [BackgroundDependencyLoader]
        private void load(ISkinSource skin)
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Texture = skin.GetTexture("hard-beat");
        }
    }
}
