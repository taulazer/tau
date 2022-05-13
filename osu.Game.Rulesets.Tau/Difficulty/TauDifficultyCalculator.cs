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
        private double difficultyMultiplier = 0.0825;

        public TauDifficultyCalculator(IRulesetInfo ruleset, IWorkingBeatmap beatmap)
            : base(ruleset, beatmap)
        {
        }

        protected override DifficultyAttributes CreateDifficultyAttributes(IBeatmap beatmap, Mod[] mods, Skill[] skills, double clockRate)
        {
            if (beatmap.HitObjects.Count == 0)
                return new DifficultyAttributes { Mods = mods };

            var aim = Math.Sqrt(skills[0].DifficultyValue()) * difficultyMultiplier;
            var speed = Math.Sqrt(skills[1].DifficultyValue()) * difficultyMultiplier;

            double preempt = IBeatmapDifficultyInfo.DifficultyRange(beatmap.Difficulty.ApproachRate, 1800, 1200, 450) / clockRate;

            double baseAimPerformance = Math.Pow(5 * Math.Max(1, aim / 0.0675) - 4, 3) / 100000;
            double baseSpeedPerformance = Math.Pow(5 * Math.Max(1, speed / 0.0675) - 4, 3) / 100000;

            double basePerformance =
                Math.Pow(
                    Math.Pow(baseAimPerformance, 1.1) +
                    Math.Pow(baseSpeedPerformance, 1.1), 1.0 / 1.1
                );

            double starRating = basePerformance > 0.00001 ? Math.Cbrt(1.12) * 0.027 * (Math.Cbrt(100000 / Math.Pow(2, 1 / 1.1) * basePerformance) + 4) : 0;

            return new TauDifficultyAttributes
            {
                AimDifficulty = aim,
                SpeedDifficulty = speed,
                StarRating = starRating,
                Mods = mods,
                MaxCombo = beatmap.GetMaxCombo(),
                OverallDifficulty = beatmap.Difficulty.OverallDifficulty,
                ApproachRate = preempt > 1200 ? (1800 - preempt) / 120 : (1200 - preempt) / 150 + 5,
                NotesCount = beatmap.HitObjects.Count(h => h is Beat and not SliderHeadBeat and not SliderRepeat and not SliderTick),
                SliderCount = beatmap.HitObjects.Count(s => s is Slider),
                HardBeatCount = beatmap.HitObjects.Count(hb => hb is HardBeat)
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
                new Speed(mods, hitWindowGreat)
            };
        }
    }
}
