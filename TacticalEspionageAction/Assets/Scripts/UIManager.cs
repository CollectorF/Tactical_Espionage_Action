using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text uiDeathText;
    [SerializeField]
    private TMP_Text uiSpotText;

    private void Awake()
    {
        gameObject.SetActive(false);
    }
    public void SetTextInfo(int dead, int spot)
    {
        uiDeathText.text = $"You died {dead} times!";
        uiSpotText.text = $"You've been spotted {spot} times!";
    }
}
