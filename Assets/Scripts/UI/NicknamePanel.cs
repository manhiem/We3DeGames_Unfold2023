using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class NicknamePanel : LobbyBase
{
    private const int MAX_CHAR_LENGTH = 2;

    [Header("NickName Panel Fields")]
    [SerializeField]
    private TMP_InputField _nickNameIP;
    [SerializeField]
    private Button _createButton;

    public override void InitPanel(LobbyManager lobbyManager)
    {
        base.InitPanel(lobbyManager);
        _createButton.interactable = false;
        _nickNameIP.onValueChanged.AddListener(OnInputGiven);
        _createButton.onClick.AddListener(OnCreateNickname);

        if (PlayerPrefs.HasKey("Nickname"))
            _nickNameIP.text = PlayerPrefs.GetString("Nickname");
    }

    private void OnCreateNickname()
    {
        var nickName = _nickNameIP.text;

        if(nickName.Length >= MAX_CHAR_LENGTH)
        {
            PlayerPrefs.SetString("Nickname", nickName);
            PlayerPrefs.Save();

            ClosePanel();
            btnEffects.Play();
            lobbyManager.ShowPanel(PanelType.RoomPanel);
        }
    }

    private void OnInputGiven(string arg0)
    {
        _createButton.interactable = arg0.Length >= MAX_CHAR_LENGTH;
    }
}