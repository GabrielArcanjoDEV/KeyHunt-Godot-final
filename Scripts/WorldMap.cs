using Godot;

namespace KeyHunt;

public partial class WorldMap : Node2D
{
    private readonly string[] tileFiles =
    {
        "vazio.png", "chaomarrom.png", "arbusto_topo.png", "pedra.png",
        "agua.png", "grama.png", "arvore.png", "cabana.png", "casaArvore.png"
    };
    private readonly string[] decorationFiles =
    {
        "", "troncocomfolha.png", "troncoseco.png", "arvorecomfruta.png",
        "palmeira1.png", "palmeira2.png", "arvore.png"
    };
    private Texture2D?[] tiles = new Texture2D?[9];
    private Texture2D?[] decorations = new Texture2D?[7];
    private Texture2D? keyTexture;

    public override void _Ready()
    {
        for (int i = 0; i < tileFiles.Length; i++)
            tiles[i] = GD.Load<Texture2D>($"res://Assets/tiles/{tileFiles[i]}");
        for (int i = 1; i < decorationFiles.Length; i++)
            decorations[i] = GD.Load<Texture2D>($"res://Assets/tiles/{decorationFiles[i]}");
        keyTexture = GD.Load<Texture2D>("res://Assets/tiles/chavemapa.png");
        CreateWorldCollisions();
        QueueRedraw();
    }


    private void CreateWorldCollisions()
    {
        for (int row = 0; row < MapData.Rows; row++)
        for (int col = 0; col < MapData.Columns; col++)
        {
            int tileId = MapData.Map[row][col];
            int decorationId = MapData.Decorations[row][col];
            bool solid = MapData.IsSolid(tileId) || MapData.IsDecorationSolid(decorationId);
            if (!solid) continue;

            var body = new StaticBody2D
            {
                Position = new Vector2(col * MapData.TileSize + MapData.TileSize / 2f,
                                       row * MapData.TileSize + MapData.TileSize / 2f),
                CollisionLayer = 1,
                CollisionMask = 0
            };
            body.AddChild(new CollisionShape2D
            {
                Shape = new RectangleShape2D { Size = new Vector2(MapData.TileSize, MapData.TileSize) }
            });
            CallDeferred(Node.MethodName.AddChild, body);
        }
    }

    public override void _Draw()
    {
        int tile = MapData.TileSize;
        for (int row = 0; row < MapData.Rows; row++)
        for (int col = 0; col < MapData.Columns; col++)
        {
            int id = MapData.Map[row][col];
            int dec = MapData.Decorations[row][col];
            Vector2 pos = new(col * tile, row * tile);
            if (dec > 0 || id == 2 || id == 3 || id == 6 || id == 7 || id == 8)
                DrawTextureRect(tiles[5]!, new Rect2(pos, new Vector2(tile, tile)), false);
            if (id >= 0 && id < tiles.Length && tiles[id] != null && id != 6)
            {
                if (id == 2)
                    DrawTextureRect(tiles[id]!, new Rect2(pos + new Vector2(4,4), new Vector2(120,120)), false);
                else
                    DrawTextureRect(tiles[id]!, new Rect2(pos, new Vector2(tile,tile)), false);
            }
        }
        for (int row = 0; row < MapData.Rows; row++)
        for (int col = 0; col < MapData.Columns; col++)
        {
            int id = MapData.Decorations[row][col];
            if (id <= 0 || id >= decorations.Length || decorations[id] == null) continue;
            Vector2 pos = new(col * tile, row * tile);
            if (id == 1 || id == 2)
                DrawTextureRect(decorations[id]!, new Rect2(pos + new Vector2(24,48), new Vector2(80,80)), false);
            else
                DrawTextureRect(decorations[id]!, new Rect2(pos, new Vector2(tile,tile)), false);
        }

        // Casa decorativa ao lado do ponto inicial do personagem.
        // Fica na célula imediatamente acima do spawn, sem bloquear o jogador.
        var house = GD.Load<Texture2D>("res://Assets/tiles/casaArvore.png");
        var spawnHousePos = new Vector2(MapData.PlayerSpawn.Col * tile, (MapData.PlayerSpawn.Row - 1) * tile);
        DrawTextureRect(house, new Rect2(spawnHousePos, new Vector2(tile, tile)), false);
    }
}
