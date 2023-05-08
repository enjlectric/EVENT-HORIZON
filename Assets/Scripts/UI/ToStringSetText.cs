using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToStringSetText : MonoBehaviour
{
    public void Set(float txt)
    {
        GetComponent<Text>().text = txt.ToString();
    }
    public void Set(int txt)
    {
        GetComponent<Text>().text = txt.ToString();
    }
}
