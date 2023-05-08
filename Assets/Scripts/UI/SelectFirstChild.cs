using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectFirstChild : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(transform.GetChild(0).gameObject);
    }
}
