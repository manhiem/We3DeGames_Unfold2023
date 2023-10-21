using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token : NetworkBehaviour
{
    [SerializeField] private CircleCollider2D coll;
    [SerializeField] private LayerMask playerLayerMask;

    public int tokenValue = 1;

    public override void FixedUpdateNetwork()
    {
        CheckIfWeHitAPlayer();
    }

    private void CheckIfWeHitAPlayer()
    {
        var playerCollider = Runner.GetPhysicsScene2D()
            .OverlapBox(transform.position, coll.bounds.size, 0, playerLayerMask);

        if (playerCollider != default)
        {
            Debug.LogError("Called!");
            playerCollider.gameObject.GetComponent<PlayerTokenCollector>().tokens += tokenValue;
            Runner.Despawn(GetComponent<NetworkObject>());
        }
    }
}
