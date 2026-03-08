
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModBarrelRoll : ModBarrelRoll<TauHitObject>{
        public override void Update(Playfield playfield)
        {
            base.Update(playfield);
            TauCursor cursor = (TauCursor)playfield.Cursor;
            cursor.ActiveCursor.Rotation =+ 167f;
        }
    }
}
