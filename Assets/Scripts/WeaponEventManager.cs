using Fusion;
using System;
using System.Collections;
using UnityEngine;

public class WeaponEventManager : NetworkBehaviour
{
    [Serializable]
    public class GunInfo
    {
        public int attackRange;
        public Transform attackPoint;
        public NetworkPrefabRef[] atkTrails; // Use an array to support multiple guns
        public float shootInterval = 0.18f;
        public AudioClip gunAudio;
    }

    [SerializeField]
    private LayerMask enemyLayers;

    [Networked] private NetworkButtons prevButtons { get; set; }

    [Networked] private TickTimer shootCooldown { get; set; }
    [Networked, HideInInspector] public NetworkBool holdingShootKey { get; private set; }

    PlayerController playerController;
    public GunInfo[] guns;
    bool weaponAnimTriggered = true;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (Runner.TryGetInputForPlayer<PlayerData>(Object.InputAuthority, out var input))
        {
            if (playerController.AcceptAnyInput)
            {
                AttackEvent(input);
                if (!weaponAnimTriggered)
                {
                    playerController.weaponAnim.SetTrigger(playerController.ANIM_NAME);
                    weaponAnimTriggered = true;
                }
                prevButtons = input.networkButtons;
            }
            else
            {
                holdingShootKey = false;
                prevButtons = default;
            }
        }
    }

    public void AttackEvent(PlayerData playerData)
    {
        var curButtons = playerData.networkButtons.GetPressed(prevButtons);
        holdingShootKey = curButtons.WasReleased(prevButtons, PlayerController.PlayerButtons.Shoot);

        if (curButtons.WasReleased(prevButtons, PlayerController.PlayerButtons.Shoot) &&
            shootCooldown.ExpiredOrNotRunning(Runner))
        {
            shootCooldown = TickTimer.CreateFromSeconds(Runner, guns[0].shootInterval);

            int currentGunIndex = 0; // You may need to determine the current gun index dynamically

            NetworkRunner trail = Runner.Spawn(guns[currentGunIndex].atkTrails[currentGunIndex], guns[currentGunIndex].attackPoint.position, guns[currentGunIndex].attackPoint.rotation, Object.InputAuthority).Runner;

            weaponAnimTriggered = false;
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(guns[currentGunIndex].attackPoint.position, guns[currentGunIndex].attackRange, enemyLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                trail.GetComponent<Bullet>().moveTowards = enemy.transform.position;
            }
        }
    }
}
