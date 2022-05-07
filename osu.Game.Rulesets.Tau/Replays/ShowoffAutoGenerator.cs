using osu.Game.Beatmaps;
using osu.Game.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.UI;
using osuTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu.Game.Rulesets.Tau.Replays;

public class ShowoffAutoGenerator : AutoGenerator {
    public new Beatmap<TauHitObject> Beatmap => (Beatmap<TauHitObject>)base.Beatmap;
    Vector2 Centre = TauPlayfield.BaseSize / 2;
    const float CURSOR_DISTANCE = 250;

    public ShowoffAutoGenerator ( IBeatmap beatmap, IReadOnlyList<Mod> mods ) : base( beatmap ) {
        
    }

    public override Replay Generate () {
        var replay = new Replay();

        foreach ( var i in Beatmap.HitObjects ) {
            switch ( i ) {
                case Beat beat:
                    waitUntil( beat.StartTime - beat.TimePreempt );
                    moveTo( beat.Angle, beat.StartTime );
                    break;

                case Slider slider:
                    waitUntil( slider.StartTime - slider.TimePreempt );
                    foreach ( var node in slider.Path.Nodes ) {
                        moveTo( node.Angle, slider.StartTime + node.Time );
                    }
                    break;
            }
        }

        void waitUntil ( double time ) {

        }

        void moveTo ( float angle, double time ) {

        }

        return replay;
    }
}
