using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Reflection;

public class DebugPropertyDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmPro;
    [SerializeField] private MonoBehaviour obj;
    [SerializeField] private string propertyName;

    private void Update()
    {
#if UNITY_EDITOR
        var property = obj.GetType().GetProperty(propertyName);
        var value = property.GetValue(obj);
        tmPro.text = value.ToString();
#endif
    }
}
