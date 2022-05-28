using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Rulesets.Tau.Objects;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Edit.Blueprints.HitObjects;

public class BeatBlueprintPiece : BlueprintPiece<Beat>
{
    public BindableFloat NoteSize = new(16f);

    public BeatBlueprintPiece()
    {
        Origin = Anchor.Centre;
        Size = new Vector2(NoteSize.Default * 1.25f);
        Anchor = Anchor.Centre;
        RelativePositionAxes = Axes.Both;

        InternalChild = new Container
        {
            BorderThickness = 10,
            BorderColour = Color4.Yellow,
            RelativeSizeAxes = Axes.Both,
            Masking = true,
            Children = new Drawable[]
            {
                new Box
                {
                    AlwaysPresent = true,
                    RelativeSizeAxes = Axes.Both
                }
            }
        };
    }

    [BackgroundDependencyLoader]
    private void load(OsuColour colours)
    {
        Colour = colours.Yellow;
    }
}
