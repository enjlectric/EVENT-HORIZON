using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialActions : MonoBehaviour
{
    public Enjlectric.ScriptableData.Concrete.ScriptableDataFloat LaserValue;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataInt Health;

    public GameObject HasShotGroup;
    public GameObject HasSuckedGroup;
    public GameObject HasFiredLaserGroup;
    public GameObject HasTakenDamageGroup;

    public void Shoot()
    {
        HasShotGroup.SetActive(true);
    }

    public void Suck()
    {
        HasSuckedGroup.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (HasTakenDamageGroup.activeSelf == false)
        {
            if (Health.Value < 4)
            {
                HasTakenDamageGroup.SetActive(true);
            }
        }

        if (HasFiredLaserGroup.activeSelf == false)
        {
            if (LaserValue.Value > 0)
            {
                HasFiredLaserGroup.SetActive(true);
            }
        }
    }
}
