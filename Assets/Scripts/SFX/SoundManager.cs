using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Singleton]
public class SoundManager : MonoBehaviour
{
    public static SoundManager _instance;

    [SerializeField] private AudioSource[] audioSources;
    [SerializeField] private SfxData sfxData;

    private void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            audioSources = GetComponents<AudioSource>();
        }
    }

    private void Update()
    {
        for (int i = 0; i < Cooldowns.Length; ++i)
        {
            if (Cooldowns[i] <= 0) continue;
            Cooldowns[i] -= Time.deltaTime;
        }
    }

    public enum SfxChannel
    {
        SHOOT = 0,
        TRANSFORM = 1,
        EFFECT = 2,
        PLAYER = 3,
        PLAYER_DEATH = 4,
    }

    public float[] ChannelVolume = new float[]
    {
        0.65f,
        0.7f,
        1,
        0.67f,
        0.6f,
    };

    private float[] Cooldowns = new float[]
    {
        0,
        0,
        0,
        0,
        0,
    };

    [SerializeField] float defaultCooldown = 1f;

    public void PlaySound(SfxData.SFXType SFXType, SfxChannel sfxChannel = SfxChannel.SHOOT, float volume = -1f)
    {
        int channelIndex = (int)sfxChannel;
        if (Cooldowns[channelIndex] > 0) return;

        AudioSource audioPlayer = audioSources[channelIndex];

        audioPlayer.volume = volume == -1 ? ChannelVolume[(int)sfxChannel] : volume;
        audioPlayer.clip = sfxData.sfxs[(int)SFXType];
        audioPlayer.Play();

        Cooldowns[channelIndex] = defaultCooldown;
    }
}