using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMove : MonoBehaviour
{
    [SerializeField]
    private GameObject parentGO;

    private void Update()
    {
        this.transform.localRotation = parentGO.transform.localRotation;
    }
}
