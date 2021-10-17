using System;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModRotate : Mod, IUpdatableByPlayfield, IApplicableToBeatmap, IApplicableToDrawableRuleset<TauHitObject>
    {
        public override string Name => "Rotate";
        public override string Acronym => "RT";
        public override string Description => "Around and around and around...";
        public override IconUsage? Icon => FontAwesome.Solid.RedoAlt;
        public override ModType Type => ModType.Fun;
        public override double ScoreMultiplier => 1.00;

        [SettingSource("Rotation direction")]
        public Bindable<Direction> Direction { get; } = new Bindable<Direction>();

        [SettingSource("Initial rate", "The initial rate this will rotate by.")]
        public BindableNumber<float> Rate { get; } = new BindableFloat
        {
            MinValue = 1f,
            MaxValue = 10f,
            Default = 3f,
            Value = 3f,
            Precision = 0.1f
        };

        [SettingSource("Final rate", "The final rate this will rotate by.")]
        public BindableNumber<float> FinalRate { get; } = new BindableFloat
        {
            MinValue = 1f,
            MaxValue = 10f,
            Default = 3f,
            Value = 3f,
            Precision = 0.1f
        };

        private double startTime;
        private double endTime;
        private readonly BindableFloat rotation = new BindableFloat();

        public void Update(Playfield playfield)
        {
            var field = (TauPlayfield)playfield;
            var currentTime = Math.Max(playfield.Time.Current, 0);
            var interpolated = Interpolation.ValueAt(currentTime, Rate.Value, FinalRate.Value, startTime, endTime);

            rotation.Value = (float)(currentTime / (interpolated * 1000) * 360 % 360) * (Direction.Value == Mods.Direction.Clockwise ? 1 : -1);
            field.Rotation = rotation.Value;
        }

        public void ApplyToBeatmap(IBeatmap beatmap)
        {
            startTime = beatmap.HitObjects.FirstOrDefault()?.StartTime ?? 0;
            endTime = beatmap.HitObjects.LastOrDefault()?.GetEndTime() ?? 0;
        }

        public void ApplyToDrawableRuleset(DrawableRuleset<TauHitObject> drawableRuleset)
        {
            var ruleset = (DrawableTauRuleset)drawableRuleset;
            ruleset.RotationOffset.BindTo(rotation);
        }
    }

    public enum Direction
    {
        Clockwise,
        CounterClockwise
    }
}
