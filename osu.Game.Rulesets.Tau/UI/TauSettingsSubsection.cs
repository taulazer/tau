using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Tau.Configuration;

namespace osu.Game.Rulesets.Tau.UI
{
    public class TauSettingsSubsection : RulesetSettingsSubsection
    {
        protected override LocalisableString Header => "tau";

        public TauSettingsSubsection(Ruleset ruleset)
            : base(ruleset)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (TauRulesetConfigManager)Config;

            if (config == null)
                return;

            Children = new Drawable[]
            {
                new SettingsCheckbox
                {
                    LabelText = "Show Visualizer",
                    Current = config.GetBindable<bool>(TauRulesetSettings.ShowVisualizer)
                },
                new SettingsCheckbox
                {
                    LabelText = "Hit lighting",
                    Current = config.GetBindable<bool>(TauRulesetSettings.HitLighting),
                    WarningText = "This has been moved into the ruleset settings to reduce the chance of Tau breaking."
                },
                new SettingsSlider<float>
                {
                    LabelText = "Playfield dim",
                    Current = config.GetBindable<float>(TauRulesetSettings.PlayfieldDim),
                    KeyboardStep = 0.01f,
                    DisplayAsPercentage = true
                },
                new SettingsSlider<float>
                {
                    LabelText = "Beat Size",
                    Current = config.GetBindable<float>(TauRulesetSettings.BeatSize),
                    KeyboardStep = 1f
                },
                new SettingsEnumDropdown<KiaiType>
                {
                    LabelText = "Kiai Effect",
                    Current = config.GetBindable<KiaiType>(TauRulesetSettings.KiaiEffect),
                }
            };
        }
    }
}
