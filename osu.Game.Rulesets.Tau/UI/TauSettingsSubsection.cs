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

        private SettingsCheckbox showEffects;
        private SettingsCheckbox showVisualizer;
        private SettingsCheckbox showKiai;

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (TauRulesetConfigManager)Config;

            if (config == null)
                return;

            Children = new Drawable[]
            {
                showEffects = new SettingsCheckbox
                {
                    LabelText = "Show effects",
                    Current = config.GetBindable<bool>(TauRulesetSettings.ShowEffects)
                },
                showVisualizer = new SettingsCheckbox
                {
                    LabelText = "Show Visualizer",
                    Current = config.GetBindable<bool>(TauRulesetSettings.ShowVisualizer)
                },
                showKiai = new SettingsCheckbox
                {
                    LabelText = "Show Kiai effects",
                    Current = config.GetBindable<bool>(TauRulesetSettings.ShowKiai)
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
            };

            showEffects.Current.BindValueChanged(v =>
            {
                showVisualizer.Current.Disabled = !v.NewValue;
                showKiai.Current.Disabled = !v.NewValue;
            }, true);
        }
    }
}
