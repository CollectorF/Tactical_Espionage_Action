using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Guard[] guards;
    [SerializeField]
    private PlayerController player;
    [SerializeField]
    private TargetPoint targetPoint;

    private void Start()
    {
        foreach (var guard in guards)
        {
            guard.TouchPlayer += LooseGame;
        }
        targetPoint.OnPlayerOnPoint += WinGame;
    }
    private void LooseGame()
    {
        Debug.Log("Game over!");
        DisableAllGuards();
        DisablePlayer();
    }

    private void WinGame()
    {
        Debug.Log("You win!");
        DisableAllGuards();
        DisablePlayer();
    }

    private void DisableAllGuards()
    {
        foreach (var guard in guards)
        {
            var guardAgent = guard.GetComponent<NavMeshAgent>();
            guardAgent.isStopped = true;
            guard.StopAllCoroutines();
        }
    }
    
    private void DisablePlayer()
    {
        var playerAgent = player.GetComponent<NavMeshAgent>();
        playerAgent.isStopped = true;
    }
}
