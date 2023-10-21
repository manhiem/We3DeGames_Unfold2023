using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomPanel : LobbyBase
{
    [Header("Create Room Vars")]
    [SerializeField]
    private TMP_InputField _joinRoomIP;
    [SerializeField]
    private TMP_InputField _createRoomIP;
    [SerializeField]
    private Button _createRoomBtn;
    //[SerializeField]
    //private Button _joinRoomBtn;
    [SerializeField]
    private Button _joinRandomRoomBtn;

    private NetworkRunnerHandler _networkRunnerHandler;

    public override void InitPanel(LobbyManager lobbyManager)
    {
        base.InitPanel(lobbyManager);

        _networkRunnerHandler = GlobalManagers.Instance.networkRunnerHandler;
        _createRoomBtn.onClick.AddListener(() => OnJoinRoomClicked(GameMode.Host, _createRoomIP.text));
        //_joinRoomBtn.onClick.AddListener(() => OnJoinRoomClicked(GameMode.Client, _joinRoomIP.text));
        _joinRandomRoomBtn.onClick.AddListener(OnJoinRandomRoomClicked);

    }

    private void OnJoinRoomClicked(GameMode mode, string roomCode)
    {
        Debug.Log($"-------Joining a room with {mode} as selected GameMode-----------");
        //btnEffects.Play();
        SoundManager.Instance.StopMusicPlay();
        _networkRunnerHandler.StartGame(mode, roomCode);
    }

    private void OnJoinRandomRoomClicked()
    {
        Debug.Log($"............Joining a random room..............");
        //btnEffects.Play();
        SoundManager.Instance.StopMusicPlay();
        _networkRunnerHandler.StartGame(GameMode.AutoHostOrClient, string.Empty);
    }
}
