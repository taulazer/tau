using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using System;
using System.Linq;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Tau
{
    public class TauDifficultyCalculator : DifficultyCalculator
    {
        public TauDifficultyCalculator(Ruleset ruleset, WorkingBeatmap beatmap) : base(ruleset, beatmap) { }

        protected override DifficultyAttributes CreateDifficultyAttributes(IBeatmap beatmap, Mod[] mods, Skill[] skills, double clockRate) =>
            new DifficultyAttributes
            {
                StarRating = beatmap.BeatmapInfo.StarDifficulty,
                Mods = mods,
                Skills = skills,
                MaxCombo = beatmap.HitObjects.Count(),
            };

        protected override IEnumerable<DifficultyHitObject> CreateDifficultyHitObjects(IBeatmap beatmap, double clockRate) => Array.Empty<DifficultyHitObject>();

        protected override Skill[] CreateSkills(IBeatmap beatmap) => Array.Empty<Skill>();
    }
}
