using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TokenSpawner : NetworkBehaviour
{
    [Networked] public TickTimer TokenSpawnTimer { get; private set; }

    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private NetworkPrefabRef suiTokenPrefabs = NetworkPrefabRef.Empty;

    // Start is called before the first frame update
    public override void Spawned()
    {
        TokenSpawnTimer = TickTimer.CreateFromSeconds(Runner, 10);
    }

    public override void FixedUpdateNetwork()
    {
        if(TokenSpawnTimer.Expired(Runner))
        {
            foreach(Transform t in _spawnPoints)
            {
                Runner.Spawn(suiTokenPrefabs, t.position, Quaternion.identity);
            }

            TokenSpawnTimer = TickTimer.None;
            TokenSpawnTimer = TokenSpawnTimer = TickTimer.CreateFromSeconds(Runner, 10);
        }
    }

    public void SpawnKilledTokens(Transform t)
    {
        Runner.Spawn(suiTokenPrefabs, t.position, Quaternion.identity);
    } 
}
