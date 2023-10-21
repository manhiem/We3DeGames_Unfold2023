using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitch : NetworkBehaviour
{
    public List<GameObject> weapons = new List<GameObject>();

    [Networked(OnChanged = nameof(OnWeaponChanged))]
    public int selectedWeapon { get; set; }

    [SerializeField] private PlayerWeaponController _weaponScript;

    private void Start()
    {
        //SelectWeapon(selectedWeapon);
    }

    public override void FixedUpdateNetwork()
    {
        if (Runner.TryGetInputForPlayer<PlayerData>(Object.InputAuthority, out var input))
        {
            CheckMouseScroll(input);
            CheckNumberKeys(input);
        }
    }

    private void CheckMouseScroll(PlayerData input)
    {
        float scrollInput = input._scrollWheel;
        if (scrollInput > 0)
        {
            CycleWeapon(1);
        }
        else if (scrollInput < 0)
        {
            CycleWeapon(-1);
        }
    }

    private void CheckNumberKeys(PlayerData input)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            KeyCode key = KeyCode.Alpha1 + i;
            if (input.networkButtons.WasPressed(input.networkButtons, key) && i < weapons.Count)
            {
                selectedWeapon = i;
            }
        }
    }

    private void CycleWeapon(int direction)
    {
        selectedWeapon += direction;
        selectedWeapon = Mathf.Clamp(selectedWeapon, 0, weapons.Count - 1);
    }

    private static void OnWeaponChanged(Changed<WeaponSwitch> changed)
    {
        var currentWeapon = changed.Behaviour.selectedWeapon;
        changed.LoadOld();
        var currentWeaponOld = changed.Behaviour.selectedWeapon;

        if (currentWeaponOld != currentWeapon)
        {
            changed.Behaviour.SelectWeapon(currentWeapon);
        }
    }

    private void SelectWeapon(int index)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].SetActive(i == index);
        }
        _weaponScript.SetGunType(index);
    }
}
