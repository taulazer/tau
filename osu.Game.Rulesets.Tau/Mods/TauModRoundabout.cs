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
using System.Collections.Generic;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Tau.UI;

namespace osu.Game.Rulesets.Tau.Mods
{
    public partial class TauModRoundabout : Mod, IApplicableToBeatmapConverter
    {
        public override string Name => "Roundabout";
        public override string Acronym => "RB";
        public override LocalisableString Description => ModStrings.RoundaboutDescription;
        public override double ScoreMultiplier => 1;
        public override ModType Type => ModType.Fun;
        public override bool UserPlayable => true;
        public override IconUsage? Icon => FontAwesome.Solid.Redo;
        public override bool HasImplementation => true;

        public override Type[] IncompatibleMods => [typeof(TauModAutoplay)];

        [SettingSource(typeof(ModStrings), nameof(ModStrings.RoundaboutDirectionName))]
        public Bindable<RotationDirection> Direction { get; } = new();

        public override IEnumerable<(LocalisableString setting, LocalisableString value)> SettingDescription
        {
            get
            {
                yield return (ModStrings.RoundaboutDirectionName, $"{Direction.Value:N1}");
            }
        }

        public override void ResetSettingsToDefaults()
        {
            Direction.SetDefault();
            base.ResetSettingsToDefaults();
        }

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            ((TauBeatmapConverter)beatmapConverter).LockedDirection = Direction.Value;
        }

        public partial class RoundaboutTauCursor : TauCursor
        {
            private float lastLockedRotation;
            private RotationDirection rotationLock;

            protected override void LoadComplete()
            {
                base.LoadComplete();

                if (Mods.GetMod(out TauModRoundabout mod))
                    rotationLock = mod.Direction.Value;
            }

            protected override bool OnMouseMove(MouseMoveEvent e)
            {
                var prev = lastLockedRotation;
                var nextAngle = ScreenSpaceDrawQuad.Centre.GetDegreesFromPosition(e.ScreenSpaceMousePosition);
                var diff = Extensions.GetDeltaAngle(nextAngle, prev);

                switch (rotationLock)
                {
                    case RotationDirection.Clockwise:
                        lastLockedRotation = diff > 0 ? nextAngle : prev;
                        Rotation = diff < 0 ? (lastLockedRotation - diff.LimitEase(40)) : lastLockedRotation;
                        break;

                    case RotationDirection.Counterclockwise:
                        lastLockedRotation = diff < 0 ? nextAngle : prev;
                        Rotation = diff > 0 ? (lastLockedRotation + diff.LimitEase(40)) : lastLockedRotation;
                        break;
                }

                Rotation = Rotation.Normalize();
                ActiveCursor.Position = ToLocalSpace(e.ScreenSpaceMousePosition);
                return false;
            }
        }
    }
}
