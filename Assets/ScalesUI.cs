using UnityEngine;
using UnityEngine.UI;

public class ScalesUI : MonoBehaviour
{
    public Slider upSlider;
    public Slider downSlider;
    public Image FillImage;

    public Color upColor;
    public Color downColor;
    public Color hiddenColor;

    int _maxValue = 12;
    int _currentValue = 0;

    public void SetUpValue(int value, int maxValue)
    {
        _maxValue = maxValue;
        _currentValue = value;
    }

    public void SetDownValue(int value, int maxValue)
    {
        _maxValue = maxValue;
        _currentValue = -value;
    }

    public void ResetValues()
    {
        _currentValue = 0;
    }

    private void Update()
    {
        if (_currentValue > 0)
        {
            upSlider.maxValue = _maxValue;
            upSlider.value = _currentValue;

            downSlider.value = 0;
            FillImage.color = upColor;
        }
        else if (_currentValue < 0)
        {
            downSlider.maxValue = _maxValue;
            downSlider.value = -_currentValue;

            upSlider.value = 0;
            FillImage.color = downColor;
        }
        else
        {
            upSlider.value = 0;
            downSlider.value = 0;
            FillImage.color = hiddenColor;
        }
    }
}
