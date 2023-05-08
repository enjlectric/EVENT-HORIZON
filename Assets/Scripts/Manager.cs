using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class Manager : MonoBehaviour
{
    public static Manager instance;
    private PlayerController _player;

    public AudioMixer AudioMixer;

    public string MasterGroup;
    public string MusicGroup;
    public string SFXGroup;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataFloat MasterGroupVolume;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataFloat MusicVolume;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataFloat SFXGroupVolume;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataFloat Vignette;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataTexture2D VignetteTexture;
    public Texture2D VignetteTex;

    public PlayerController player { get
        {

            if (_player == null)
            {
                _player = FindObjectOfType<PlayerController>();
            }
            return _player;
        }
        private set { _player = value; } }
    private PlayerInput _input;
    public PlayerInput input
    {
        get
        {
            if (_input == null)
            {
                if (player != null)
                {
                    _input = player.GetComponent<PlayerInput>();
                }
            }
            return _input;
        }
        private set { _input = value; }
    }

    public GameLevels gameMode;

    public static float deltaTime;

    public float GameSpeed = 8;
    public int StartLevel = 0;
    public int StartWave = 0;

    [HideInInspector]
    public bool canPause = true;
    [HideInInspector]
    public float time;
    [HideInInspector]
    public float kills = 0;

    [HideInInspector]
    public int thisLevelHits = 0;
    [HideInInspector]
    public int thisLevelKills = 0;
    [HideInInspector]
    public int thisLevelCombo = 0;

    [HideInInspector]
    public int currentCombo = 0;

    public PickupData CoinPickup1;
    public PickupData CoinPickup5;
    public PickupData CoinPickup10;
    public List<Enjlectric.ScriptableData.ScriptableDataBase> ResettableScriptableData = new List<Enjlectric.ScriptableData.ScriptableDataBase>();

    public Enjlectric.ScriptableData.Concrete.ScriptableDataInt Coins;

    private static Coroutine _slowdownRoutine;

    private int _score = 0;
    private int _scoreMultiplier = 10;

    public int Score
    {
        get { return _score; }
        set { 
            _score = value;
            Coins.Value = value;
            //UIManager.instance.SetPoints(value);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            MasterGroupVolume.OnValueChanged.AddListener(ChangeMasterVolume);
            SFXGroupVolume.OnValueChanged.AddListener(ChangeSFXVolume);
            MusicVolume.OnValueChanged.AddListener(ChangeMusicVolume);
            return;
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            MasterGroupVolume.OnValueChanged.RemoveListener(ChangeMasterVolume);
            SFXGroupVolume.OnValueChanged.RemoveListener(ChangeSFXVolume);
            MusicVolume.OnValueChanged.RemoveListener(ChangeMusicVolume);
        }
    }

    private float GetVolumeInDB(float volume)
    {
        return Mathf.Max(-80f, Mathf.Log(Mathf.Clamp01(volume)) * 20);
    }

    private void ChangeMasterVolume()
    {
        AudioMixer.SetFloat(MasterGroup, GetVolumeInDB(MasterGroupVolume.Value));
    }

    private void ChangeMusicVolume()
    {
        AudioMixer.SetFloat(MusicGroup, GetVolumeInDB(MusicVolume.Value));
    }

    private void ChangeSFXVolume()
    {
        AudioMixer.SetFloat(SFXGroup, GetVolumeInDB(SFXGroupVolume.Value));
    }

    public static void LockPlayerInput(bool locked)
    {
        if (locked)
        {
            instance.input?.DeactivateInput();
        } else
        {
            instance.input?.ActivateInput();
        }
    }

    public static void ShakeCamera(float duration, float strength, bool rotate = true)
    {
        Camera.main.DOComplete();
        Camera.main.DOShakePosition(duration, strength, 50);
        if (rotate)
        {
            Camera.main.DOShakeRotation(duration, strength * 30, 50);
        }
    }

    public void EndRun()
    {
        LockPlayerInput(true);
        SFX.PlayerDie.Play();
        AudioManager.ChangeMusic(null, 1f);
        //UIManager.instance.DOFlash(2f, 1f);
        CoroutineManager.Start(RunEnder());
    }

    private IEnumerator RunEnder()
    {
        yield return new WaitForSeconds(2f);
        VignetteTexture.Value = VignetteTex;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 0.5f;
            Vignette.Value = t;
            yield return null;
        }
        Vignette.Value = 1;
        References.DestroyObjectsOfType<BulletBehaviour>();
        References.DestroyObjectsOfType<EnemyBehaviour>();
        CoroutineManager.AbortAll();
        CoroutineManager.Start(Transition());
    }

    IEnumerator Transition()
    {
        SceneManager.LoadScene(9);
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 0.5f;
            Vignette.Value = 1 - t;
            yield return null;
        }
        Vignette.Value = 0;
    }

    public static bool InputIsLocked()
    {
        return instance.input != null && !instance.input.inputIsActive;
    }

    public static void SetUIInput()
    {
        instance.input?.SwitchCurrentActionMap("UI");
    }

    public static void SetWorldInput()
    {
        instance.input?.SwitchCurrentActionMap("Player");
    }


    private void Start()
    {
        #if UNITY_EDITOR
        if (StartLevel != 0 || StartWave != 0)
        {
            canPause = true;
            CoroutineManager.Start(gameMode.Execute());
            return;
        }
        #endif

    }

    public void StartRun()
    {
        for (int i = 0; i < ResettableScriptableData.Count; i++)
        {
            ResettableScriptableData[i].ResetToDefault();
        }
        References.DeactivateAll();
        CoroutineManager.AbortAll();
        CoroutineManager.Start(RunStartRoutine());
    }

    public void FinishCurrentWave()
    {
        gameMode.FinishCurrentWave();
    }

    private IEnumerator RunStartRoutine()
    {
        AudioManager.ChangeMusic(null, 4);
        VignetteTexture.Value = VignetteTex;
        Vignette.Value = 1;
        SceneManager.LoadScene(1);

        canPause = true;
        CoroutineManager.Start(gameMode.Execute());
        Score = 0;
        kills = 0;
        time = 0;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 2f;
            Vignette.Value = 1 - t;
            yield return null;
        }
        Vignette.Value = 0;
    }

    public static void DoSlowdown(float speed, float minTime)
    {
        if (_slowdownRoutine != null)
        {
            CoroutineManager.Abort(_slowdownRoutine);
            _slowdownRoutine = null;
        }
        _slowdownRoutine = CoroutineManager.Start(instance.SlowdownRoutine(speed, minTime));
    }

    private IEnumerator SlowdownRoutine(float speed, float minTime)
    {
        Time.timeScale  = minTime;
        while (Time.timeScale < 1)
        {
            yield return null;
            Time.timeScale += Time.unscaledDeltaTime * speed;
        }
        Time.timeScale = 1;
    }

    private void Update()
    {
        deltaTime = Time.deltaTime;

        if (!InputIsLocked())
        {
            time += Time.deltaTime;
        }
    }

    public void AddPoints(int points)
    {
        var s = Score;
        Score += points;

        var remainder = Mathf.FloorToInt(s / 5000.0f) > Mathf.FloorToInt(Score / 5000.0f);

        if (remainder)
        {
            Manager.instance.player.AddHP();
        }
    }

    public void KillEnemy()
    {
        currentCombo++;
        kills++;
        thisLevelKills++;
    }

    public void ReduceScoreMultiplierFromDamage()
    {
        thisLevelCombo = currentCombo;
        currentCombo = 0;
    }

    public void GoToMainMenu()
    {
        References.DeactivateAll();
        CoroutineManager.AbortAll();
        CoroutineManager.Start(ShowHideVignette(0));
    }

    private IEnumerator ShowHideVignette(int sceneidx)
    {
        AudioManager.ChangeMusic(null, 8);
        float t = 0;
        Vignette.Value = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 2f;
            Vignette.Value = t;
            yield return null;
        }
        Vignette.Value = 1;
        SceneManager.LoadScene(sceneidx);
    }

    public void PointExplosion(Vector3 position, int points)
    {
        if (true)
        {
            Manager.instance.AddPoints(points);
            return;
        }
        for (int i = 0; i < points; i++)
        {
            var rng = Random.Range(0, 3);
            if (rng == 0 && points - i >= 10)
            {
                References.CreateObject<Pickup>(CoinPickup10, position);
                i += 9;
            } else if (rng < 2 && points - i >= 5)
            {
                References.CreateObject<Pickup>(CoinPickup5, position);
                i += 4;
            }
            else
            {
                References.CreateObject<Pickup>(CoinPickup1, position);
            }
        }
    }
}