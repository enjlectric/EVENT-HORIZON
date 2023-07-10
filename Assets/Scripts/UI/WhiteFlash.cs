using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteFlash : MonoBehaviour
{
    public Enjlectric.ScriptableData.Types.ScriptableDataInt WhiteFlashOpacity;

    private CanvasGroup _cg;

    // Start is called before the first frame update
    private void Awake()
    {
        _cg = GetComponent<CanvasGroup>();
        _cg.alpha = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        if (WhiteFlashOpacity.Value > 0)
        {
            WhiteFlashOpacity.SetValueWithoutNotify(WhiteFlashOpacity.Value - 1);
            _cg.alpha = 1;
        }
        else if (_cg.alpha > 0)
        {
            _cg.alpha = 0;
        }
    }
}