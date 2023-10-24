using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_InputField))]
public class UpdatePref : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    private void Start()
    {
        GetComponent<TMP_InputField>().text = PlayerPrefs.GetString("PlayerName", "");
        
        GetComponent<TMP_InputField>().onValueChanged.AddListener(UpdatePrefValue);
    }

    private void OnDestroy()
    {
        GetComponent<TMP_InputField>().onValueChanged.RemoveListener(UpdatePrefValue);
    }

    public void UpdatePrefValue(string value)
    {
        PlayerPrefs.SetString("PlayerName", value);
    }
}
