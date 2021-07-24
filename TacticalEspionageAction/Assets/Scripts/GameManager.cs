using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Guard[] guards;

    private void Start()
    {
        foreach (var guard in guards)
        {
            guard.TouchPlayer += LooseGame;
        }
    }
    private void LooseGame()
    {
        foreach (var guard in guards)
        {
            StopCoroutine(guard.guardCoroutine);
            Debug.Log("Game over!");
        }
    }
}
