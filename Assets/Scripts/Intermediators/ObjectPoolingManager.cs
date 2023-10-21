using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager : MonoBehaviour, INetworkObjectPool
{
    private Dictionary<NetworkObject, List<NetworkObject>> poolingObjects = new();

    private void Start()
    {
        if(GlobalManagers.Instance != null)
        {
            GlobalManagers.Instance.objectPoolingManager = this;
        }
    }
    public NetworkObject AcquireInstance(NetworkRunner runner, NetworkPrefabInfo info)
    {
        NetworkObject networkObject = null;
        NetworkProjectConfig.Global.PrefabTable.TryGetPrefab(info.Prefab, out var prefab);
        poolingObjects.TryGetValue(prefab, out var networkObjects);

        bool foundMatch = false;
        if(networkObjects.Count > 0)
        {
            foreach(var item in networkObjects)
            {
                if(item != null && item.gameObject.activeSelf == false)
                {
                    networkObject = item;
                    foundMatch = true;
                    break;
                }
            }
        }

        if(foundMatch)
        {
            networkObject = CreateObjectInstance(prefab);
        }

        return networkObject;
    }

    private NetworkObject CreateObjectInstance(NetworkObject prefab)
    {
        var obj = Instantiate(prefab);

        if(poolingObjects.TryGetValue(prefab, out var instanceData))
        {
            instanceData.Add(obj);
        }
        else
        {
            var list = new List<NetworkObject>() { obj };
            poolingObjects.Add(prefab, list);
        }

        return obj;
    }

    public void ReleaseInstance(NetworkRunner runner, NetworkObject instance, bool isSceneObject)
    {
        instance.gameObject.SetActive(false);
    }

    public void RemoveNetworkObjectFromDic(NetworkObject obj)
    {
        if(poolingObjects.Count > 0)
        {
            foreach (var item in poolingObjects)
            {
                foreach(var networkObject in item.Value)
                {
                    if(networkObject == obj)
                    {
                        item.Value.Remove(networkObject);
                        break;
                    }
                }   
            }
        }
    }


}
