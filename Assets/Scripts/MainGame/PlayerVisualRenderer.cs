using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualRenderer : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    private bool init;

    private void Start()
    {
        init = true;
    }

    public void TriggerDieAnimation()
    {
        anim.SetTrigger("Dead");
    }
    
    public void TriggerRespawnAnimation()
    {
        anim.SetTrigger("Respawn");
    }


}
