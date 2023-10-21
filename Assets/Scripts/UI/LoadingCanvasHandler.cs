using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCanvasHandler : MonoBehaviour
{
    [SerializeField]
    private Button _cancelBtn;
    [SerializeField]
    private Animator anim;

    private NetworkRunnerHandler _networkRunnerHandler;

    private void Start()
    {
        _networkRunnerHandler = GlobalManagers.Instance.networkRunnerHandler;

        _networkRunnerHandler.OnPlayerJoinedSuccessfully += OnPlayerJoined;
        _networkRunnerHandler.OnPlayerRunnerConnection += OnStartedRunningConnection;

        _cancelBtn.onClick.AddListener(_networkRunnerHandler.ShutDown);
        this.gameObject.SetActive(false);
    }

    private void OnStartedRunningConnection()
    {
        this.gameObject.SetActive(true);
        StartCoroutine(Utils.PlayAnimTillFinished(gameObject, anim, "PopOIn"));
    }

    private void OnPlayerJoined()
    {
        StartCoroutine(Utils.PlayAnimTillFinished(gameObject, anim, "PopOut", false));
    }

    private void OnDestroy()
    {
        _networkRunnerHandler.OnPlayerJoinedSuccessfully -= OnPlayerJoined;
        _networkRunnerHandler.OnPlayerRunnerConnection -= OnStartedRunningConnection;
    }
}
