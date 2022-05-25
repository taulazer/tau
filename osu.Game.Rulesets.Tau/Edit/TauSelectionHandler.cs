﻿using System.Linq;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Screens.Edit.Compose.Components;

namespace osu.Game.Rulesets.Tau.Edit;

public class TauSelectionHandler : EditorSelectionHandler
{
    public override bool HandleMovement(MoveSelectionEvent<HitObject> moveEvent)
    {
        var anchor = moveEvent.Blueprint;

        var dragOrigin = anchor.ScreenSpaceSelectionPoint;
        var currentMousePos = anchor.ScreenSpaceSelectionPoint + moveEvent.ScreenSpaceDelta;
        var center = ScreenSpaceDrawQuad.Centre;

        float angleDelta = center.GetDegreesFromPosition(currentMousePos) - center.GetDegreesFromPosition(dragOrigin);

        if (!SelectedBlueprints.All(b => b.Item is TauHitObject)) return true;

        foreach (var b in SelectedBlueprints.Where(b => b.Item is IHasAngle))
        {
            var h = (AngledTauHitObject)b.Item;
            h.Angle += angleDelta;

            EditorBeatmap?.Update(h);
        }

        return true;
    }
}
