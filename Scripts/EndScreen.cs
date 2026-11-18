using Godot;

namespace KeyHunt;

public partial class EndScreen : Control
{
    [Export] public string BackgroundPath { get; set; } = "res://Assets/imagens/TelaDerrota.png";
    [Export] public string MusicPath { get; set; } = "res://Assets/sons/musicaDerrota.wav";
    [Export] public int ButtonY { get; set; } = 632;

    public override void _Ready()
    {
        SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        AudioService.Instance?.PlayOnce(MusicPath);
        AddChild(new TextureRect
        {
            Texture = GD.Load<Texture2D>(BackgroundPath),
            AnchorLeft = 0, AnchorTop = 0, AnchorRight = 1, AnchorBottom = 1,
            OffsetLeft = 0, OffsetTop = 0, OffsetRight = 0, OffsetBottom = 0,
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.Scale
        });
        var button = new Button
        {
            Text = "VOLTAR",
            Position = new Vector2(516, ButtonY),
            Size = new Vector2(247,48),
            FocusMode = Control.FocusModeEnum.None
        };
        MainMenu.StyleButton(button, new Color(0.4f,0.2f,0.6f));
        button.Pressed += () => SceneFlow.Go("res://Scenes/MainMenu.tscn");
        AddChild(button);
    }
}
