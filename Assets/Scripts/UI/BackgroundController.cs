using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BackgroundLayerModulation
{
    public SpriteRenderer renderer;
    public float Delay;
    public float Duration;
    public float StartValue;
    public float AlphaDifference;
}

public class BackgroundController : MonoBehaviour
{
    private float timer = 0;

    public List<BackgroundLayerModulation> Layers = new List<BackgroundLayerModulation>();

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        foreach (var layer in Layers)
        {
            if (timer >= layer.Delay && timer <= layer.Delay + layer.Duration)
            {
                layer.renderer.color = layer.renderer.color.SetAlpha(layer.StartValue + layer.AlphaDifference * (timer - layer.Delay) / layer.Duration);
            }
        }
    }
}
