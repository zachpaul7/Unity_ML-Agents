using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triger_test : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("������ ������");
        if (other.gameObject.CompareTag("street"))
        {
            print("street");
        }
        if (other.gameObject.CompareTag("floor"))
        {
            print("floor");
        }
    }

    //private void OnCollisionEnter(Collision other)
    //{
    //    Debug.Log("������ ������");
    //    if (other.gameObject.CompareTag("street"))
    //    {
    //        print("street");
    //    }
    //    if (other.gameObject.CompareTag("floor"))
    //    {
    //        print("floor");
    //    }
    //}
}
