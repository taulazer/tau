using osu.Framework;

namespace osu.Game.Rulesets.Tau.Game
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            using var host = Host.GetSuitableDesktopHost(@"osu");
            host.Run(new OsuGameDesktop());
        }
    }
}
