using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SFX
{
    None = -1,

    UI_Move = 0, // OK
    UI_Confirm = 1, // OK
    UI_Cancel = 2, // OK
    UI_Alert_Small = 3, // OK
    UI_Alert_Big = 4, // OK
    UI_Pause = 5, // OK
    UI_Unpause = 6, // OK
    UI_ShowResults = 7, // OK

    StartGame = 10, // OK
    ToMainMenu = 11, // NOOONEED

    PlayerShoot = 20, // OK
    PlayerBlackHole = 21, // OK
    PlayerBlackHoleAbsorb = 22, // OK
    PlayerLaserWeak = 23, // OK
    PlayerLaserMedium = 24, // OK
    PlayerLaserStrong = 25, // OK
    PlayerHurt = 26, // OK
    PlayerHurtLastHP = 27, // OK
    PlayerDie = 28, // OK
    PlayerGainHealth = 29, // OK

    EnemyHit = 30, // OK
    EnemyHitInvulnerable = 31, // OK
    EnemyHitOrganic = 32, // OK
    EnemyHitHeart = 33, // OK
    EnemyTelegraphSmall = 34, // NO NEED
    EnemyTelegraphBig = 35, // NO NEED
    EnemyTelegraphLaser = 36, // OK
    EnemyLaserStart = 37, // OK
    EnemyLaserLoop = 38, // OK
    
    ExplosionSmall = 40, // OK
    ExplosionRegular = 41, // OK
    ExplosionBoss = 42, // OK
    ExplosionBomb = 43, // OK
    ExplosionBlackHole = 44, // OK
    ExplosionPlayerHurt = 45, // OK
    ExplosionMiniboss = 46, // OK

    EnemyTelegraphBeastLaser = 50, // NO NEED
    EnemyBeastLaserStart = 51, // NO NEED
    EnemyBeastLaserLoop = 52, // NO NEED
    EnemyBeastRoar = 53, // OK
    EnemyAlienTalk = 54, // OK
    EnemyCentipedeSpawn = 55, // OK
    EnemyAnglerRoar = 56, // NO NEED
    EnemyKingDockMachine = 57, // OK
    EnemyPhaseTransition = 58, // NO NEED
    EnemyShieldRetaliate = 59, // OK

    PlayerBlackHoleAbsorbStage2 = 60, // OK
    PlayerBlackHoleAbsorbStage3 = 61, // OK
    PlayerBlackHoleAbsorbStage4 = 62, // OK
    PlayerLaserStrongest = 63, // OK
    EnemyFirePew = 64, // OK
    EnemyFireRegular = 65, // OK
    EnemyFireBomb = 66 // OK
}

public enum BGM
{
    None = -1,
    MainMenuIntro,
    MainMenuLoop,
    Stage1Intro,
    Stage1Loop,
    Stage2Intro,
    Stage2Loop,
    Stage3Intro,
    Stage3Loop,
    Stage4Intro,
    Stage4Loop,
    Stage5Intro,
    Stage5Loop,
    Stage6Intro,
    Stage6Loop,
    BossIntro,
    BossLoop,
    EndingIntro,
    EndingLoop,
    LoseIntro,
    LoseLoop,
    TutorialIntro,
    TutorialLoop
}


public class AudioManager : MonoBehaviour
{
    public AudioSource SFXSourcePrefab;
    public Transform SFXSourceParent;

    public AudioSource MusicSource;

    public AudioDefinition data;

    private static Dictionary<SFX, float> LastPlayTime = new Dictionary<SFX, float>();

