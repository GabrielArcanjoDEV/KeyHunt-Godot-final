using Godot;

namespace KeyHunt;

public partial class OptionsMenu : Control
{
    private Label status = null!;
    private int volume = 3;
    private bool enabled = true;

    public override void _Ready()
    {
        SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        AudioService.Instance?.PlayLoop("res://Assets/sons/musicaJogo.ogg");
        AddBackground();
        var title = MakeLabel("MÚSICA", new Vector2(0, 205), 1280, 44);
        AddChild(title);
        var on = MakeButton("ON", new Vector2(505, 272), new Vector2(123, 48), () => { enabled = true; AudioService.Instance?.SetEnabled(true); UpdateStatus(); });
        var off = MakeButton("OFF", new Vector2(652, 272), new Vector2(123, 48), () => { enabled = false; AudioService.Instance?.SetEnabled(false); UpdateStatus(); });
        AddChild(on); AddChild(off);
        AddChild(MakeLabel("VOLUME", new Vector2(0, 365), 1280, 44));
        for (int i = 0; i < 5; i++)
        {
            int level = i + 1;
            var bar = MakeButton(" ", new Vector2(474 + i * 54, 424), new Vector2(40, 52),
                () => { volume = level; AudioService.Instance?.SetVolume(volume); UpdateStatus(); });
            AddChild(bar);
        }
        status = MakeLabel("Volume: 3/5 | Música: ON", new Vector2(0, 495), 1280, 30);
        AddChild(status);
        AddChild(MakeButton("VOLTAR", new Vector2(507, 576), new Vector2(264, 72),
            () => SceneFlow.Go("res://Scenes/MainMenu.tscn")));
        UpdateStatus();
    }

    private void AddBackground()
    {
        AddChild(new TextureRect
        {
            Texture = GD.Load<Texture2D>("res://Assets/imagens/TelaOpcoes.png"),
            AnchorLeft = 0, AnchorTop = 0, AnchorRight = 1, AnchorBottom = 1,
            OffsetLeft = 0, OffsetTop = 0, OffsetRight = 0, OffsetBottom = 0,
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.Scale
        });
    }

    private static Label MakeLabel(string text, Vector2 pos, float width, float height)
    {
        var label = new Label
        {
            Text = text, Position = pos, Size = new Vector2(width, height),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        label.AddThemeFontSizeOverride("font_size", 26);
        label.AddThemeColorOverride("font_color", new Color(0.24f, 0.12f, 0.04f));
        return label;
    }

    private static Button MakeButton(string text, Vector2 pos, Vector2 size, System.Action action)
    {
        var b = new Button { Text = text, Position = pos, Size = size, FocusMode = Control.FocusModeEnum.None };
        MainMenu.StyleButton(b, new Color(0.55f, 0.32f, 0.12f));
        b.Pressed += action;
        return b;
    }

    private void UpdateStatus()
    {
        if (status != null) status.Text = $"Volume: {volume}/5 | Música: {(enabled ? "ON" : "OFF")}";
    }
}
