using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{

    public float moveSpeed = 7;
    public float smoothMoveTime = .1f;
    public float turnSpeed = 8;

    float angle;

    float smoothInputMagnitude;
    float smoothMoveVelocity;
    Vector3 velocity;

    new Rigidbody rigidbody;
    private GameObject soonToBeSeptic;
    public float strikingDistance = 2f;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        soonToBeSeptic = GameObject.FindGameObjectWithTag("Antiseptic");


    }

    void Update()
    {
        Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);

        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);

        velocity = transform.forward * moveSpeed * smoothInputMagnitude;
        CheckStrikingRange();
    }

    void FixedUpdate()
    {
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);
    }

    void CheckStrikingRange()
    {
        if(soonToBeSeptic != null)
        {
            Vector3 positionDied = soonToBeSeptic.transform.position;
            if (Vector3.Distance(soonToBeSeptic.transform.position, transform.position) < strikingDistance)
            {
                Debug.Log("Here!!!");
                Destroy(soonToBeSeptic);
                Instantiate(this.gameObject, positionDied, Quaternion.identity);
            }
        }

    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, strikingDistance);
    }
}