using System;
using System.Linq;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModSuddenDeath : ModSuddenDeath
    {
        public override Type[] IncompatibleMods => base.IncompatibleMods.Concat(new[] { typeof(TauModAutopilot) }).ToArray();
    }
}
