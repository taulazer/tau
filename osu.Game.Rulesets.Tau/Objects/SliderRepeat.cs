using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Tau.Objects
{
    public class SliderRepeat : Beat
    {
        public int RepeatIndex { get; set; }

        protected override HitWindows CreateHitWindows() => HitWindows.Empty;
    }
}
