using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Replays;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModAutoplay : ModAutoplay
    {
        public override Type[] IncompatibleMods => base.IncompatibleMods.Concat(new[] { typeof(TauModAutopilot) }).ToArray();

        public override Score CreateReplayScore(IBeatmap beatmap, IReadOnlyList<Mod> mods) => new Score
        {
            ScoreInfo = new ScoreInfo { User = new APIUser { Username = "Astraeus" } },
            Replay = new TauAutoGenerator(beatmap, mods).Generate()
        };
    }
}
