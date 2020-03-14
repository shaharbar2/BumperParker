using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        // if (other.gameObject.GetComponent("CarControllerv2"))
        // {
        Debug.Log("Car is parking");
        // }
    }
}