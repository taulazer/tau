// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Audio;
using osuTK;

namespace osu.Game.Rulesets.Tau.Beatmaps
{
    public class TauBeatmapConverter : BeatmapConverter<TauHitObject>
    {
        // todo: Check for conversion types that should be supported (ie. Beatmap.HitObjects.Any(h => h is IHasXPosition))
        // https://github.com/ppy/osu/tree/master/osu.Game/Rulesets/Objects/Types
        public override bool CanConvert() => Beatmap.HitObjects.All(h => h is IHasPosition);

        public TauBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
        }

        protected override IEnumerable<TauHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap)
        {
            Vector2 position = ((IHasPosition)original).Position;
            var comboData = original as IHasCombo;
            bool isFeign = (original is IHasPathWithRepeats tmp ? tmp.NodeSamples[0] : original.Samples).Any(s => s.Name == HitSampleInfo.HIT_WHISTLE);

            switch (original)
            {
                default:
                    return new Beat
                    {
                        Samples = original is IHasPathWithRepeats curve ? curve.NodeSamples[0] : original.Samples,
                        Feign = isFeign,
                        StartTime = original.StartTime,
                        Angle = position.GetHitObjectAngle(),
                        NewCombo = comboData?.NewCombo ?? false,
                        ComboOffset = comboData?.ComboOffset ?? 0,
                    }.Yield();
            }
        }

        protected override Beatmap<TauHitObject> CreateBeatmap() => new TauBeatmap();
    }
}
