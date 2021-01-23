using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Tau.Skinning.Legacy
{
    public class LegacyBeat : Sprite
    {
        [BackgroundDependencyLoader]
        private void load(ISkinSource skin)
        {
            Texture = skin.GetTexture("beat");
        }
    }
}
