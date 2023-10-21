using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : NetworkBehaviour
{
    [SerializeField] private Animator bloodHitAnim;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private PlayerCameraShake _playerCameraShake;
    [SerializeField] private LayerMask _deathGroundLayer;
    [SerializeField] private GameManager gameManager;

    PlayerMovementController _playerMovementController;

    [Networked(OnChanged = nameof(HealthAmountChanged))] private int _curHealthAmount {  get; set; }

    public override void Spawned()
    {
        _curHealthAmount = 100;
        _playerMovementController = GetComponent<PlayerMovementController>();
        gameManager = FindObjectOfType<GameManager>();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void Rpc_ReduceHealth(int damage)
    {
        Debug.Log($"{damage} hit");
        _curHealthAmount -= damage;
        SoundManager.Instance.PlayEffect(Sounds.PlayerHit);
    }

    private static void HealthAmountChanged(Changed<PlayerHealthManager> changed)
    {
        var curHP = changed.Behaviour._curHealthAmount;

        changed.LoadOld();

        var oldHP = changed.Behaviour._curHealthAmount;

        if(curHP != oldHP)
        {
            changed.Behaviour.UpdateVisuals(curHP);

            if(curHP!=100)
            {
                changed.Behaviour.PlayerGotHit(curHP);
            }
        }
    }

    private void UpdateVisuals(int healthAmount)
    {
        var num = (float)healthAmount / 100;
        healthText.text = $"{healthAmount}/100";
    }

    private void PlayerGotHit(int healthAmount)
    {
        if(Utils.IsLocalPlayer(Object))
        {
            //todo do blood animation, shake camera, etc
            Debug.Log("LOCAL PLAYER GOT HIT!");
            bloodHitAnim.Play("BloodHit");
            StartCoroutine(_playerCameraShake.Shake(5f, 100f));
        }

        if(healthAmount <= 0)
        {
            SoundManager.Instance.PlayEffect(Sounds.PlayerDied);
            _playerMovementController.KillPlayer();
            Debug.Log("PLAYER DIED!");
        }
    }

    public void ResetHealth()
    {
        _curHealthAmount = 100;
    }
}
