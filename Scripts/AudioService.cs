using Godot;

namespace KeyHunt;

public partial class AudioService : Node
{
    public static AudioService? Instance { get; private set; }

    private AudioStreamPlayer player = null!;
    private string currentPath = "";
    private bool currentIsLoop = false;

    public bool MusicEnabled { get; private set; } = true;
    public int VolumeLevel { get; private set; } = 3;

    public override void _Ready()
    {
        Instance = this;
        ProcessMode = ProcessModeEnum.Always;

        player = new AudioStreamPlayer
        {
            Name = "BackgroundMusicPlayer",
            Bus = "Master",
            VolumeDb = GetVolumeDb(VolumeLevel)
        };
        AddChild(player);
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }

    public void PlayLoop(string path) => Play(path, true);

    public void PlayOnce(string path) => Play(path, false);

    private void Play(string path, bool loop)
    {
        if (player == null)
            return;

        // Ao trocar de cena (menu <-> jogo <-> opções), mantém a mesma
        // música de fundo tocando sem reiniciar do começo.
        // PlayOnce (vitória/derrota) continua substituindo a música normalmente.
        if (loop && currentIsLoop && currentPath == path && player.Playing)
            return;

        currentPath = path;
        currentIsLoop = loop;
        player.Stop();

        var stream = GD.Load<AudioStream>(path);
        if (stream == null)
        {
            GD.PushWarning($"Audio nao encontrado: {path}");
            return;
        }

        // A trilha de fundo usa OGG para carregar de forma mais leve e confiável.
        // O loop é configurado no próprio stream antes de iniciar a reprodução.
        if (stream is AudioStreamOggVorbis ogg)
            ogg.Loop = loop;
        else if (stream is AudioStreamWav wav)
            wav.LoopMode = loop
                ? AudioStreamWav.LoopModeEnum.Forward
                : AudioStreamWav.LoopModeEnum.Disabled;

        player.Stream = stream;
        player.VolumeDb = GetVolumeDb(VolumeLevel);
        player.StreamPaused = false;

        if (MusicEnabled || !loop)
        {
            player.Play(0.0f);
            GD.Print($"AudioService: reprodução iniciada ({path}), loop={loop}, tocando={player.Playing}");
        }
    }

    public void SetEnabled(bool enabled)
    {
        MusicEnabled = enabled;
        if (!enabled)
        {
            player?.Stop();
        }
        else if (!string.IsNullOrEmpty(currentPath) && currentIsLoop)
        {
            Play(currentPath, true);
        }
    }

    public void SetVolume(int level)
    {
        VolumeLevel = Mathf.Clamp(level, 0, 5);
        if (player != null)
            player.VolumeDb = GetVolumeDb(VolumeLevel);
    }

    private static float GetVolumeDb(int level) => level switch
    {
        0 => -80f,
        1 => -24f,
        2 => -15f,
        3 => -8f,
        4 => -4f,
        _ => 0f
    };

    public void Stop()
    {
        player?.Stop();
        currentPath = "";
        currentIsLoop = false;
    }
}
