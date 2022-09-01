using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Localisation;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModDifficultyAdjust : ModDifficultyAdjust
    {
        [SettingSource(typeof(ModStrings), nameof(ModStrings.DifficultyAdjustPaddleSizeName), nameof(ModStrings.DifficultyAdjustPaddleSizeDescription),
            FIRST_SETTING_ORDER - 1, SettingControlType = typeof(DifficultyAdjustSettingsControl))]
        public DifficultyBindable PaddleSize { get; } = new DifficultyBindable
        {
            Precision = 0.1f,
            MinValue = 0,
            MaxValue = 10,
            ExtendedMaxValue = 11,
            ReadCurrentFromDifficulty = diff => diff.CircleSize,
        };

        // [SettingSource("Approach Rate", "Override a beatmap's set AR.", LAST_SETTING_ORDER + 1, SettingControlType = typeof(DifficultyAdjustSettingsControl))]
        [SettingSource(typeof(ModStrings), nameof(ModStrings.DifficultyAdjustApproachRateName), nameof(ModStrings.DifficultyAdjustApproachRateDescription),
            FIRST_SETTING_ORDER + 1, SettingControlType = typeof(DifficultyAdjustSettingsControl))]
        public DifficultyBindable ApproachRate { get; } = new DifficultyBindable
        {
            Precision = 0.1f,
            MinValue = 0,
            MaxValue = 10,
            ExtendedMaxValue = 11,
            ReadCurrentFromDifficulty = diff => diff.ApproachRate,
        };

        public override string SettingDescription
        {
            get
            {
                string paddleSize = PaddleSize.IsDefault ? string.Empty : $"PS {PaddleSize.Value:N1}";
                string approachRate = ApproachRate.IsDefault ? string.Empty : $"AR {ApproachRate.Value:N1}";

                return string.Join(", ", new[]
                {
                    paddleSize,
                    base.SettingDescription,
                    approachRate
                }.Where(s => !string.IsNullOrEmpty(s)));
            }
        }

        protected override void ApplySettings(BeatmapDifficulty difficulty)
        {
            base.ApplySettings(difficulty);

            if (PaddleSize.Value != null) difficulty.CircleSize = PaddleSize.Value.Value;
            if (ApproachRate.Value != null) difficulty.ApproachRate = ApproachRate.Value.Value;
        }
    }
}
