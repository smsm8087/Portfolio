using System.Collections;
using System.Collections.Generic;
using DataModels;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReadyIcon : MonoBehaviour
{
    [SerializeField] private List<GameObject> checkReadyList;
    [SerializeField] private GameObject baseIcon;
    
    public void SetSelected(bool selected)
    {
        baseIcon.SetActive(!selected);
        foreach (var check in checkReadyList)
        {
            check.SetActive(selected);
        }
    }
}