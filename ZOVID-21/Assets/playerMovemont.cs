using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovemont : MonoBehaviour
{

    public float WalkSpeed;
    public float RunSpeed;
    public float CurrentSpeed;

    CharacterController controller;
    private Vector3 offSet;

    // Update is called once per frame
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        RunSpeed = WalkSpeed + 7;
        CurrentSpeed = WalkSpeed;
    }
    void Update()
    {

        PlayerMovement();

    }

    public bool PlayerMovement()
    {

        float hor = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        if (Input.GetMouseButton(1) || Input.GetMouseButton(0))
        {
            CurrentSpeed = WalkSpeed;
        }
        else if (((hor != 0) || (vert != 0)) && Input.GetKey("left shift"))
        {
            CurrentSpeed = RunSpeed;
        }
        else if (Input.GetKeyUp("left shift"))
        {
            CurrentSpeed = WalkSpeed;
        }


        Vector3 playerMovement = new Vector3(hor, 0f, vert) * CurrentSpeed * Time.deltaTime;

        transform.Translate(playerMovement, Space.Self);

        return true;

    }
}

