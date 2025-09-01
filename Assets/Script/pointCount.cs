using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCount : MonoBehaviour
{
    public int points = 0;

    private void OnTriggerEnter(Collider other)
    {
        
       /* if (other.CompareTag("Point"))
        {
            points++;
            //Debug.Log("Points: " + points);
            Destroy(other.gameObject);
        }*/
    }
}
