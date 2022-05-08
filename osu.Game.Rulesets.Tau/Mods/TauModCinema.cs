using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Replays;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModCinema : ModCinema<TauHitObject>
    {
        public override Type[] IncompatibleMods => base.IncompatibleMods.Concat(new[] { typeof(TauModAutopilot) }).ToArray();

        public override ModReplayData CreateReplayData(IBeatmap beatmap, IReadOnlyList<Mod> mods)
            => new(new TauAutoGenerator(beatmap, mods).Generate(), new ModCreatedUser { Username = "Astraeus" });
    }
}
