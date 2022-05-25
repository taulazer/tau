using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Tau.Edit;

public class TauDrawableEditorRuleset : TauDrawableRuleset
{
    public TauDrawableEditorRuleset(TauRuleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
        : base(ruleset, beatmap, mods)
    {
    }

    public override PlayfieldAdjustmentContainer CreatePlayfieldAdjustmentContainer() => new TauPlayfieldAdjustmentContainer { Size = new Vector2(0.8f) };
    protected override Playfield CreatePlayfield() => new TauEditorPlayfield();

    private class TauEditorPlayfield : TauPlayfield
    {
        protected override GameplayCursorContainer CreateCursor() => null;

        [BackgroundDependencyLoader]
        private void load()
        {
            // All special effects should be turned off.
            // These are all found within the effects container which is currently private.
            // KiaiVisualizer.Hide();
            // KiaiExplosionContainer.Hide();
            // SliderParticleEmitter.Hide();
        }
    }
}
