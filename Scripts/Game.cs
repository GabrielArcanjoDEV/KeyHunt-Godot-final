using Godot;
using System.Collections.Generic;

namespace KeyHunt;

public partial class Game : Node2D
{
    private Player player = null!;
    private readonly List<Enemy> enemies = new();
    private readonly List<(Vector2I cell, Sprite2D sprite)> keys = new();
    private Label keyLabel = null!;
    private Label healthLabel = null!;
    private ColorRect darkness = null!;
    private bool ending = false;
    private double attackCooldown = 0;

    public override void _Ready()
    {
        var world = new WorldMap { Name = "WorldMap" };
        AddChild(world);

        player = new Player { Name = "Player", Position = CellCenter(MapData.PlayerSpawn.Row, MapData.PlayerSpawn.Col) };
        player.AddToGroup("player");
        AddChild(player);

        var camera = new Camera2D { PositionSmoothingEnabled = true, PositionSmoothingSpeed = 6f };
        player.AddChild(camera);
        camera.MakeCurrent();

        foreach (var spawn in MapData.EnemySpawns)
        {
            var enemy = new Enemy { Position = CellCenter(spawn.Row, spawn.Col) };
            AddChild(enemy);
            enemies.Add(enemy);
        }

        foreach (var spawn in MapData.KeySpawns)
        {
            var key = new Sprite2D
            {
                Texture = GD.Load<Texture2D>("res://Assets/tiles/chavemapa.png"),
                Position = CellCenter(spawn.Row, spawn.Col),
                Scale = new Vector2(0.75f, 0.75f)
            };
            AddChild(key);
            keys.Add((new Vector2I(spawn.Row, spawn.Col), key));
        }

        BuildHud();
        AddDarkness();
        AddChild(new PauseOverlay { Name = "PauseOverlay" });
        AudioService.Instance?.PlayLoop("res://Assets/sons/musicaJogo.ogg");
    }

    private static Vector2 CellCenter(int row, int col) =>
        new(col * MapData.TileSize + MapData.TileSize / 2f, row * MapData.TileSize + MapData.TileSize / 2f);

    private void BuildHud()
    {
        var hudLayer = new CanvasLayer { Name = "HUDLayer" };
        AddChild(hudLayer);
        var hud = new Panel
        {
            Position = new Vector2(10,10),
            Size = new Vector2(355,50),
            Modulate = new Color(1,1,1,0.9f)
        };
        hudLayer.AddChild(hud);
        keyLabel = new Label
        {
            Position = new Vector2(18,12), Size = new Vector2(100,30),
            Text = "Chaves: 0 / 7"
        };
        healthLabel = new Label
        {
            Position = new Vector2(145,12), Size = new Vector2(200,30),
            Text = "Vida: 5/5"
        };
        hud.AddChild(keyLabel);
        hud.AddChild(healthLabel);
        keyLabel.AddThemeColorOverride("font_color", Colors.Yellow);
        healthLabel.AddThemeColorOverride("font_color", Colors.White);
        keyLabel.AddThemeFontSizeOverride("font_size", 18);
        healthLabel.AddThemeFontSizeOverride("font_size", 16);
    }

    private void AddDarkness()
    {
        // Overlay escuro translúcido; a luz radial é simplificada nesta primeira migração.
        darkness = new ColorRect
        {
            Color = new Color(0,0,0,0.38f),
            Position = new Vector2(-5000,-5000),
            Size = new Vector2(12000,12000),
            MouseFilter = Control.MouseFilterEnum.Ignore,
            ZIndex = 10
        };
        // A camada é adicionada abaixo da interface; a visibilidade pode ser ajustada depois.
        AddChild(darkness);
        darkness.Visible = false; // evita escurecer o HUD; será substituída por iluminação radial refinada.
    }

    public override void _Process(double delta)
    {
        if (ending || player == null) return;

        foreach (var item in keys)
        {
            if (item.sprite.Visible && player.GlobalPosition.DistanceTo(item.sprite.GlobalPosition) < 70)
            {
                item.sprite.Visible = false;
                player.CollectKey();
            }
        }

        attackCooldown += delta;
        if (player.IsAttacking && player.CanAttack())
        {
            foreach (var enemy in enemies)
                if (player.GlobalPosition.DistanceTo(enemy.GlobalPosition) <= 100)
                    enemy.TakeDamage(1);
        }

        keyLabel.Text = $"Chaves: {player.KeysCollected} / 7";
        healthLabel.Text = $"Vida: {player.Health}/5";

        if (!player.IsAlive)
        {
            ending = true;
            AudioService.Instance?.Stop();
            SceneFlow.Go("res://Scenes/GameOver.tscn");
        }
        else if (player.KeysCollected >= 7)
        {
            ending = true;
            AudioService.Instance?.Stop();
            SceneFlow.Go("res://Scenes/Victory.tscn");
        }
    }
}
