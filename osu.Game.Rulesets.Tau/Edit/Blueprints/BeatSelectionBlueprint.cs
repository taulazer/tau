﻿using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Tau.Edit.Blueprints.HitObjects;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Edit.Blueprints;

public class BeatSelectionBlueprint : TauSelectionBlueprint<Beat>
{
    protected new DrawableBeat DrawableObject => (DrawableBeat)base.DrawableObject;
    protected readonly BeatBlueprintPiece SelectionPiece;
    protected readonly Box Distance;

    public BeatSelectionBlueprint(Beat hitObject)
        : base(hitObject)
    {
        InternalChildren = new Drawable[]
        {
            SelectionPiece = new BeatBlueprintPiece(),
            Distance = new Box
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.Y,
                RelativePositionAxes = Axes.Both,
                Height = .5f,
                Width = 2.5f,
                Colour = Color4.Yellow.Opacity(.5f),
                EdgeSmoothness = Vector2.One
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        SelectionPiece.Rotation = Distance.Rotation = DrawableObject.Rotation;
        SelectionPiece.Position = Extensions.FromPolarCoordinates(-DrawableObject.DrawableBox.Y, DrawableObject.Rotation);

        Distance.Height = -DrawableObject.DrawableBox.Y;
    }

    public override Vector2 ScreenSpaceSelectionPoint => DrawableObject.DrawableBox.ScreenSpaceDrawQuad.Centre;

    public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => SelectionPiece.ReceivePositionalInputAt(screenSpacePos);

    public override Quad SelectionQuad => DrawableObject.DrawableBox.ScreenSpaceDrawQuad.AABB.Inflate(5);
}
