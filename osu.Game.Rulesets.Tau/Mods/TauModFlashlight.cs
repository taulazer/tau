using System;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Framework.Utils;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Objects;
using osuTK;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModFlashlight : ModFlashlight<TauHitObject>
    {
        public override double ScoreMultiplier => 1.12;

        public override float DefaultFlashlightSize => 180;

        [SettingSource("Flashlight size", "Multiplier applied to the default flashlight size.")]
        public override BindableFloat SizeMultiplier { get; } = new BindableFloat
        {
            MinValue = 0.5f,
            MaxValue = 1.5f,
            Default = 1f,
            Value = 1f,
            Precision = 0.1f
        };

        [SettingSource("Change size based on combo", "Decrease the flashlight size as combo increases.")]
        public override BindableBool ComboBasedSize { get; } = new BindableBool
        {
            Default = true,
            Value = true
        };

        protected override Flashlight CreateFlashlight() => new TauFlashlight(this);

        private class TauFlashlight : Flashlight, IRequireHighFrequencyMousePosition
        {

            public TauFlashlight(TauModFlashlight modFlashlight) : base(modFlashlight)
            {
                FlashlightSize = new Vector2(0, GetSizeFor(0));
            }

            protected override bool OnMouseMove(MouseMoveEvent e)
            {
                const double follow_delay = 120;

                var position = FlashlightPosition;
                var destination = e.MousePosition;

                FlashlightPosition = Interpolation.ValueAt(
                    Math.Clamp(Clock.ElapsedFrameTime, 0, follow_delay), position, destination, 0, follow_delay, Easing.Out);

                return base.OnMouseMove(e);
            }

            protected override void OnComboChange(ValueChangedEvent<int> e)
            {
                this.TransformTo(nameof(FlashlightSize), new Vector2(0, GetSizeFor(e.NewValue)), FLASHLIGHT_FADE_DURATION);
            }

            protected override string FragmentShader => "CircularFlashlight";
        }
    }
}
