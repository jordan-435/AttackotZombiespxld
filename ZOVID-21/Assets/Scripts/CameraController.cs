using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    
    public GameObject[] septic;
    // Start is called before the first frame update
    void Start()
    {
        septic = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        septic = GameObject.FindGameObjectsWithTag("Player");
        if(septic != null)
        {

            transform.position = new Vector3(septic[0].transform.position.x, transform.position.y, septic[0].transform.position.z);
        }
        
    }
}
