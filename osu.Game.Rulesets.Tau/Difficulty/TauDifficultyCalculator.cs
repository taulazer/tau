using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Difficulty.Preprocessing;
using osu.Game.Rulesets.Tau.Difficulty.Skills;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Scoring;
using osu.Game.Rulesets.Tau.UI;

namespace osu.Game.Rulesets.Tau.Difficulty
{
    public class TauDifficultyCalculator : DifficultyCalculator
    {
        private readonly TauCachedProperties properties = new();
        private double hitWindowGreat;

        public TauDifficultyCalculator(IRulesetInfo ruleset, IWorkingBeatmap beatmap)
            : base(ruleset, beatmap)
        {
        }

        protected override DifficultyAttributes CreateDifficultyAttributes(IBeatmap beatmap, Mod[] mods, Skill[] skills, double clockRate)
        {
            if (beatmap.HitObjects.Count == 0)
                return new DifficultyAttributes { Mods = mods };

            var aim = Math.Sqrt(skills[0].DifficultyValue()) * 0.153;
            var speed = skills[1].DifficultyValue();

            double preempt = IBeatmapDifficultyInfo.DifficultyRange(beatmap.Difficulty.ApproachRate, 1800, 1200, 450) / clockRate;

            return new TauDifficultyAttributes
            {
                AimDifficulty = aim,
                StarRating = aim, // TODO: Include speed.
                Mods = mods,
                MaxCombo = beatmap.HitObjects.Count,
                OverallDifficulty = (80 - hitWindowGreat) / 6,
                ApproachRate = preempt > 1200 ? (1800 - preempt) / 120 : (1200 - preempt) / 150 + 5,
            };
        }

        protected override IEnumerable<DifficultyHitObject> CreateDifficultyHitObjects(IBeatmap beatmap, double clockRate)
        {
            properties.SetRange(beatmap.Difficulty.CircleSize);

            TauHitObject lastObject = null;

            foreach (var hitObject in beatmap.HitObjects.Cast<TauHitObject>())
            {
                if (hitObject is not AngledTauHitObject)
                    continue;

                if (lastObject != null)
                    yield return new TauAngledDifficultyHitObject(hitObject, lastObject, clockRate, properties);

                lastObject = hitObject;
            }
        }

        protected override Skill[] CreateSkills(IBeatmap beatmap, Mod[] mods, double clockRate)
        {
            HitWindows hitWindows = new TauHitWindow();
            hitWindows.SetDifficulty(beatmap.Difficulty.OverallDifficulty);

            hitWindowGreat = hitWindows.WindowFor(HitResult.Great) / clockRate;
            return new Skill[]
            {
                new Aim(mods),
                new Speed(mods)
            };
        }
    }
}
