using Godot;

namespace KeyHunt;

public partial class Player : CharacterBody2D
{
    [Export] public float Speed { get; set; } = 380f;
    public int Health { get; private set; } = 5;
    public int KeysCollected { get; private set; } = 0;
    public bool IsAlive => Health > 0;
    public bool IsAttacking { get; private set; }

    private Sprite2D sprite = null!;
    private Texture2D sheet = null!;
    private int directionRow = 0; // down, left, right, up
    private int frame = 0;
    private double animationTimer = 0;
    private double attackTimer = 0;
    private const float FrameDelay = 0.10f;
    private const int FrameW = 64, FrameH = 64;

    public override void _Ready()
    {
        sheet = GD.Load<Texture2D>("res://Assets/imagens/Jogador.png");
        sprite = new Sprite2D { Texture = sheet, Centered = true, Scale = new Vector2(2,2) };
        AddChild(sprite);
        UpdateFrame();
        var shape = new CollisionShape2D
        {
            Shape = new RectangleShape2D { Size = new Vector2(68,68) }
        };
        AddChild(shape);
        CollisionLayer = 1;
        CollisionMask = 1;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsAlive) return;
        Vector2 input = Vector2.Zero;
        if (Input.IsKeyPressed(Key.W) || Input.IsKeyPressed(Key.Up)) input.Y -= 1;
        if (Input.IsKeyPressed(Key.S) || Input.IsKeyPressed(Key.Down)) input.Y += 1;
        if (Input.IsKeyPressed(Key.A) || Input.IsKeyPressed(Key.Left)) input.X -= 1;
        if (Input.IsKeyPressed(Key.D) || Input.IsKeyPressed(Key.Right)) input.X += 1;
        input = input.Normalized();
        if (input.X < 0) directionRow = 1;
        else if (input.X > 0) directionRow = 2;
        else if (input.Y < 0) directionRow = 3;
        else if (input.Y > 0) directionRow = 0;

        Velocity = input * Speed;
        MoveAndSlide();

        IsAttacking = Input.IsKeyPressed(Key.Space);
        attackTimer += delta;
        if (input.LengthSquared() > 0)
        {
            animationTimer += delta;
            if (animationTimer >= FrameDelay)
            {
                animationTimer -= FrameDelay;
                frame = (frame + 1) % 8;
            }
        }
        else
        {
            frame = 0;
            animationTimer = 0;
        }
        UpdateFrame();
    }

    private void UpdateFrame()
    {
        if (sprite == null || sheet == null) return;
        sprite.RegionEnabled = true;
        sprite.RegionRect = new Rect2(frame * FrameW, directionRow * FrameH, FrameW, FrameH);
    }

    public bool CanAttack()
    {
        if (attackTimer < 0.5) return false;
        attackTimer = 0;
        return true;
    }

    public void TakeDamage(int amount)
    {
        Health = Mathf.Max(0, Health - amount);
    }

    public void CollectKey() => KeysCollected++;
}
