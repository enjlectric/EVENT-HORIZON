using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Image HPImagePrefab;
    private List<Image> InstantiatedHPImages = new List<Image>();
    public RectTransform InstantiationRoot;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataInt Health;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataInt MaxHealth;

    public Color HaveColor;
    public Color MostColor;
    public Color OffColor;

    // Start is called before the first frame update
    void Start()
    {
        Health.OnValueChanged.AddListener(ColorHPImages);
        Health.OnValueChanged.AddListener(InstantiateImages);

        InstantiateImages();
    }

    private void InstantiateImages()
    {
        for (int i = InstantiatedHPImages.Count; i < MaxHealth.Value; i++)
        {
            var inst = Instantiate(HPImagePrefab, InstantiationRoot);

            InstantiatedHPImages.Add(inst);
        }

        ColorHPImages();

    }

    private void OnDestroy()
    {
        Health.OnValueChanged.RemoveListener(InstantiateImages);
        Health.OnValueChanged.RemoveListener(ColorHPImages);
    }

    private void ColorHPImages()
    {
        int i = 1;
        foreach (var inst in InstantiatedHPImages)
        {
            if (i == Health.Value)
            {
                inst.color = MostColor;
            } else if (i < Health.Value)
            {
                inst.color = HaveColor;
            } else
            {
                inst.color = OffColor;
            }
            i++;
        }
    }
}
