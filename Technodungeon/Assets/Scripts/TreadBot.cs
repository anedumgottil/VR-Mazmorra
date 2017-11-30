using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TreadBot : MobileEntity {
    public GameObject deathParticleSystem;
    public GameObject muzzleflash;
    public GameObject turret = null;
    public AudioClip trackSound = null;//found player
    public AudioClip randomSound = null;
    public AudioClip driveSound = null;
    public AudioClip deathSound = null;
    public AudioClip gunSound = null;

    NavMeshAgent navMeshAgent;
    NavMeshPath path;
    public float timeForNewPath;
    bool inCoRoutine;
    Vector3 target;
    bool validPath;
    Transform headset;
    TrackingSystem ts = null;
    AudioSource audioSource = null;
    public float lastFireTime = 0.0f;

    void Start()
    {
        ts = this.GetComponentInChildren<TrackingSystem> ();
        if (ts == null) {
            Debug.LogError ("TreadBot: could not find tracking system!");
            this.gameObject.SetActive (false);
        }
        navMeshAgent = GetComponent<NavMeshAgent>();
        audioSource = this.GetComponent<AudioSource> ();
        path = new NavMeshPath();
        headset = VRTK.VRTK_DeviceFinder.HeadsetTransform ();
        if (headset == null) {
            Debug.LogError ("TreadBot: could not find headset!");
        }
    }

    void Update()
    {
        if (!inCoRoutine && navMeshAgent != null && navMeshAgent.isOnNavMesh)
            StartCoroutine(DoNavigate());

        if (Vector3.Distance (headset.position, this.transform.position) < 10) {
            if (Random.Range (0, 3000) == 42) {
                audioSource.PlayOneShot (trackSound);
            }
            ts.setTarget (headset.position);
            ts.shouldTrack (true);
            //check if we can shoot the player yet
            RaycastHit rhit;
            //if (Physics.Raycast (turret.transform.position, transform.forward, out rhit, 20f)) {
            if (Vector3.Distance (headset.position, this.transform.position) < 1) {
                //if (rhit.collider.gameObject.name.Contains ("Camera") || rhit.collider.gameObject.name.Contains ("Controller")) {
                    //we hit player;

                    this.fire ();
                //}
            }
        } else {
            ts.shouldTrack (false);
        }

    }

    IEnumerator DoNavigate()
    {
        inCoRoutine = true;
        if (isAlive()) {
            yield return new WaitForSeconds (timeForNewPath);
            if (Random.Range (0, 1000) == 42) {
                audioSource.volume = 0.5f;
                audioSource.PlayOneShot (randomSound);
            }
            GetNewPath ();
            validPath = navMeshAgent.CalculatePath (target, path);
            if (!validPath) {
                //Debug.Log ("Found an invalid Path"); TODO fix this!
            }
                
        }
        inCoRoutine = false;
    }

    void GetNewPath()
    {
        if (isAlive ()) {
            target = headset.position;
            navMeshAgent.SetDestination (target);
        }
    }

    public override void die() {
        this.alive = false; 
        this.navMeshAgent.enabled = false;
        Rigidbody rb = this.GetComponent<Rigidbody> ();
        if (rb != null) {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        audioSource.PlayOneShot (deathSound);
        //Renderer rend = this.GetComponent<Renderer> ();
        //rend.material.color = Color.black;

        if (deathParticleSystem != null) {
            deathParticleSystem.SetActive (true);
        }

        Destroy (this.gameObject, 5f);
    }

    public void fire() {
        if (Time.time - 3f >= lastFireTime) {
            muzzleflash.GetComponent<ParticleSystem> ().Play ();
            this.GetComponent<AudioSource> ().PlayOneShot (gunSound);
            lastFireTime = Time.time;
        }
    }
        
    public void MoveToLocation(Vector3 targetPoint)
    {
        navMeshAgent.destination = targetPoint;
        navMeshAgent.isStopped = false;
    }
}
