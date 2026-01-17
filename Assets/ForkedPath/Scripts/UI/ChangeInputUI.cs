using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeInputUI : MonoBehaviour
{
    public Button SwitchSchemeButton;
    public Toggle LockToggle;
    public TextMeshProUGUI SchemeLabel;

    public InputSchemeData[] inputSchemesDisplayData;

    [Serializable]
    public struct InputSchemeData
    {
        public InputScheme inputScheme;
        public string displayName;
        public Sprite icon;
        public bool toggleAvailable;
    }

    private void Awake()
    {
        //lock toggle
        LockToggle.isOn = PlayerInputController.Instance.lockToggle;
        LockToggle.onValueChanged.AddListener(PlayerInputController.Instance.OnLockToggle);
        //switch control scheme button
        SwitchSchemeButton.onClick.AddListener(() => InputSchemeManager.Instance.CycleInputScheme());

        InputSchemeManager.Instance.OnInputSchemeChanged += displaySchemeChanges;
        displaySchemeChanges(InputSchemeManager.Instance.GetCurrentInputScheme());
    }

    void displaySchemeChanges(InputScheme inputScheme)
    {
        var data = Array.Find(inputSchemesDisplayData, d => d.inputScheme == inputScheme);
        Debug.Assert(data.displayName != null, "No display data found for input scheme " + inputScheme.ToString());
        SchemeLabel.text = data.displayName;
        LockToggle.gameObject.SetActive(data.toggleAvailable);
    }
}