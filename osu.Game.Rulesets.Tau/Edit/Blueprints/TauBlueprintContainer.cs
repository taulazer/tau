using System.Collections.Generic;
using System.Linq;
using osu.Framework.Input;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Screens.Edit.Compose.Components;
using osuTK;

namespace osu.Game.Rulesets.Tau.Edit.Blueprints;

public class TauBlueprintContainer : ComposeBlueprintContainer
{
    public TauBlueprintContainer(HitObjectComposer composer)
        : base(composer)
    {
    }

    protected override SelectionHandler<HitObject> CreateSelectionHandler()
        => new TauSelectionHandler();

    private InputManager inputManager;
    internal InputManager InputManager => inputManager ??= GetContainingInputManager();

    private Vector2 currentMousePosition => InputManager.CurrentState.Mouse.Position;

    protected override IEnumerable<SelectionBlueprint<HitObject>> SortForMovement(IReadOnlyList<SelectionBlueprint<HitObject>> blueprints)
        => blueprints.OrderBy(b => Vector2.DistanceSquared(b.ScreenSpaceSelectionPoint, currentMousePosition));

    public override HitObjectSelectionBlueprint CreateHitObjectBlueprintFor(HitObject hitObject)
    {
        switch (hitObject)
        {
            case Beat beat:
                return new BeatSelectionBlueprint(beat);
        }

        return base.CreateHitObjectBlueprintFor(hitObject);
    }
}

