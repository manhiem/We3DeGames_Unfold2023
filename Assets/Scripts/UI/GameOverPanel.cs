using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private Button returnToLobby;
    [SerializeField] private GameObject childObj;
    // Start is called before the first frame update
    void Start()
    {
        GlobalManagers.Instance.gameManager.OnGameIsOver += GMOnGameIsOver;
        returnToLobby.onClick.AddListener(() => GlobalManagers.Instance.networkRunnerHandler.ShutDown());
    }

    private void GMOnGameIsOver()
    {
        childObj.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnDestroy()
    {
        GlobalManagers.Instance.gameManager.OnGameIsOver -= GMOnGameIsOver;
    }
}
