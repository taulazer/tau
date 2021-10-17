using osu.Framework.Graphics.Sprites;

namespace osu.Game.Rulesets.Tau
{
    public class TauIcon
    {
        public static IconUsage Get(int icon) => new IconUsage((char)icon, "tauFont");

        public static IconUsage ModHidden => Get(8280);

        public static IconUsage ModFadeIn => Get(8281);

        public static IconUsage ModInverse => Get(8282);
    }
}
