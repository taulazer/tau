using osu.Framework.Graphics;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModInverse : Mod, IApplicableToDrawableHitObject, IApplicableToDrawableRuleset<TauHitObject>
    {
        public override string Name => "Inverse";
        public override string Acronym => "IN";
        public override ModType Type => ModType.DifficultyIncrease;
        public override string Description => @"Hit objects will come outside of the playfield.";
        public override double ScoreMultiplier => 1.2;

        public void ApplyToDrawableHitObject(DrawableHitObject drawable)
        {
            drawable.ApplyCustomUpdateState += (drawableObject, state) =>
            {
                switch (drawableObject)
                {
                    case DrawableBeat beat:
                        var box = beat.Box;
                        var beatObject = beat.HitObject;

                        box.ClearTransforms(targetMember: "Y");

                        using (beat.BeginAbsoluteSequence(beatObject.StartTime, false))
                        {
                            box.MoveToY(-1);
                            box.MoveToY(-0.516f, beatObject.TimePreempt);
                        }

                        break;

                    case DrawableHardBeat hardBeat:
                        var hardBeatObject = hardBeat.HitObject;

                        hardBeat.ClearTransforms();

                        using (hardBeat.BeginAbsoluteSequence(hardBeatObject.StartTime - hardBeatObject.TimePreempt))
                        {
                            hardBeat.ResizeTo(2);
                            hardBeat.ResizeTo(1, hardBeatObject.TimePreempt);
                        }

                        break;

                    case DrawableSlider slider:
                        slider.Inversed = true;
                        slider.MaskingContainer.Masking = false;

                        break;
                }
            };
        }

        public void ApplyToDrawableRuleset(DrawableRuleset<TauHitObject> drawableRuleset)
        {
            var playfield = (TauPlayfield)drawableRuleset.Playfield;
            playfield.Inversed = true;
        }
    }
}
