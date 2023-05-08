using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public RectTransform ready;
    public List<Image> readyText;

    [Header("Warning")]

    public RectTransform warning;
    public List<Image> warningImages;
    public List<WarningPulse> warningText;

    [Header("Boss Health")]

    public CanvasGroup healthCanvasGroup;
    public HealthBar healthBarPrefab;
    private List<HealthBar> _healthBars = new List<HealthBar>();
    public List<Color> healthBarColors;

    [Header("HUD")]

    public CanvasGroup hudGroup;

    public RectTransform healthNeedle;
    public RectTransform healthNeedle2;
    public Text pointsText;
    public Text scoreMultiplierText;

    public Image powerUpProgressBar;
    public List<Image> powerUps;

    public Image flash;

    [Header("MainMenu")]

    public CanvasGroup mainMenuGroup;
    public Text highScoreText;

    [Header("PauseMenu")]

    public CanvasGroup pauseGroup;
    public AudioMixer masterMixer;
    public Sprite musicActiveSprite;
    public Sprite musicInactiveSprite;
    public List<Image> musicImages;
    public List<Image> sfxImages;

    [Header("Victory")]

    public GameObject victory;
    public RectTransform victoryHeader;
    public Text victoryHeaderText;
    public ResultsDisplay victoryContentTime;
    public ResultsDisplay victoryContentKills;
    public ResultsDisplay victoryContentHits;
    public Text victoryOntoNextText;

    [Header("ResultsScreen")]

    public Image thanksForPlayingImage;
    public CanvasGroup resultsGroup;
    public CanvasGroup resultsTitleGroup;
    public RectTransform resultsTitleRUN;
    public RectTransform resultsTitleOVER;
    public CanvasGroup resultsContentGroup;
    public ResultsDisplay resultsContentPoints;
    public ResultsDisplay resultsContentTime;
    public ResultsDisplay resultsContentKills;
    public CanvasGroup resultsButtonGroup;

    public static UIManager instance;

    private int _lastPowerLevel = 0;

    private Tween _needleShakeTween;
    private Tween _needleMoveTween;
    private Tween _powerFillTween;
    private Tween _scoreTextTween;

    private Tween _flashTween;
    private int _lastPoints;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ready.gameObject.SetActive(false);
        warning.gameObject.SetActive(false);
        victory.gameObject.SetActive(false);
        int musicVolume = 6;

        if (PlayerPrefs.HasKey("musicVol"))
        {
            musicVolume = PlayerPrefs.GetInt("musicVol");
        }

        int sfxVolume = 9;

        if (PlayerPrefs.HasKey("soundVol"))
        {
            sfxVolume = PlayerPrefs.GetInt("soundVol");
        }

        SetVolumeDisplay(musicImages, "musicVol", musicVolume);
        SetVolumeDisplay(sfxImages, "soundVol", sfxVolume);
    }

    public void AdjustMusicVolume(int add)
    {
        SetVolumeDisplay(musicImages, "musicVol", PlayerPrefs.GetInt("musicVol") + add);
    }

    public void AdjustSFXVolume(int add)
    {
        SetVolumeDisplay(sfxImages, "soundVol", PlayerPrefs.GetInt("soundVol") + add);
    }

    public void ResetUI()
    {
        foreach(var item in _healthBars)
        {
            Destroy(item.gameObject);
        }
        _healthBars.Clear();
        ready.gameObject.SetActive(false);
        warning.gameObject.SetActive(false);
        victory.gameObject.SetActive(false);
        healthCanvasGroup.alpha = 0;
        hudGroup.DOFade(0, 0.25f);
    }

    private void SetVolumeDisplay(List<Image> images, string path, int value)
    {
        value = Mathf.Clamp(value, 0, 10);
        PlayerPrefs.SetInt(path, value);
        masterMixer.SetFloat(path, Mathf.Max(-80f, Mathf.Log(Mathf.Clamp01(value * 0.1f)) * 20));

        for (int i = 0; i < images.Count; i++)
        {
            images[i].sprite = (value-1) >= i ? musicActiveSprite : musicInactiveSprite;
        }
    }

    public void SetPoints(int points)
    {
        _scoreTextTween?.Complete();
        _scoreTextTween = pointsText.DOCounter(_lastPoints, points, 0.5f, false);
        _scoreTextTween.onUpdate += () => pointsText.text = int.Parse(pointsText.text).ToString("D9");
        _lastPoints = points;
    }

    public void SetScoreMultiplier(float multiplier)
    {
        scoreMultiplierText.text = $"x{multiplier.ToString("0.0")}";
    }

    public void DOFlash(float duration = 1, float maxOpacity = 1)
    {
        _flashTween?.Pause();
        _flashTween = flash.DOFade(maxOpacity, duration * 0.25f).SetEase(Ease.OutQuad);
        _flashTween.onComplete += () => flash.DOFade(0, duration * 0.75f).SetEase(Ease.InQuint);
    }

    public void SetHP(float hp)
    {
        float angle = 90 - 180 * (hp/Manager.instance.player.data.health);

        float a = 90 - 180 * (1 / Manager.instance.player.data.health);
        if (angle < healthNeedle.transform.localEulerAngles.z && angle <= a && healthNeedle.transform.localEulerAngles.z > a)
        {
        }

        _needleMoveTween?.Complete();
        _needleMoveTween = healthNeedle.transform.DORotate(new Vector3(0, 0, angle), 0.25f).SetEase(Ease.OutQuart);
    }

    public void ShakeHPNeedle(bool permanent = false)
    {
        _needleShakeTween?.Kill(true);
        _needleShakeTween = healthNeedle2.transform.DOShakeRotation(0.25f, 15f, 25);
        healthNeedle2.DOPunchScale(Vector2.one * 0.25f, 0.2f);
        _needleShakeTween.SetLoops(0);
        if (permanent)
        {
            _needleShakeTween.SetLoops(-1);
        }
    }

    public void ShowResultsScreen()
    {
        CoroutineManager.Start(Results());
    }

    private IEnumerator Results()
    {
        if (!PlayerPrefs.HasKey("HighScore") || Manager.instance.Score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", Manager.instance.Score);
        }

        resultsGroup.alpha = 1;
        resultsTitleGroup.alpha = 1;
        resultsContentGroup.alpha = 0;
        resultsButtonGroup.alpha = 0;
        resultsTitleGroup.transform.localPosition = Vector3.zero;
        resultsTitleRUN.gameObject.SetActive(true);
        resultsTitleOVER.gameObject.SetActive(false);
        resultsTitleRUN.transform.localScale = Vector3.one * 2;
        resultsTitleRUN.DOScale(Vector3.one, 0.75f).SetEase(Ease.OutQuint);
        yield return new WaitForSeconds(1f);
        resultsTitleOVER.gameObject.SetActive(true);
        resultsTitleOVER.transform.localScale = Vector3.one * 2;
        resultsTitleOVER.DOScale(Vector3.one, 0.75f).SetEase(Ease.OutQuint);
        yield return new WaitForSeconds(1f);
        resultsTitleGroup.transform.DOLocalMoveY(110, 1f).SetEase(Ease.InOutQuad);
        resultsContentPoints.Reset();
        resultsContentTime.Reset();
        resultsContentKills.Reset();
        resultsContentGroup.DOFade(1, 1f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.25f);
        resultsContentPoints.Show(Manager.instance.Score.ToString());

        yield return new WaitForSeconds(0.25f);
        var time = new System.TimeSpan(0, Mathf.FloorToInt(Manager.instance.time / 60), Mathf.FloorToInt(Manager.instance.time % 60));
        resultsContentTime.Show(string.Format("{0}:{1}", Mathf.FloorToInt((float)time.TotalMinutes), time.Seconds.ToString("D2")));
        yield return new WaitForSeconds(0.25f);
        resultsContentKills.Show(Manager.instance.kills.ToString());
        yield return new WaitForSeconds(0.25f);
        resultsGroup.interactable = true;
        resultsGroup.blocksRaycasts = true;
        resultsButtonGroup.DOFade(1, 0.5f);
    }

    public void ExitResultsScreenLeft()
    {
        resultsGroup.interactable = false;
        resultsGroup.blocksRaycasts = false;
        resultsGroup.DOFade(0, 0.25f);
        Manager.instance.StartRun();
    }

    public void ExitResultsScreenRight()
    {
        resultsGroup.interactable = false;
        resultsGroup.blocksRaycasts = false;
        resultsGroup.DOFade(0, 0.25f);
        Manager.instance.GoToMainMenu();
    }

    public void StartFromMainMenu()
    {
        Manager.instance.StartRun();
    }

    public void SetPowerLevel(int powerLevel)
    {
        if (powerLevel < _lastPowerLevel)
        {
            for (int i = powerLevel; i < _lastPowerLevel; i++)
            {
                powerUps[i].transform.DOScale(0, 0.25f).SetEase(Ease.InQuad);
            }
        } else if (powerLevel > _lastPowerLevel)
        {
            for (int i = _lastPowerLevel; i < powerLevel; i++)
            {
                powerUps[i].gameObject.SetActive(true);
                powerUps[i].transform.localScale = Vector3.zero;
                powerUps[i].transform.DOScale(1, 0.75f).SetEase(Ease.OutBack);
            }
        }

        _lastPowerLevel = powerLevel;
    }

    public void SetPowerLevelProgress(float powerLevelProgress)
    {
        _powerFillTween?.Complete();
        _powerFillTween = powerUpProgressBar.DOFillAmount(powerLevelProgress, 0.5f);
    }

    public void EnablePause()
    {
        if (Manager.InputIsLocked() || !Manager.instance.canPause) return;
        Manager.SetUIInput();
        pauseGroup.blocksRaycasts = true;
        pauseGroup.alpha = 1;
        Time.timeScale = 0;
        pauseGroup.interactable = true;
    }

    public void DisablePause()
    {
        Manager.SetWorldInput();
        pauseGroup.blocksRaycasts = false;
        pauseGroup.alpha = 0;
        Time.timeScale = 1;
        pauseGroup.interactable = false;
    }

    public void DisablePauseAndReset()
    {
        DisablePause();
        Manager.instance.StartRun();
    }

    public void DisablePauseAndEnd()
    {
        DisablePause();
        Manager.instance.GoToMainMenu();
    }

    public void InitializeBossHP(int phases)
    {
        CoroutineManager.Start(InitializeHP(phases));
    }

    private IEnumerator InitializeHP(int phases)
    {
        foreach(var hbar in _healthBars)
        {
            if (hbar != null)
            {
                Destroy(hbar.gameObject);
            }
        }
        _healthBars = new List<HealthBar>();
        healthCanvasGroup.DOFade(1, 1);
        for (int i = 0; i < phases; i++)
        {
            var instance = Instantiate(healthBarPrefab, healthCanvasGroup.transform);
            instance.Initialize(healthBarColors[Mathf.Min(i, healthBarColors.Count - 1)]);
            instance.transform.Translate((-Vector2.one + Vector2.down) * 4 * i);
            _healthBars.Add(instance);
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void ReduceBossHP(int phase, float amount)
    {
        if (_healthBars.Count > 0)
        {
            _healthBars[_healthBars.Count - 1 - phase].ReduceTo(amount);
        }
    }
}
