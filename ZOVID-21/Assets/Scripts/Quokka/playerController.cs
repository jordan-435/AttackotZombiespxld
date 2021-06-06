using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public Animator anim;
    private Rigidbody rb;
    public float sidewaySpeed = 0.05f;
    public LayerMask layerMsk;
    public bool grounded;
    // Start is called before the first frame update
    void Start()
    {
        this.rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Grounded();
        Jump();
        Move();
    }

    private void Grounded()
    {
        if(Physics.CheckSphere(this.transform.position + Vector3.down, 0.2f, layerMsk))
        {
            this.grounded = true;
        }
        else
        {
            this.grounded = false;
        }

        anim.SetBool("jump", this.grounded);
    }



    private void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && this.grounded)
        {
            this.rb.AddForce(Vector3.up * 4, ForceMode.Impulse);
        }

    }

    private void Move()
    {
        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalAxis = Input.GetAxis("Horizontal");

        Vector3 movement = (this.transform.forward * verticalAxis) + (this.transform.right * horizontalAxis);
        movement.Normalize();

        if (horizontalAxis != 0)
        {
            if (verticalAxis != 0)
            {
                this.transform.position += movement * 0.003f;
            }
            else
            {
                this.transform.position += movement * 0.0035f;
            }    
        }
        else
        {
            this.transform.position += movement * 0.002f;
        }


        anim.SetFloat("vertical", verticalAxis);
        anim.SetFloat("horizontal", horizontalAxis);
    }
}
