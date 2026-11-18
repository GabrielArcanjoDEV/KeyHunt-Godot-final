using Godot;
using System;
using System.Collections.Generic;

namespace KeyHunt;

public partial class Enemy : CharacterBody2D
{
    private enum EnemyState { Idle, Chasing, Attacking }
    private EnemyState state = EnemyState.Idle;
    private Sprite2D sprite = null!;
    private readonly Dictionary<string, Texture2D[]> animations = new();
    private string activeAnimation = "Idle";
    private int frame = 0;
    private double frameTimer = 0;
    private double damageCooldown = 0;
    private bool damageDone = false;
    private bool facingRight = true;
    private Vector2 spawn;
    public int Health { get; private set; } = 3;

    public override void _Ready()
    {
        spawn = GlobalPosition;
        LoadAnimation("Idle", "Idle", 18);
        LoadAnimation("Running", "Running", 12);
        LoadAnimation("Kicking", "Kicking", 12);
        sprite = new Sprite2D { Centered = true };
        AddChild(sprite);
        SetAnimation("Idle");
        AddChild(new CollisionShape2D { Shape = new RectangleShape2D { Size = new Vector2(96,96) } });
        CollisionLayer = 2;
        CollisionMask = 1;
    }

    private void LoadAnimation(string key, string folder, int count)
    {
        var frames = new List<Texture2D>();
        for (int i = 0; i < count; i++)
        {
            string path = $"res://Assets/imagens/Golem_3/PNG/{folder}/0_Golem_{folder}_{i:000}.png";
            var tex = GD.Load<Texture2D>(path);
            if (tex != null) frames.Add(tex);
        }
        animations[key] = frames.ToArray();
    }

    private void SetAnimation(string name)
    {
        if (activeAnimation == name) return;
        activeAnimation = name;
        frame = 0;
        frameTimer = 0;
    }

    public override void _PhysicsProcess(double delta)
    {
        damageCooldown = Math.Max(0, damageCooldown - delta);
        var player = GetTree().GetFirstNodeInGroup("player") as Player;
        if (player == null || !player.IsAlive)
        {
            state = EnemyState.Idle;
            Velocity = Vector2.Zero;
            SetAnimation("Idle");
        }
        else
        {
            float distance = GlobalPosition.DistanceTo(player.GlobalPosition);
            if (distance <= 80)
            {
                state = EnemyState.Attacking;
                Velocity = Vector2.Zero;
                SetAnimation("Kicking");
            }
            else if (distance <= 500)
            {
                state = EnemyState.Chasing;
                damageDone = false;
                Velocity = GlobalPosition.DirectionTo(player.GlobalPosition) * 125f;
                facingRight = Velocity.X >= 0;
                SetAnimation("Running");
            }
            else
            {
                state = EnemyState.Idle;
                damageDone = false;
                Velocity = Vector2.Zero;
                SetAnimation("Idle");
            }
            MoveAndSlide();
            if (state == EnemyState.Attacking && damageCooldown <= 0 && !damageDone && player.IsAlive)
            {
                player.TakeDamage(1);
                damageDone = true;
                damageCooldown = 1.0;
            }
        }

        frameTimer += delta;
        double delay = activeAnimation == "Kicking" ? 0.08 : activeAnimation == "Running" ? 0.12 : 0.16;
        if (frameTimer >= delay)
        {
            frameTimer = 0;
            var list = animations.GetValueOrDefault(activeAnimation);
            if (list != null && list.Length > 0)
            {
                frame = (frame + 1) % list.Length;
                sprite.Texture = list[frame];
                sprite.FlipH = !facingRight;
                sprite.Scale = new Vector2(1.0f, 1.0f);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            Health = 3;
            GlobalPosition = spawn;
        }
    }

    public override void _Draw()
    {
        DrawRect(new Rect2(-30,-76,60,6), Colors.White, false, 1);
        DrawRect(new Rect2(-30,-76,60 * (Health / 3f),6), Colors.Red, true);
    }
}
