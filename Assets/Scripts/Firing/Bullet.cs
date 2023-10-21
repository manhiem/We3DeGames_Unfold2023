using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed = 20;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private float _lifeTimeAmount = 0.8f;
    [SerializeField] private int _bulletDmg = 20;

    [Networked] private TickTimer _lifeTimeTimer { get; set; }
    [Networked] private NetworkBool _didHitCol { get; set; }
    private Collider2D col;
    public Vector3 moveTowards;

    public override void Spawned()
    {
        _lifeTimeTimer = TickTimer.CreateFromSeconds(Runner, _lifeTimeAmount);
        col = GetComponent<BoxCollider2D>();
    }

    public override void FixedUpdateNetwork()
    {
        if(!_didHitCol)
        {
            CheckIfHitAPlayer();
            //CheckIfHitGround();
        }


        if(_lifeTimeTimer.ExpiredOrNotRunning(Runner) == false && !_didHitCol)
        {
            transform.Translate(transform.forward * _moveSpeed * Runner.DeltaTime, Space.World);
        }

        if(_lifeTimeTimer.Expired(Runner) || _didHitCol)
        {
            _lifeTimeTimer = TickTimer.None;
            Runner.Despawn(Object);
        }
    }

    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
    private void CheckIfHitAPlayer()
    {
        Runner.LagCompensation.OverlapBox(transform.position, col.bounds.size, Quaternion.identity, Object.InputAuthority, hits, playerLayerMask);
        transform.position = moveTowards * 10 * Runner.DeltaTime;
        if(hits.Count > 0 )
        {
            foreach(var item in hits)
            {
                if(item.Hitbox != null)
                {

                    var player = item.Hitbox.GetComponentInParent<PlayerMovementController>();
                    var didNotHitItSelf = player.Object.InputAuthority.PlayerId != Object.InputAuthority.PlayerId;

                    if(didNotHitItSelf && player._isPlayerAlive)
                    {
                        if (Runner.IsServer && didNotHitItSelf)
                        {
                            player.GetComponent<PlayerHealthManager>().Rpc_ReduceHealth(_bulletDmg);
                        }

                        _didHitCol = true;
                        break;
                    }

                }
            }
        }
    }

}
