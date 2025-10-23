using UnityEngine;
using Pixelplacement;
using TMPro;
using Sirenix;
public class UpgradesNotificationUI : Singleton<UpgradesNotificationUI>
{
    [SerializeField] GameObject worldTextLabelPrefab;
    [SerializeField] float floatingDistance = 2f;
    [SerializeField] float floatingDuration = 1f;

    [SerializeField] Color meatColor = Color.red;
    [SerializeField] Color veggieColor = Color.green;

    private void OnEnable()
    {
        GameEvents.Instance.OnPlayerUpgraded += onPlayerUpgraded;
        GameEvents.Instance.OnPlayerHealed += onHealthReplenish;
    }

    private void OnDisable()
    {
        GameEvents.Instance.OnPlayerUpgraded -= onPlayerUpgraded;
        GameEvents.Instance.OnPlayerHealed -= onHealthReplenish;
    }

    void onPlayerUpgraded(ProgressionLevel newLevel)
    {

        Entity player = Player.Instance.CurrentAvatar;
        var type = ProgressionManager.Instance.CurrentComboTracker.CurrentType;
        Color color = Color.white;
        switch (type)
        {
            case EntityFoodType.Meat:
                color = meatColor;
                break;
            case EntityFoodType.Vegetable:
                color = veggieColor;
                break;
        }
        createText($"+{type.ToString()} Level {newLevel.level}", color, player.transform.position);
    }


    private void createText(string text, Color color, Vector3 position)
    {
        var textLabel = Instantiate(worldTextLabelPrefab, position, Quaternion.identity);
        TextMeshProUGUI textMeshPro = textLabel.GetComponentInChildren<TextMeshProUGUI>();
        var transparency = Transparency.GetController(textMeshPro);
        textMeshPro.color = color;
        textMeshPro.text = text;
        Tween.Position(textLabel.transform, textLabel.transform.position + Vector3.up * floatingDistance, floatingDuration, 0f);
        float duration = floatingDuration * 0.3f;
        Tween.Value(1f, 0f, v => transparency.Alpha = v, duration, floatingDuration - duration);
        Destroy(textLabel, floatingDuration + 0.5f);
    }

    void onHealthReplenish(int amount)
    {
        Vector2 position = Player.Instance.CurrentAvatar?.transform.position ?? Camera.main.transform.position;

        createText($"{amount} Health restored", Color.white, position);
    }
}
