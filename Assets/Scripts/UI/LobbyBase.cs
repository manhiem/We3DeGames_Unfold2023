using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyBase : MonoBehaviour
{
    [field: SerializeField, Header("LobbyBase Vars")]
    public PanelType panelType;
    public Animator anim;
    public AudioSource btnEffects;
    protected LobbyManager lobbyManager;

    public enum PanelType
    {
        None,
        NicknamePanel,
        RoomPanel
    }

    public virtual void InitPanel(LobbyManager lobbyManager)
    {
        this.lobbyManager = lobbyManager;
    }

    public void ShowPanel(PanelType panelType)
    {
        this.gameObject.SetActive(true);
        anim.Play("PopIn");
    }

    protected void ClosePanel()
    {
        StartCoroutine(Utils.PlayAnimTillFinished(gameObject, anim, "PopOut", false));
    }
}
