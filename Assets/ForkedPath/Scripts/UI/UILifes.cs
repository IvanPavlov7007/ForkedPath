using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class UILifes : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField]
    Transform heartsContainer;
    [SerializeField]
    TextMeshProUGUI livesCounter;

    [Header("Prefabs")]
    [SerializeField]
    GameObject fullHeartPrefab;
    [SerializeField]
    GameObject emptyHeartPrefab;

    List<GameObject> fullHeartInstances = new List<GameObject>();
    List<GameObject> emptyHeartInstances = new List<GameObject>();

    private void Awake()
    {
        for(int i = 0; i < heartsContainer.childCount; i++)
        {
            var child = heartsContainer.GetChild(i).gameObject;
            child.SetActive(false);
        }
    }

    private void OnEnable()
    {
        GameEvents.Instance.OnPlayerLifeChange += onHealthChanged;
        Redraw();
    }

    private void OnDisable()
    {
        GameEvents.Instance.OnPlayerLifeChange -= onHealthChanged;
    }


    void onHealthChanged()
    {
        Redraw();
    }

    public void Redraw()
    {
        if (Player.Instance == null) return;
        livesCounter.text = $"x {Player.Instance.lives.ToString()}";


        if (Player.Instance.CurrentAvatar == null || !Player.Instance.CurrentAvatar.IsInitialized)
        {
            Debug.LogWarning("UILifes: Player's current avatar is null, cannot display health.");
            StartCoroutine(delayedRedraw());
            return;
        }

        
        int currentLives = Player.Instance.CurrentAvatar.Health.CurrentHealth;
        int maxLives = Player.Instance.CurrentAvatar.Health.MaxHealth;

        showFullHearts(currentLives);
        showEmptyHearts(maxLives - currentLives);
    }

    IEnumerator delayedRedraw()
    {
        yield return null;
        Redraw();
    }


    void showFullHearts(int count)
    {
        int currentFullHearts = fullHeartInstances.Count;
        if (count > currentFullHearts)
        {
            for (int i = currentFullHearts; i < count; i++)
            {
                var heart = Instantiate(fullHeartPrefab, heartsContainer);
                heart.transform.SetAsFirstSibling();
                fullHeartInstances.Add(heart);
            }
        }

        for(int i = 0; i < fullHeartInstances.Count; i++)
        {
            fullHeartInstances[i].SetActive(i < count);
        }
    }

    void showEmptyHearts(int count)
    {
        int currentEmptyHearts = emptyHeartInstances.Count;
        if (count > currentEmptyHearts)
        {
            for (int i = currentEmptyHearts; i < count; i++)
            {
                var heart = Instantiate(emptyHeartPrefab, heartsContainer);
                heart.transform.SetAsLastSibling();
                emptyHeartInstances.Add(heart);
            }
        }
        for (int i = 0; i < emptyHeartInstances.Count; i++)
        {
            emptyHeartInstances[i].SetActive(i < count);
        }
    }
}
