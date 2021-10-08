using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Tau.Edit
{
    public class DrawableTauEditorRuleset : DrawableTauRuleset
    {
        public DrawableTauEditorRuleset(Ruleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods)
            : base(ruleset, beatmap, mods)
        {
        }

        protected override Playfield CreatePlayfield() => new TauEditorPlayfield(Beatmap.BeatmapInfo.BaseDifficulty);

        public override PlayfieldAdjustmentContainer CreatePlayfieldAdjustmentContainer() => new TauPlayfieldAdjustmentContainer(0.8f);

        public class TauEditorPlayfield : TauPlayfield
        {
            public TauEditorPlayfield(BeatmapDifficulty difficulty)
                : base(difficulty)
            {
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                // All special effects should be turned off.
                KiaiVisualizer.Hide();
                KiaiExplosionContainer.Hide();
                SliderParticleEmitter.Hide();

                // Cursor should also be hidden.
                Cursor.Hide();
            }
        }
    }
}
