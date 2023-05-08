using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ModularCurve
{
    [HorizontalGroup("-1")]
    public BulletPatternData.BulletListType evaluationPattern;
    [HorizontalGroup("-1")]
    public float timeMultiplier = 1;

    [HorizontalGroup("0")]
    [LabelText("Time")]
    [LabelWidth(100)]
    public AnimationCurve curve = AnimationCurve.Constant(0, 1, 1);
    [HorizontalGroup("0")]
    [HideLabel]
    public float startPosition;

    [HorizontalGroup("1")]
    [LabelText("Amplitude")]
    [LabelWidth(100)]
    public AnimationCurve amplitudeOverTime = AnimationCurve.Constant(0, 1, 0);
    [HorizontalGroup("1")]
    [HideLabel]
    public float amplitudeMultiplier = 1;

    [HorizontalGroup("2")]
    [LabelText("Frequency")]
    [LabelWidth(100)]
    public AnimationCurve frequencyOverTime = AnimationCurve.Constant(0, 1, 1);
    [HorizontalGroup("2")]
    [HideLabel]
    public float frequencyMultiplier = 1;

    public float Get(float curvePosition)
    {
        curvePosition += startPosition;
        curvePosition *= timeMultiplier;
        curvePosition %= 1;

        float amp = amplitudeMultiplier * amplitudeOverTime.Evaluate(curvePosition);
        float freq = frequencyMultiplier * frequencyOverTime.Evaluate(curvePosition);
        curvePosition *= freq;
        curvePosition %= 1;

        switch (evaluationPattern)
        {
            case BulletPatternData.BulletListType.Reverse: curvePosition = 1 - curvePosition; break;
            case BulletPatternData.BulletListType.Random: curvePosition = Random.Range(0.0f, 1.0f); break;
        }
        return curve.Evaluate(curvePosition) * amp;
    }
}

public class BulletEmitter : MonoBehaviour
{
    [InlineEditor]
    [AssetList(AssetNamePrefix = "BulletPattern_")]
    public List<BulletPatternData> patterns;

    private Coroutine[] _routines;

    void Start()
    {
        _routines = new Coroutine[patterns.Count];
    }

    public void CancelPattern(int idx)
    {
        if (idx >= 0 && idx < _routines.Length)
        {
            if (_routines[idx] != null)
            {
                CoroutineManager.Abort(_routines[idx]);
                _routines[idx] = null;
            }
        }
    }

    public void CancelAllPatterns()
    {
        for (int i = 0; i < patterns.Count; i++)
        {
            CancelPattern(i);
        }
    }

    public void EvaluateAllPatterns(float damageMult = 1)
    {
        for (int i = 0; i < patterns.Count; i++)
        {
            EvaluatePattern(i, damageMult);
        }
    }

    public void EvaluatePattern(int idx, float damageMult = 1)
    {
        if (_routines == null)
        {
            return;
        }
        if (idx >= 0 && idx < _routines.Length)
        {
            CancelPattern(idx);
            _routines[idx] = CoroutineManager.Start(PatternCoroutine(damageMult, patterns[idx]));
        }
    }

    private IEnumerator PatternCoroutine(float damageMult, BulletPatternData pattern)
    {
        if (pattern.TelegraphLength > 0)
        {
            if (pattern.TelegraphSound != SFX.None)
            {
                pattern.TelegraphSound.Play();
            }
            var tele = Instantiate(pattern.Telegraph, transform);
            if (pattern.AlignTelegraph)
            {
                pattern.Telegraph.transform.rotation = transform.rotation;
            }
            // Play a sound..
            yield return new WaitForSeconds(pattern.TelegraphLength);
        }
        if (this != null)
        {
            yield return pattern.Evaluate(transform, damageMult);
        }
    }
}
