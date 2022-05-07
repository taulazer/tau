using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Replays;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModAutoplay : ModAutoplay
    {
        public override Type[] IncompatibleMods => base.IncompatibleMods.Concat(new[] { typeof(TauModAutopilot) }).ToArray();

        public override ModReplayData CreateReplayData(IBeatmap beatmap, IReadOnlyList<Mod> mods)
            => new(new TauAutoGenerator(beatmap, mods).Generate(), new ModCreatedUser { Username = "Astraeus" });
    }

    public class TauModShowoffAutoplay : ModAutoplay {
        public override IconUsage? Icon => FontAwesome.Regular.Eye;
        public override Type[] IncompatibleMods => base.IncompatibleMods.Concat( new[] { typeof( TauModAutopilot ) } ).ToArray();

        public override ModReplayData CreateReplayData ( IBeatmap beatmap, IReadOnlyList<Mod> mods )
            => new( new ShowoffAutoGenerator( beatmap, mods ).Generate(), new ModCreatedUser { Username = "Redez" } );
    }
}
