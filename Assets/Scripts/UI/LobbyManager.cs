using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public LobbyBase[] panelTypes;

    private void Start()
    {
        foreach (var panelType in panelTypes)
        {
            panelType.InitPanel(this);
        }
    }

    public void ShowPanel(LobbyBase.PanelType panel)
    {
        foreach (var item in panelTypes)
        {
            if(item.panelType == panel)
            {
                item.ShowPanel(panel);
                break;
            }
        }
    }
}
