using System;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModRotate : Mod, IUpdatableByPlayfield, IApplicableToBeatmap
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

        public void Update(Playfield playfield)
        {
            var field = (TauPlayfield)playfield;
            var currentTime = Math.Max(playfield.Time.Current - startTime, 0);
            var progress = currentTime / endTime;
            var interpolated = Interpolation.ValueAt(progress, Rate.Value, FinalRate.Value, 0.0, 1.0);

            field.Rotation = (float)(currentTime / (interpolated * 1000) * 360 % 360) * (Direction.Value == Mods.Direction.Clockwise ? 1 : -1);
            field.Cursor.Rotation = -field.Rotation;
        }

        public void ApplyToBeatmap(IBeatmap beatmap)
        {
            startTime = beatmap.HitObjects.FirstOrDefault()?.StartTime ?? 0;
            endTime = beatmap.HitObjects.LastOrDefault()?.GetEndTime() ?? 0;
        }
    }

    public enum Direction
    {
        Clockwise,
        CounterClockwise
    }
}
