using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickup/Score")]
public class ScorePickupData : PickupData
{
    public override void GrantPlayer()
    {
        Manager.instance.AddPoints((int)value);
    }
}
