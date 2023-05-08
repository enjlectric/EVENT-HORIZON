using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AssetImporters;
#endif


[System.Serializable]
public class Sound
{
    [HorizontalGroup("B")]
    [HideLabel]
    public AudioClip clip;
    [Tooltip("Randomized volume")]
    [MinMaxSlider("", MinValue = 0, MaxValue = 1, ShowFields = true)]
    public Vector2 volume = Vector2.one;
    [Tooltip("Randomized pitch")]
    [MinMaxSlider("", MinValue = -3, MaxValue = 3, ShowFields = true)]
    public Vector2 pitch = Vector2.one;
    [Tooltip("Randomized panning. - is left")]
    [MinMaxSlider("", MinValue = -1, MaxValue = 1, ShowFields = true)]
    public Vector2 panning = Vector2.zero;

    [HorizontalGroup("B")]
    [Button("Play")]

    public void Preview()
    {
#if UNITY_EDITOR
        //EditorSFXFunctions.PlayClip(clip);
#endif
    }
    [HorizontalGroup("B")]
    [Button("Stop")]
    public void StopPreview()
    {
#if UNITY_EDITOR
        //EditorSFXFunctions.StopAllClips();
#endif
    }
}

[System.Serializable]
public class SoundGroup
{
    [HorizontalGroup("B")]
    public SFX type;
    public List<Sound> sounds;
    public AudioMixerGroup mixerGroup;

    [MinMaxSlider("", MinValue = 0, MaxValue = 1)]
    public Vector2 volume = Vector2.one;

    [Tooltip("Lower values are higher priority and can \"steal\" higher value ones")]
    [Range(0, 255)]
    public int priority = 128;

    [Tooltip("How much should position factor into the sound, if played at a specific position?")]
    [Range(0, 1)]
    public float threeDBlend = 0;

    public bool loop = false;

    public float minDelay = 0.05f;

    [HorizontalGroup("B")]
    [Button("Play")]

    public void Preview()
    {
#if UNITY_EDITOR
        //EditorSFXFunctions.PlayClip(sounds[Random.Range(0, sounds.Count)].clip);
#endif
    }
    [HorizontalGroup("B")]
    [Button("Stop")]
    public void StopPreview()
    {
#if UNITY_EDITOR
        //EditorSFXFunctions.StopAllClips();
#endif
    }
}

[System.Serializable]
public class Music
{
    public BGM type;
    public AudioClip clip;
    [Range(0, 1)] public float volume = 1;
}

[CreateAssetMenu(menuName = "Data/Audio", fileName = "Audio")]
public class AudioDefinition : ScriptableObject
{
    [Sirenix.OdinInspector.Searchable]
    public List<SoundGroup> Sounds;
    public List<Music> Music;

    public SoundGroup GetSound(SFX type)
    {
        List<SoundGroup> candidates = Sounds.FindAll(s => s.type == type);
        if (candidates.Count == 0)
        {
            return null;
        }

        return candidates[Random.Range(0, candidates.Count)];
    }

    public Music GetMusic(BGM type)
    {
        List<Music> candidates = Music.FindAll(s => s.type == type);
        if (candidates.Count == 0)
        {
            return null;
        }

        return candidates[Random.Range(0, candidates.Count)];
    }
}

public static class SFXExtensions
{
    public static AudioSource Play(this SFX sfx, float volumeModifier = 1)
    {
        return AudioManager.PlaySound(sfx, volumeModifier);
    }
}