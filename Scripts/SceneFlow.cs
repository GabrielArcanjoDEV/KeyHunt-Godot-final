using Godot;

namespace KeyHunt;

public static class SceneFlow
{
    public static void Go(string scenePath)
    {
        var tree = Engine.GetMainLoop() as SceneTree;
        tree?.ChangeSceneToFile(scenePath);
    }
}
