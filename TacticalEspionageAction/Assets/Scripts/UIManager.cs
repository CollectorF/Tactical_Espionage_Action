using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    private TMP_Text uiDeathText;

    private void Awake()
    {
        uiDeathText = GetComponentInChildren<TMP_Text>();
        gameObject.SetActive(false);
    }
    public void SetTextInfo(int score)
    {
        uiDeathText.text = $"You died {score} times!";
    }
}
