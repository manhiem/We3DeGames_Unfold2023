using UnityEngine;
using Fusion;
using TMPro;

public class PlayerHealth : NetworkBehaviour
{

    [Networked(OnChanged = nameof(HealthAmountChanged))] private int health { get; set; }
    [SerializeField] private TextMeshProUGUI healthText;

    private PlayerController playerController;
    public bool isDead = false;

    public override void Spawned()
    {
        health = 100;
        playerController = GetComponent<PlayerController>();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void Rpc_ReduceHealth(int damage)
    {
        Debug.Log($"{damage} hit");
        health -= damage;
        SoundManager.Instance.PlayEffect(Sounds.PlayerHit);
    }

    private static void HealthAmountChanged(Changed<PlayerHealth> changed)
    {
        var curHP = changed.Behaviour.health;

        changed.LoadOld();

        var oldHP = changed.Behaviour.health;

        if (curHP != oldHP)
        {
            changed.Behaviour.UpdateVisuals(curHP);

            if (curHP != 100)
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
        if (Utils.IsLocalPlayer(Object))
        {
            //todo do blood animation, shake camera, etc
            Debug.Log("LOCAL PLAYER GOT HIT!");
            //bloodHitAnim.Play("BloodHit");
            //StartCoroutine(_playerCameraShake.Shake(5f, 100f));
        }

        if (healthAmount <= 0)
        {
            SoundManager.Instance.PlayEffect(Sounds.PlayerDied);
            playerController.KillPlayer();
            Debug.Log("PLAYER DIED!");
        }
    }

    public void TakeDamage(int damage)
    {
        //anim.SetTrigger("Hit");
        Rpc_ReduceHealth(damage);
    }

    public void ResetHealth()
    {
        health = 100;
    }
}
