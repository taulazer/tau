using osu.Framework.Graphics.Sprites;

namespace osu.Game.Rulesets.Tau
{
    public class TauIcons
    {
        private static IconUsage get(int icon) => new((char)icon, "tauFont");

        public static IconUsage Tau => get(8280);

        public static IconUsage ModFadeOut => get(8281);

        public static IconUsage ModFadeIn => get(8282);

        public static IconUsage ModInverse => get(8283);
    }
}
