using System;
using System.Diagnostics;
using System.Linq;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Beatmaps;
using osu.Game.Rulesets.Tau.Localisation;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Tau.Mods
{
    public partial class TauModLenience : Mod, IApplicableAfterBeatmapConversion, IApplicableToDrawableRuleset<TauHitObject>
    {
        public override string Name => "Lenience";
        public override LocalisableString Description => ModStrings.LenienceDescription;
        public override double ScoreMultiplier => 0.6;
        public override string Acronym => "LN";
        public override ModType Type => ModType.DifficultyReduction;
        public override Type[] IncompatibleMods => new[] { typeof(TauModStrict), typeof(TauModLite) };

        public void ApplyToBeatmap(IBeatmap beatmap)
        {
            var tauBeatmap = (TauBeatmap)beatmap;
            if (tauBeatmap.HitObjects.Count == 0) return;

            var hitObjects = tauBeatmap.HitObjects.Select(ho =>
            {
                if (ho is not HardBeat hardBeat)
                    return ho;

                var newHardBeat = new LenientHardBeats(hardBeat);
                return newHardBeat;
            }).ToList();

            tauBeatmap.HitObjects = hitObjects;
        }

        public void ApplyToDrawableRuleset(DrawableRuleset<TauHitObject> drawableRuleset)
        {
            drawableRuleset.Playfield.RegisterPool<LenientHardBeats, DrawableLenientHardBeats>(10, 100);
        }

        private class LenientHardBeats : HardBeat
        {
            public LenientHardBeats(HardBeat original)
            {
                StartTime = original.StartTime;
                Samples = original.Samples;
                NewCombo = original.NewCombo;
                ComboOffset = original.ComboOffset;
            }
        }

        private partial class DrawableLenientHardBeats : DrawableHardBeat
        {
            protected override TauAction[] Actions { get; } =
            {
                TauAction.HardButton1,
                TauAction.HardButton2,
                TauAction.LeftButton,
                TauAction.RightButton
            };

            private TauAction pressedAction;

            public DrawableLenientHardBeats()
            {
            }

            protected override void CheckForResult(bool userTriggered, double timeOffset)
            {
                Debug.Assert(HitObject.HitWindows != null);

                if (!userTriggered)
                {
                    if (!HitObject.HitWindows.CanBeHit(timeOffset))
                        ApplyResult(r => r.Type = HitResult.Miss);

                    return;
                }

                if (!CheckForValidation())
                    return;

                var result = HitObject.HitWindows.ResultFor(timeOffset);

                if (result == HitResult.None)
                    return;

                if (pressedAction is TauAction.LeftButton or TauAction.RightButton)
                {
                    if (result != HitResult.Miss)
                        result = HitResult.Ok;
                }

                ApplyResult(r =>
                {
                    r.Type = result;
                    ApplyCustomResult(r);
                });
            }

            public override bool OnPressed(KeyBindingPressEvent<TauAction> e)
            {
                if (Judged)
                    return false;

                if (!Actions.Contains(e.Action))
                    return false;

                pressedAction = e.Action;

                return UpdateResult(true);
            }
        }
    }
}
