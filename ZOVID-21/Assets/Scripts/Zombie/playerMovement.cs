using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    //Player Movement
    private float moveSpeed = 7;
    public float smoothMoveTime = .1f;
    public float turnSpeed = 8;
    float smoothInputMagnitude;
    float smoothMoveVelocity;
    Vector3 velocity;
    float angle;

    bool instantiated = false;

 

    new Rigidbody rigidbody;
    private GameObject[] soonToBeSeptic;
    private guardMovement[] guard;
    public GameObject[] septic;


    //HealthBar Stuff
    public HealthBarZ zhBar;
    public int currentHealth;
    int MaxHealth = 1000;

    //Attack Skill
    public int damage = 10;
    public float strikingDistance = 1.25f;
    public float AttachDistance;

    public Animator anim;


    void Awake(){
        guard = GameObject.FindObjectsOfType<guardMovement>();
        septic = GameObject.FindGameObjectsWithTag("Player");

        //Scale the Zombie(copy) by 15
        Vector3 scale = transform.localScale;
        scale.y = 15F; // your new value
        scale.z = 15F; // your new value
        scale.x = 15F; // your new value
        transform.localScale = scale;

    }

    void Start()
    {
        currentHealth = MaxHealth;
        zhBar.SetMaxHealth(MaxHealth);
        zhBar.SetHealth(MaxHealth);
        rigidbody = GetComponent<Rigidbody>();
        moveSpeed = Random.Range(5.0f, 10.0f);
        AttachDistance = Random.Range(1.0f, 4.0f);



    }

    void Update()

    {   //Actions taken when a zombie is extantiated

        if (septic.Length != 1)
        {
            

            if(Vector3.Distance(transform.position, septic[0].transform.position) > AttachDistance)
            {
                transform.position =  Vector3.MoveTowards(transform.position, septic[0].transform.position, 3 * Time.deltaTime);
            }
        }

        soonToBeSeptic = GameObject.FindGameObjectsWithTag("Antiseptic");
        
        PlayerMControls();
        CheckStrikingRange();
    }

    void FixedUpdate()
    {
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);
    }

    void PlayerMControls()
    {
        Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);

        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);

        velocity = transform.forward * moveSpeed * smoothInputMagnitude;



        //Zombie Movement Animation
        if ((Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0) || (Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0))
        {
            //Zombie 1
            anim.SetFloat("Movement", 1);
            
        }
        else
        {
            anim.SetFloat("Movement", 0);
        }
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
                    Kill(i, positionDied);
                }
                else {
                    anim.SetBool("Attack", false);
                
                }
                    
            }
            if (guard[i].guardDead == true)
            {
                StartCoroutine(CreateNewLife(soonToBeSeptic[i].transform.position));
            }
        }
    }

    void Kill(int guardTagged, Vector3 positionDied)
    {
        guardMovement target = soonToBeSeptic[guardTagged].transform.GetComponent<guardMovement>();
        if (target != null)
        {
            // Zombie attack animation
            anim.SetBool("Attack", true);


            // Guard is taking damage
            target.TakeDamage(damage);
        }
        if (guard[guardTagged].guardDead == true)
        {
            StartCoroutine(CreateNewLife(positionDied));
        }
    }

    public void TakeDamage(int amount)
    {        
        
        currentHealth -= amount;
        zhBar.SetHealth(currentHealth);
        if (currentHealth <= 0f)
        {
            Die();
        }           
    }

    void Die()
    {
        Destroy(gameObject);
    }

    IEnumerator CreateNewLife(Vector3 lookTarget)
    {  
        yield return new WaitForSeconds(5);
        if(instantiated == false)
        {

            Instantiate(this.gameObject, lookTarget, Quaternion.identity);
            instantiated = true;

            
        }  
    }















    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, strikingDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, AttachDistance);
    }
}