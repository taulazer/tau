using osu.Framework.Bindables;
using osu.Framework.Utils;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using System;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Tau.Localisation;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModNoScope : ModNoScope, IUpdatableByPlayfield
    {
        public override LocalisableString Description => ModStrings.NoScopeDescription;

        [SettingSource(
            typeof(ModStrings),
            nameof(ModStrings.NoScopeThresholdName),
            nameof(ModStrings.NoScopeThresholdDescription),
            SettingControlType = typeof(SettingsSlider<int, HiddenComboSlider>)
        )]
        public override BindableInt HiddenComboCount { get; } = new()
        {
            Default = 10,
            Value = 10,
            MinValue = 0,
            MaxValue = 50,
        };

        public void Update(Playfield playfield)
        {
            bool shouldAlwaysShowCursor = IsBreakTime.Value;
            float targetAlpha = shouldAlwaysShowCursor ? 1 : ComboBasedAlpha;
            playfield.Cursor.Alpha = (float)Interpolation.Lerp(playfield.Cursor.Alpha, targetAlpha, Math.Clamp(playfield.Time.Elapsed / TRANSITION_DURATION, 0, 1));
        }
    }
}
