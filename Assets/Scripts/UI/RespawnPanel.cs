using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RespawnPanel : SimulationBehaviour
{
    [SerializeField] private PlayerMovementController _playerMovementController;
    [SerializeField] private TextMeshProUGUI _respawnAmountText;
    [SerializeField] private GameObject childObj;

    public override void FixedUpdateNetwork()
    {
        if(Utils.IsLocalPlayer(Object))
        {
            var timerIsRunning = _playerMovementController.respawnTimer.IsRunning;
            childObj.SetActive(timerIsRunning);

            if(timerIsRunning && _playerMovementController.respawnTimer.RemainingTime(Runner).HasValue)
            {
                var time = _playerMovementController.respawnTimer.RemainingTime(Runner).Value;
                var roundToInt = Mathf.RoundToInt(time);
                _respawnAmountText.text = roundToInt.ToString();
            }
        }
    }
}
