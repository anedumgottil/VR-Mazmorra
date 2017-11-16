using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Drone : MobileEntity {
    //public Transform[] target;
   // public float speed;
    //private int current;
    //private NavMeshAgent agent;
    // Use this for initialization

    public float inspectTime = 0.5f;
    public Transform player;
    NavMeshAgent navMeshAgent;
    NavMeshPath path;
    public float timeForNewPath;
    bool inCoRoutine;
    Vector3 target;
    bool validPath;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
    }

    void Update()
    {
        if (!inCoRoutine && navMeshAgent != null && navMeshAgent.isOnNavMesh)
            StartCoroutine(DoSomething());
    }

    Vector3 getNewRandomPosition()
    {
        float x = Random.Range(-20, 20);
        float z = Random.Range(-20, 20);

        Vector3 pos = new Vector3(x, 0, z);
        return pos;
    }

    IEnumerator DoSomething()
    {
        inCoRoutine = true;
        yield return new WaitForSeconds(timeForNewPath);
        GetNewPath();
        validPath = navMeshAgent.CalculatePath(target, path);
        if (!validPath) Debug.Log("Found an invalid Path");

        while (!validPath)
        {
            yield return new WaitForSeconds(inspectTime);
            GetNewPath();
            validPath = navMeshAgent.CalculatePath(target, path);
        }
        inCoRoutine = false;
    }

    void GetNewPath()
    {
        target = getNewRandomPosition();
        navMeshAgent.SetDestination(target);
    }

    //NEW CODE
    /*void Update()
    {
        if(Vector3.Distance(player.position, this.transform.position) < 10)
        {
            Vector3 direction = player.position - this.transform.position;
            direction.y = 0;
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 0.1f);
            if(direction.magnitude < 3)
            {
                this.transform.Translate(0, 0, 0.05f);
            }
        }
    }*/

    /*void Awake () {
         agent = GetComponent<NavMeshAgent> ();
     }

     public void MoveToLocation(Vector3 targetPoint)
     {
         agent.destination = targetPoint;
         agent.isStopped = false;
     }

     private void Update()
     {
         if (transform.position != target[current].position)
         {
             Vector3 pos = Vector3.MoveTowards(transform.position, target[current].position, speed * Time.deltaTime);
             GetComponent<Rigidbody>().MovePosition(pos);
         }
         else current = (current + 1) % target.Length;
     }*/

}
