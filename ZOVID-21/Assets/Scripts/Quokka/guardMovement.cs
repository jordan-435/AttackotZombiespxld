using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class guardMovement : MonoBehaviour
{

    
    //Specific Zombie 
    int whoGotCaught;
    public GameObject[] player;

    public bool guardDead = false;

    //Recognize Zombie
    public Light spotlight;
    Color originalSpotlightColor;
    public float timeToSpotPlayer = .5f;
    float playerVisibleTimer;
    public float viewDistanceFar = 8;
    public float viewDistanceMid;
    public float viewDistanceClose;
    public float immediateDistance = 2;
    

    //Guard Movement
    Coroutine walkPath;
    public LayerMask viewMask;
    float viewAngle;
    public Transform pathHolder;
    public float speed = 5;
    public float waitTime = .3f;
    public float turnspeed = 90;

    public Animator anim;

    //HealthBar Stuff
    public HealthBarQ hBar;
    int MaxHealth = 1000;
    int currentHealth;
    


    //Weapon Damage
    public int HitHowHard = 5;
    public float fireRate = 15f;
    private float nextTimeToFire = 0f;


    private void Start()
    {
        currentHealth = MaxHealth;
        hBar.SetMaxHealth(MaxHealth);
        viewDistanceMid = viewDistanceFar - (viewDistanceFar / 8f);
        viewDistanceClose = viewDistanceMid - (viewDistanceMid / 4f);
        originalSpotlightColor = spotlight.color;
        viewAngle = spotlight.spotAngle;

        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for(int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }
        walkPath = StartCoroutine(FollowPath(waypoints));
    }
    void Update()
    {
        player = GameObject.FindGameObjectsWithTag("Player");
        if (player != null)
        {
            if (CanSeePlayer() != 0)
            {
                spotlight.color = Color.red;
                playerVisibleTimer += Time.deltaTime;
            }
            else
            {
                playerVisibleTimer -= Time.deltaTime;              
            }
            playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
            spotlight.color = Color.Lerp(originalSpotlightColor, Color.red, playerVisibleTimer / timeToSpotPlayer);
        }
        else
        {
            spotlight.color = originalSpotlightColor;
        }
        
    }

    void StepTowards(int zombieTagged)
    {
        anim.SetBool("Aim", true);
        transform.LookAt(player[zombieTagged].transform.position);
        transform.position = Vector3.MoveTowards(transform.position, player[zombieTagged].transform.position, speed * Time.deltaTime);
    }

    void Kill(int zombieTagged)
    {
        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, viewDistanceClose))
            {
                playerMovement target = hit.transform.GetComponent<playerMovement>();
                if (target != null)
                {
                    target.TakeDamage(HitHowHard);
                }
            }
        }
     //   CheckIfAnotherZombieIsCloser(zombieTagged);
     //   transform.LookAt(player[zombieTagged].transform.position);
     //   anim.SetBool("fire", true);

    }

    void CheckIfAnotherZombieIsCloser( int currentZombieLockedIn)
    {
        if(CanSeePlayer(currentZombieLockedIn) == 3)
        {
            Kill(whoGotCaught);
        }
    }



    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        hBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        anim.SetBool("Die", true);
        StopCoroutine(walkPath);
        guardDead = true;
        Invoke("Dissapear", 5);
    }

    void Dissapear()
    {
        Destroy(gameObject);  
    }

    int CanSeePlayer()
    {
        for(int i = 0; i < player.Length; i++)
        {
            if (Vector3.Distance(transform.position, player[i].transform.position) <= viewDistanceFar)
            {
                Vector3 dirToPlayer = (player[i].transform.position - transform.position).normalized;
                float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
                if (angleBetweenGuardAndPlayer < viewAngle / 2f)
                {
                    if (!Physics.Linecast(transform.position, player[i].transform.position, viewMask))
                    {
                        if (Vector3.Distance(transform.position, player[i].transform.position) <= viewDistanceMid)
                        {
                            if(Vector3.Distance(transform.position, player[i].transform.position) <= viewDistanceClose)
                            {
                                return 3;
                            }
                            return 2;
                        }
                        
                        whoGotCaught = i;
                        return 1;
                    }
                }
            }
            if (Vector3.Distance(transform.position, player[i].transform.position) <= immediateDistance)
            {
                    if (!Physics.Linecast(transform.position, player[i].transform.position, viewMask))
                    {
                        whoGotCaught = i;
                        return 3;
                    }  
            }
        }
        return 0;
    }


    int CanSeePlayer(int playerAlreadyTagged)
    {
        for (int i = 0; i < player.Length; i++)
        {
            if (playerAlreadyTagged == i) continue;

            if (Vector3.Distance(transform.position, player[i].transform.position) <= viewDistanceFar)
            {
                Vector3 dirToPlayer = (player[i].transform.position - transform.position).normalized;
                float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
                if (angleBetweenGuardAndPlayer < viewAngle / 2f)
                {
                    if (!Physics.Linecast(transform.position, player[i].transform.position, viewMask))
                    {
                        if (Vector3.Distance(transform.position, player[i].transform.position) <= viewDistanceMid)
                        {
                            if (Vector3.Distance(transform.position, player[i].transform.position) <= viewDistanceClose)
                            {
                                return 3;
                            }
                            return 2;
                        }

                        whoGotCaught = i;
                        return 1;
                    }
                }
            }
            if (Vector3.Distance(transform.position, player[i].transform.position) <= immediateDistance)
            {
                if (!Physics.Linecast(transform.position, player[i].transform.position, viewMask))
                {
                    whoGotCaught = i;
                    return 3;
                }
            }
        }
        return 0;
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];
        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while (true)
        {
            if (CanSeePlayer() == 1)
            {
                float stop = 0;
                transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, stop * Time.deltaTime);
                transform.LookAt(player[whoGotCaught].transform.position);
                anim.SetBool("Aim", true);
            }
            else if (CanSeePlayer() == 2)
            {
                StepTowards(whoGotCaught);
            }
            else if(CanSeePlayer() == 3)
            {
                Kill(whoGotCaught);
            }
            else
            {
                anim.SetBool("Aim", false);
                anim.SetBool("fire", false);
                transform.LookAt(targetWaypoint);
                transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
                anim.SetFloat("vertical", 1);
            }
           
            if (transform.position == targetWaypoint)
            {
                anim.SetFloat("vertical", 0);
                anim.SetBool("turn", true);
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }
            yield return null;
        }
    }

    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnspeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
        anim.SetBool("turn", false);
    }








































    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach (Transform waypoint in pathHolder) 
        {
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistanceFar);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistanceClose);
    }
}
