using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static IEnumerator PlayAnimTillFinished(GameObject caller, Animator animator, string animName, bool activeOrNot = true)
    {
        animator.Play(animName);
        var length = animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(length);
        caller.SetActive(false);

    }

    public static bool IsLocalPlayer(NetworkObject networkObject) 
    {
        return networkObject.IsValid == networkObject.HasInputAuthority;
    }

}
