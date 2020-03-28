using osu.Framework.Graphics;
using osu.Framework.Graphics.Lines;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.UI;
using osuTK.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace osu.Game.Rulesets.Tau.Edit
{
    public class DrawableTauEditRuleset : DrawabletauRuleset
    {
        /// <summary>
        /// Hit objects are intentionally made to fade out at a constant slower rate than in gameplay.
        /// This allows a mapper to gain better historical context and use recent hitobjects as reference / snap points.
        /// </summary>
        private const double editor_hit_object_fade_out_extension = 500;

        public DrawableTauEditRuleset(TauRuleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods)
            : base(ruleset, beatmap, mods)
        {
        }

        public override DrawableHitObject<TauHitObject> CreateDrawableRepresentation(TauHitObject h)
            => base.CreateDrawableRepresentation(h)?.With(d => d.ApplyCustomUpdateState += updateState);

        private void updateState(DrawableHitObject hitObject, ArmedState state)
        {
            switch (state)
            {
                case ArmedState.Miss:
                    // Get the existing fade out transform
                    var existing = hitObject.Transforms.LastOrDefault(t => t.TargetMember == nameof(Alpha));
                    if (existing == null)
                        return;

                    hitObject.RemoveTransform(existing);

                    using (hitObject.BeginAbsoluteSequence(existing.StartTime))
                        hitObject.FadeOut(editor_hit_object_fade_out_extension).Expire();
                    break;
            }
        }
        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (e.ControlPressed & e.PressedKeys.Contains(Key.S))
            {
                //Save in temp file for now
                string directory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "@osu");

                Directory.CreateDirectory(directory);
                var path = System.IO.Path.Combine(directory, "tau.osu");

                //using (var sw = new StreamWriter(path))
                //{
                //    var encoder = new TauLegacyBeatmapEncoder();
                //    sw.WriteLine(encoder.Encode(new Beatmap
                //    {
                //        HitObjects = Beatmap.HitObjects.OfType<HitObject>().ToList()
                //    }));
                //}
                return true;
            }
            return base.OnKeyDown(e);
        }

        protected override Playfield CreatePlayfield() => new TauPlayfieldNoCursor();
        public class TauPlayfieldNoCursor : TauPlayfield
        {
            protected override GameplayCursorContainer CreateCursor() => null;
            public override bool ShowVisualizer => false;
        }
    }
}