    private static AudioManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        LastPlayTime = new Dictionary<SFX, float>();
        DontDestroyOnLoad(gameObject);
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += CancelAllSFX;
        instance = this;
    }

    private float time;
    private BGM targetClip;

    private void OnDestroy()
    {
        if (instance == this)
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= CancelAllSFX;
        }
    }

    public static async void ChangeMusic(AudioClip newClip, float blendSpeed = 0.5f)
    {
        if (newClip != instance.MusicSource.clip)
        {
            if (instance.MusicSource.clip != null)
            {
                float t = 1;
                while (t > 0)
                {
                    t -= Time.unscaledDeltaTime * blendSpeed;
                    instance.MusicSource.volume = t;
                    await System.Threading.Tasks.Task.Yield();
                }
            }
            instance.MusicSource.clip = newClip;
            instance.MusicSource.Play();

            if (newClip != null)
            {
                float t = 0;
                while (t < 1)
                {
                    t += Time.unscaledDeltaTime * blendSpeed;
                    instance.MusicSource.volume = Mathf.Min(t, 1);
                    await System.Threading.Tasks.Task.Yield();
                }
            }
        }
    }

    public static void ChangeMusic(BGM introClip, BGM loopingClip)
    {
        instance.targetClip = loopingClip;
        ChangeMusic(instance.data.GetMusic(introClip), instance.data.GetMusic(loopingClip));
    }

    public static void ChangeMusic(BGM clip)
    {
        instance.targetClip = clip;
        ChangeMusic(null, instance.data.GetMusic(clip));
    }

    private void CancelAllSFX(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.Scene scene2)
    {
        foreach (Transform sfx in instance.SFXSourceParent)
        {
            Destroy(sfx.gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        MusicSource.Stop();
        MusicSource.clip = null;
    }

    public static async void FadeOut()
    {
        instance.targetClip = BGM.None;
        if (instance.MusicSource.clip != null)
        {
            float t = 1;
            while (t > 0)
            {
                t -= Time.unscaledDeltaTime * 0.5f;
                instance.MusicSource.volume = t;
                await System.Threading.Tasks.Task.Yield();
            }
        }
        instance.MusicSource.Stop();
    }

    public static void SetMusicVolume(float f)
    {
        instance.MusicSource.volume = f;
    }

    public static void PauseSFX(bool paused)
    {
        foreach(Transform audioSource in instance.SFXSourceParent)
        {
            if (paused)
            {
                audioSource.GetComponent<AudioSource>().Pause();
            } else
            {
                audioSource.GetComponent<AudioSource>().UnPause();
            }
        }
    }

    public static async void ChangeMusic(Music introClip, Music loopingClip)
    {
        if (((introClip != null && introClip.clip != instance.MusicSource.clip) || introClip == null) && (loopingClip != null && loopingClip.clip != instance.MusicSource.clip))
        {
            if (introClip != null)
            {
                if (instance.MusicSource.clip != null)
                {
                    float t = 1;
                    while (t > 0)
                    {
                        t -= Time.unscaledDeltaTime * 0.5f;
                        instance.MusicSource.volume = t;
                        await System.Threading.Tasks.Task.Yield();
                    }
                }
                instance.MusicSource.clip = introClip.clip;
                instance.MusicSource.Play();
                instance.MusicSource.time = 0;
                instance.MusicSource.volume = introClip.volume;
                instance.MusicSource.loop = false;
                instance.time = 0;
                while (instance.MusicSource.isPlaying)
                {
                    await System.Threading.Tasks.Task.Yield();
                }
                if (!Application.isPlaying || instance.MusicSource.clip != introClip.clip)
                {
                    return;
                }
                await System.Threading.Tasks.Task.Yield();
            }

            instance.MusicSource.clip = loopingClip.clip;
            instance.MusicSource.loop = true;
            instance.MusicSource.Play();
            instance.MusicSource.time = 0;
            instance.time = 0;
            instance.MusicSource.volume = loopingClip.volume;
        }
    }

    private static async void AutoDisposeAudioSource(AudioSource source)
    {
        while (source != null && source.isPlaying)
        {
            await System.Threading.Tasks.Task.Yield();
        }

        if (source != null)
        {
            DestroyImmediate(source.gameObject);
        }
    }

    public static AudioSource PlaySound(SFX sfx, float volumeModifier = 1, Vector3 position = default)
    {
        SoundGroup soundgroup = instance.data.GetSound(sfx);
        if (soundgroup == null || soundgroup.sounds.Count == 0)
        {
            Debug.LogError("No sound registered for SFX " + sfx.ToString());
            return null;
        }
        if (LastPlayTime.ContainsKey(sfx))
        {
            if (LastPlayTime[sfx] < Time.time - instance.data.GetSound(sfx).minDelay)
            {
                LastPlayTime[sfx] = Time.time;
            } else
            {
                return null;
            }
        } else
        {
            LastPlayTime.Add(sfx, Time.time);
        }
        var selectedSound = soundgroup.sounds[Random.Range(0, soundgroup.sounds.Count)];
        if (selectedSound == null)
        {
            Debug.LogError("No sound registered for SFX " + sfx.ToString());
            return null;
        }

        if (position == default)
        {
            position = Camera.main.transform.position;
        }

        var source = Instantiate(instance.SFXSourcePrefab, instance.SFXSourceParent);
        source.clip = selectedSound.clip;
        source.volume = selectedSound.volume.Random() * soundgroup.volume.Random() * volumeModifier;
        source.panStereo = selectedSound.panning.Random();
        source.pitch = selectedSound.pitch.Random();
        source.outputAudioMixerGroup = soundgroup.mixerGroup != null ? soundgroup.mixerGroup : source.outputAudioMixerGroup;
        source.priority = soundgroup.priority;
        source.spatialBlend = soundgroup.threeDBlend;
        source.transform.position = position;
        source.loop = soundgroup.loop;
        source.Play();

        if (!source.loop)
        {
            AutoDisposeAudioSource(source);
        }
        return source;
    }
}
