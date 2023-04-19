using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GivePlayerWeapon : MonoBehaviour
{
    public Weapon weapon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerFireControl fireControl = other.gameObject.GetComponent<PlayerFireControl>();
            if (fireControl.weapon != weapon)
                fireControl.SwitchWeapon(weapon);
        }
    }
}
