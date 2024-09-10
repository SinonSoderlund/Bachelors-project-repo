using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieVisionedDetctor : MonoBehaviour
{
    internal static ZombieVisionedDetctor ZVD;
    [SerializeField] private bool HideCone = true;
    public bool isConeGide { get => !HideCone; }
    //Shows vision cone on trigger enter
    private void Awake()
    {
        if (ZVD == null)
            ZVD = this;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!HideCone)
            return;
        if(collision.tag == "Zombie")
        {
            collision.GetComponent<ZombieLook>().coneActiveToggle(true);
        }
    }
    //hide it on exit
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Zombie")
        {
            if (!HideCone)
                return;
            collision.GetComponent<ZombieLook>().coneActiveToggle(false);
        }
    }
}
