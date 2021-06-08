using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class guardMovement : MonoBehaviour
{
    public float speed = 5;
    public float waitTime = .3f;
    public float turnspeed = 90;
    public float timeToSpotPlayer = .5f;
    float playerVisibleTimer;
    int whoGotCaught;
    public Transform pathHolder;
    public GameObject[] player;

    public Light spotlight;
    public float viewDistance;
    public LayerMask viewMask;
    float viewAngle;
    Color originalSpotlightColor;

    public Animator anim;

    private void Start()
    {
        originalSpotlightColor = spotlight.color;
        viewAngle = spotlight.spotAngle;

        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for(int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }
        StartCoroutine(FollowPath(waypoints));

    }
    void Update()
    {
        player = GameObject.FindGameObjectsWithTag("Player");
        if (player != null)
        {
            if (CanSeePlayer())
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
            if(playerVisibleTimer >= timeToSpotPlayer)
            {
                BlastThatMf(whoGotCaught);
            }
        }
        else
        {
            spotlight.color = originalSpotlightColor;
        }
        
    }

    void BlastThatMf(int zombieTagged)
    {
        Destroy(player[zombieTagged]);
    }


    bool CanSeePlayer()
    {
        for(int i = 0; i < player.Length; i++)
        {
            if (Vector3.Distance(transform.position, player[i].transform.position) < viewDistance)
            {
                Vector3 dirToPlayer = (player[i].transform.position - transform.position).normalized;
                float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
                if (angleBetweenGuardAndPlayer < viewAngle / 2f)
                {
                    if (!Physics.Linecast(transform.position, player[i].transform.position, viewMask))
                    {
                        whoGotCaught = i;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];
        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            anim.SetFloat("vertical", 1);
            if (transform.position == targetWaypoint)
            {
                Debug.Log("Here!!!!!");
                anim.SetFloat("vertical", 0);
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
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}
