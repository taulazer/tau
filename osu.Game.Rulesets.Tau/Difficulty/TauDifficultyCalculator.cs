using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Tau.Difficulty;
using osu.Game.Rulesets.Tau.Difficulty.Skills;
using osu.Game.Rulesets.Tau.Difficulty.Preprocessing;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Scoring;

using System;
using System.Linq;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Tau
{
    public class TauDifficultyCalculator : DifficultyCalculator
    {
        private const double difficulty_multiplier = 0.0675;
        public TauDifficultyCalculator(Ruleset ruleset, WorkingBeatmap beatmap) : base(ruleset, beatmap) { }

        protected override DifficultyAttributes CreateDifficultyAttributes(IBeatmap beatmap, Mod[] mods, Skill[] skills, double clockRate)
        {
            if (beatmap.HitObjects.Count == 0)
                return new TauDifficultyAttributes { Mods = mods, Skills = skills };

            double aimRating = Math.Sqrt(skills[0].DifficultyValue()) * difficulty_multiplier;
            double speedRating = Math.Sqrt(skills[1].DifficultyValue()) * difficulty_multiplier;
            double starRating = aimRating + speedRating + (Math.Abs(aimRating - speedRating) / 2);
            
            // Uncomment to see aimrating vs speedrating of a map: if aim rating is 3.5 and speed rating is 2.6 then sr will be 352.6
            // starRating = Math.Round(aimRating,1)*100 + Math.Round(speedRating,1);

            HitWindows hitWindows = new TauHitWindows();
            hitWindows.SetDifficulty(beatmap.BeatmapInfo.BaseDifficulty.OverallDifficulty);

            // Todo: These int casts are temporary to achieve 1:1 results with osu!stable, and should be removed in the future
            double hitWindowGreat = (int)(hitWindows.WindowFor(HitResult.Great)) / clockRate;
            double preempt = (int)BeatmapDifficulty.DifficultyRange(beatmap.BeatmapInfo.BaseDifficulty.ApproachRate, 1800, 1200, 450) / clockRate;

            int maxCombo = beatmap.HitObjects.Count;

            // IReadOnlyList<double> aimPeaks = skills[0].StrainPeaks;

            // Add the ticks + tail of the slider. 1 is subtracted because the head circle would be counted twice (once for the slider itself in the line above)
            // maxCombo += beatmap.HitObjects.OfType<Slider>().Sum(s => s.NestedHitObjects.Count - 1);

            return new TauDifficultyAttributes
            {
                StarRating = starRating,
                Mods = mods,
                AimStrain = aimRating,
                SpeedStrain = speedRating,
                ApproachRate = preempt > 1200 ? (1800 - preempt) / 120 : ((1200 - preempt) / 150) + 5,
                OverallDifficulty = (80 - hitWindowGreat) / 6,
                MaxCombo = maxCombo,
                Skills = skills,
            };
        }

        protected override IEnumerable<DifficultyHitObject> CreateDifficultyHitObjects(IBeatmap beatmap, double clockRate)
        {
            // The first jump is formed by the first two hitobjects of the map.
            // If the map has less than two OsuHitObjects, the enumerator will not return anything.
            for (int i = 1; i < beatmap.HitObjects.Count; i++)
            {
                var lastLast = i > 1 ? beatmap.HitObjects[i - 2] : null;
                var last = beatmap.HitObjects[i - 1];
                var current = beatmap.HitObjects[i];

                yield return new TauDifficultyHitObject(current, lastLast, last, clockRate, beatmap);
            }
        }

        protected override Skill[] CreateSkills(IBeatmap beatmap) => new Skill[]
        {
            new Aim(),
            new Speed()
        };
        protected override Mod[] DifficultyAdjustmentMods => new Mod[]
        {
            new TauModDoubleTime(),
            new TauModHalfTime(),
            new TauModEasy(),
            new TauModHardRock(),
        };
    }
}
