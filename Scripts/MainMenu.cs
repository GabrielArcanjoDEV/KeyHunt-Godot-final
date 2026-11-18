using Godot;

namespace KeyHunt;

public partial class MainMenu : Control
{
    public override void _Ready()
    {
        SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        GD.Print("KEY HUNT: MENU INICIADO!");

        AudioService.Instance?.PlayLoop("res://Assets/sons/musicaJogo.ogg");

        GD.Print("KEY HUNT: AUDIO CARREGADO!");

        AddBackground("res://Assets/imagens/TelaInicio.png");

        GD.Print("KEY HUNT: IMAGEM DO MENU CARREGADA!");

        AddMenuButton("NOVO JOGO", new Vector2(189, 400), () => SceneFlow.Go("res://Scenes/Game.tscn"));
        AddMenuButton("OPÇÕES", new Vector2(189, 464), () => SceneFlow.Go("res://Scenes/Options.tscn"));
        AddMenuButton("SAIR", new Vector2(189, 528), () => GetTree().Quit());

        GD.Print("KEY HUNT: MENU FINALIZADO!");
    }

    private void AddBackground(string path)
    {
        var bg = new TextureRect
        {
            Texture = GD.Load<Texture2D>(path),
            AnchorLeft = 0,
            AnchorTop = 0,
            AnchorRight = 1,
            AnchorBottom = 1,
            OffsetLeft = 0,
            OffsetTop = 0,
            OffsetRight = 0,
            OffsetBottom = 0,
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.Scale
        };
        AddChild(bg);
    }

    private void AddMenuButton(string label, Vector2 pos, System.Action action)
    {
        var button = new Button
        {
            Text = label,
            Position = pos,
            Size = new Vector2(247, 48),
            FocusMode = Control.FocusModeEnum.None
        };
        StyleButton(button, new Color(0.85f, 0.57f, 0.0f));
        button.Pressed += action;
        AddChild(button);
    }

    internal static void StyleButton(Button button, Color border)
    {
        var normal = new StyleBoxFlat
        {
            BgColor = new Color(0, 0, 0, 0.78f),
            BorderColor = border,
            CornerRadiusTopLeft = 16,
            CornerRadiusTopRight = 16,
            CornerRadiusBottomLeft = 16,
            CornerRadiusBottomRight = 16,
            ContentMarginLeft = 8,
            ContentMarginRight = 8,
            ContentMarginTop = 4,
            ContentMarginBottom = 4
        };
        normal.SetBorderWidthAll(3);
        var hover = (StyleBoxFlat)normal.Duplicate();
        hover.BgColor = new Color(0.25f, 0.18f, 0.08f, 0.95f);
        button.AddThemeStyleboxOverride("normal", normal);
        button.AddThemeStyleboxOverride("hover", hover);
        button.AddThemeStyleboxOverride("pressed", hover);
        button.AddThemeColorOverride("font_color", Colors.White);
        button.AddThemeFontSizeOverride("font_size", 22);
    }
}
