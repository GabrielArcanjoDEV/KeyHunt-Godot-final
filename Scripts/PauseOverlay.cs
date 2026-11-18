using Godot;

namespace KeyHunt;

public partial class PauseOverlay : CanvasLayer
{
    private bool paused;
    private Panel? pausePanel;
    private Button? pauseButton;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        pauseButton = new Button
        {
            Text = "Ⅱ",
            Position = new Vector2(1188, 14),
            Size = new Vector2(76, 48),
            FocusMode = Control.FocusModeEnum.None,
            TooltipText = "Pausar (Esc)"
        };
        StyleButton(pauseButton, new Color(0.85f, 0.57f, 0.0f));
        pauseButton.AddThemeFontSizeOverride("font_size", 25);
        pauseButton.Pressed += TogglePause;
        AddChild(pauseButton);

        BuildPausePanel();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Pressed && !key.Echo && key.Keycode == Key.Escape)
        {
            TogglePause();
            GetViewport().SetInputAsHandled();
        }
    }

    private void BuildPausePanel()
    {
        pausePanel = new Panel
        {
            Position = new Vector2(440, 165),
            Size = new Vector2(400, 390),
            Visible = false,
            ProcessMode = ProcessModeEnum.Always
        };
        var panelStyle = new StyleBoxFlat
        {
            BgColor = new Color(0.035f, 0.025f, 0.018f, 0.96f),
            BorderColor = new Color(0.85f, 0.57f, 0.0f),
            CornerRadiusTopLeft = 18,
            CornerRadiusTopRight = 18,
            CornerRadiusBottomLeft = 18,
            CornerRadiusBottomRight = 18
        };
        panelStyle.SetBorderWidthAll(4);
        pausePanel.AddThemeStyleboxOverride("panel", panelStyle);
        AddChild(pausePanel);

        var title = new Label
        {
            Text = "JOGO PAUSADO",
            Position = new Vector2(20, 24),
            Size = new Vector2(360, 48),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        title.AddThemeColorOverride("font_color", new Color(1.0f, 0.78f, 0.25f));
        title.AddThemeFontSizeOverride("font_size", 27);
        pausePanel.AddChild(title);

        AddPauseButton("CONTINUAR", 95, ResumeGame);
        AddPauseButton("REINICIAR", 165, () => ChangeScene("res://Scenes/Game.tscn"));
        AddPauseButton("VOLTAR AO MENU", 235, () => ChangeScene("res://Scenes/MainMenu.tscn"));
        AddPauseButton("SAIR DO JOGO", 305, () =>
        {
            GetTree().Paused = false;
            GetTree().Quit();
        });
    }

    private void AddPauseButton(string text, float y, System.Action action)
    {
        var button = new Button
        {
            Text = text,
            Position = new Vector2(55, y),
            Size = new Vector2(290, 52),
            FocusMode = Control.FocusModeEnum.None
        };
        StyleButton(button, new Color(0.85f, 0.57f, 0.0f));
        button.AddThemeFontSizeOverride("font_size", 19);
        button.Pressed += action;
        pausePanel!.AddChild(button);
    }

    private static void StyleButton(Button button, Color accent)
    {
        var normal = new StyleBoxFlat
        {
            BgColor = new Color(0, 0, 0, 0.88f),
            BorderColor = accent,
            CornerRadiusTopLeft = 12,
            CornerRadiusTopRight = 12,
            CornerRadiusBottomLeft = 12,
            CornerRadiusBottomRight = 12,
            ContentMarginLeft = 8,
            ContentMarginRight = 8,
            ContentMarginTop = 4,
            ContentMarginBottom = 4
        };
        normal.SetBorderWidthAll(3);
        var hover = (StyleBoxFlat)normal.Duplicate();
        hover.BgColor = new Color(0.25f, 0.18f, 0.08f, 0.98f);
        button.AddThemeStyleboxOverride("normal", normal);
        button.AddThemeStyleboxOverride("hover", hover);
        button.AddThemeStyleboxOverride("pressed", hover);
        button.AddThemeColorOverride("font_color", Colors.White);
    }

    private void TogglePause()
    {
        paused = !paused;
        pausePanel!.Visible = paused;
        pauseButton!.Text = paused ? "▶" : "Ⅱ";
        pauseButton.TooltipText = paused ? "Retomar" : "Pausar (Esc)";
        GetTree().Paused = paused;
    }

    private void ResumeGame()
    {
        paused = false;
        pausePanel!.Visible = false;
        pauseButton!.Text = "Ⅱ";
        pauseButton.TooltipText = "Pausar (Esc)";
        GetTree().Paused = false;
    }

    private void ChangeScene(string path)
    {
        GetTree().Paused = false;
        paused = false;
        SceneFlow.Go(path);
    }
}
