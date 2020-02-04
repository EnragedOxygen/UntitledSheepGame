using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "SheepAgent")
        {
            other.gameObject.GetComponent<SheepBehaviour>().KillSheep();
        }
    }
}
