using Pixelplacement;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomToggle : MonoBehaviour
{
    [SerializeField]
    Image background;
    [SerializeField]
    TextMeshProUGUI textMeshPro;
    public Toggle toggle { get; private set; }

    [SerializeField]
    Color backgroundColorOn;
    [SerializeField]
    Color backgroundColorOff;
    [SerializeField]
    Color textColorOn;
    [SerializeField]
    Color textColorOff;
    [SerializeField]
    string textOn;
    [SerializeField]
    string textOff;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleValueChanged);

        if (toggle.isOn)
        {
            background.color = backgroundColorOn;
            textMeshPro.color = textColorOn;
            textMeshPro.text = textOn;
        }
        else
        {
            background.color = backgroundColorOff;
            textMeshPro.color = textColorOff;
            textMeshPro.text = textOff;
        }
    }

    private void OnToggleValueChanged(bool on)
    {
        Tween.Color(background, on ? backgroundColorOn : backgroundColorOff, 0.1f, 0f);
        Tween.Color(textMeshPro, on ? textColorOn : textColorOff, 0.1f, 0f);
        textMeshPro.text = on ? textOn : textOff;
    }
}