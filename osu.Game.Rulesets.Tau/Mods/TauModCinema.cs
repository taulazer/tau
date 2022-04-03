using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Replays;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModCinema : ModCinema<TauHitObject>
    {
        public override Score CreateReplayScore(IBeatmap beatmap, IReadOnlyList<Mod> mods) => new Score
        {
            ScoreInfo = new ScoreInfo { User = new APIUser { Username = "Astraeus" } },
            Replay = new TauAutoGenerator(beatmap, mods).Generate()
        };
    }
}
