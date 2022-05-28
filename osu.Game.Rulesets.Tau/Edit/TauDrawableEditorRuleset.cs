using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Tau.Edit;

public class TauDrawableEditorRuleset : TauDrawableRuleset
{
    public TauDrawableEditorRuleset(Ruleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
        : base(ruleset, beatmap, mods)
    {
    }

    public override PlayfieldAdjustmentContainer CreatePlayfieldAdjustmentContainer() => new TauPlayfieldAdjustmentContainer(0.9f);
    protected override Playfield CreatePlayfield() => new TauEditorPlayfield();

    public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;

    private class TauEditorPlayfield : TauPlayfield
    {
        protected override GameplayCursorContainer CreateCursor() => new InvisibleTauCursor();

        protected override void LoadComplete()
        {
            base.LoadComplete();

            EffectsContainer.OnLoadComplete += _ =>
            {
                EffectsContainer.Hide();
            };
        }

        private class InvisibleTauCursor : TauCursor
        {
            protected override void LoadComplete()
            {
                base.LoadComplete();

                Alpha = 0;
                AlwaysPresent = true;
            }
        }
    }
}
