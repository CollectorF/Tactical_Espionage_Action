using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Guard[] guards;
    [SerializeField]
    private PlayerController player;
    [SerializeField]
    private TargetPoint targetPoint;
    [SerializeField]
    private AudioSource audioSourceRegular;
    [SerializeField]
    private AudioSource audioSourceAlert;
    [SerializeField]
    private UIManager uiManager;

    private PlayableDirector currentDirector;
    private bool sceneIsSkipped = true;
    private float skipTime;
    private int deathCounter;
    private string DEATH_KEY = "death_key";

    private void Start()
    {
        foreach (var guard in guards)
        {
            guard.TouchPlayer += LooseGame;
            guard.SpotPlayer += SpotPlayer;
            guard.LoosePlayer += LoosePlayer;
        }
        targetPoint.OnPlayerOnPoint += WinGame;
        SetDeathCounter(PlayerPrefs.GetInt(DEATH_KEY, 0));
    }

    private void SetDeathCounter(int counter)
    {
        deathCounter = counter;
        uiManager.SetTextInfo(counter);
    }

    private void Update()
    {
        if (Input.anyKeyDown && !sceneIsSkipped)
        {
            skipTime = (float)currentDirector.duration - 0.1f;
            currentDirector.time = skipTime;
            sceneIsSkipped = true;
        }
    }

    private void LooseGame()
    {
        DisableAllGuards();
        DisablePlayer();
        SetDeathCounter(deathCounter + 1);
        Debug.Log("Game over!");
    }

    private void WinGame()
    {
        DisableAllGuards();
        DisablePlayer();
        Debug.Log("You win!");
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

    private void GetDirector(PlayableDirector director)
    {
        sceneIsSkipped = false;
        currentDirector = director;
    }

    public void StartBackgroundAudio()
    {
        audioSourceRegular.Play();
    }
    private void SpotPlayer()
    {
        audioSourceRegular.Pause();
        audioSourceAlert.Play();
    }

    private void LoosePlayer()
    {
        audioSourceRegular.UnPause();
        audioSourceAlert.Stop();
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt(DEATH_KEY, deathCounter);
        PlayerPrefs.Save();
    }

    [ContextMenu("Clear PlayerPrefs")]
    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
