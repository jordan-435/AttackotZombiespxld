using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    private float moveSpeed = 7;
    public float smoothMoveTime = .1f;
    public float turnSpeed = 8;

    float angle;

    float smoothInputMagnitude;
    float smoothMoveVelocity;
    Vector3 velocity;

    new Rigidbody rigidbody;
    private GameObject[] soonToBeSeptic;
    public float strikingDistance = 2f;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        moveSpeed = Random.Range(5.0f, 10.0f);


    }

    void Update()
    {
        soonToBeSeptic = GameObject.FindGameObjectsWithTag("Antiseptic");
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
        for(int i = 0; i < soonToBeSeptic.Length; i++)
        {
            if (soonToBeSeptic != null)
            {
                Vector3 positionDied = soonToBeSeptic[i].transform.position;
                if (Vector3.Distance(soonToBeSeptic[i].transform.position, transform.position) < strikingDistance)
                {
                    Destroy(soonToBeSeptic[i]);
                    Instantiate(this.gameObject, positionDied, Quaternion.identity);
                }
            }
        }


    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, strikingDistance);
    }
}