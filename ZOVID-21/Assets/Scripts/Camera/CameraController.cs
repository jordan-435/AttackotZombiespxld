using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    
    public GameObject[] septic;
    public float cameraHeight = 10.4f;
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
            if(gameObject.tag == "CamAngled")
            {
                transform.position = new Vector3(septic[0].transform.position.x + 14.1f, transform.position.y , septic[0].transform.position.z - 19.4f);
                transform.LookAt(septic[0].transform.position);
            }
            else
            {
                transform.position = new Vector3(septic[0].transform.position.x, 10.4f, septic[0].transform.position.z);
            }

        }
        
    }
}
