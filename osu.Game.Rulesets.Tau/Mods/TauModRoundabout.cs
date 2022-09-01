using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Beatmaps;
using System;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Tau.Localisation;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModRoundabout : Mod, IApplicableToBeatmapConverter
    {
        public override string Name => "Roundabout";
        public override string Acronym => "RB";
        public override LocalisableString Description => ModStrings.RoundaboutDescription;
        public override double ScoreMultiplier => 1;
        public override ModType Type => ModType.Fun;
        public override bool UserPlayable => true;
        public override IconUsage? Icon => FontAwesome.Solid.Redo;
        public override bool HasImplementation => true;

        [SettingSource(typeof(ModStrings), nameof(ModStrings.RoundaboutDirectionName))]
        public Bindable<RotationDirection> Direction { get; } = new();

        public override string SettingDescription => $"{Direction.Value}";

        public override void ResetSettingsToDefaults()
        {
            Direction.SetDefault();
            base.ResetSettingsToDefaults();
        }

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            ((TauBeatmapConverter)beatmapConverter).LockedDirection = Direction.Value;
        }

        public override Type[] IncompatibleMods => new[] { typeof(TauModAutoplay) };
    }
}
