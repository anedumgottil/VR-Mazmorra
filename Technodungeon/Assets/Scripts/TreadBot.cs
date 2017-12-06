using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TreadBot : MobileEntity {
    public GameObject deathParticleSystem = null;
    public GameObject muzzleflash = null;
    public GameObject turret = null;
    public AudioSource engineAudioSource = null;
    public AudioSource SFXAudioSource = null;
    public AudioClip trackSound = null;//found player sound
    public AudioClip randomSound = null;//this one is emitted seemingly at random
    public AudioClip driveSound = null;//emitted as the treadbot moves. should be set in it's audiosource
    public AudioClip deathSound = null;//explosion or something
    public AudioClip[] gunSounds = null;//pop pop pop

    public float fireRate = 2f;
    public float fireRange = 3f;
    public float engineStartTime = 1.0f;//acts as kind of a "debounce" for the navmesh
    public float haltRangeSensitivity = 1.0f;//once the treadbot stops, it'll require more than this sensitivity value to reactivate.
    public float haltRange = 10f;
    public float timeForNewPath;

    private NavMeshAgent navMeshAgent = null;
    private NavMeshPath path = null;
    private bool inCoRoutine = false;
    private Vector3 target;
    private bool shouldNavigate = false;
    private bool validPath = false;
    private Transform headset = null;
    private TrackingSystem ts = null;
    private float lastFireTime = 0.0f;
    private float lastEngineStartTime = 0.0f;
    private float originalHaltRange;
    private Gun turretGun = null;

    void Start()
    {
        ts = turret.GetComponentInChildren<TrackingSystem> ();
        turretGun = turret.GetComponent<Gun> ();
        if (ts == null) {
            Debug.LogError ("TreadBot: could not find tracking system!");
        }
        navMeshAgent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        headset = VRTK.VRTK_DeviceFinder.HeadsetTransform ();
        if (headset == null) {
            Debug.LogError ("TreadBot: could not find headset!");
        }
        originalHaltRange = haltRange;
        turretGun.setFlashlightState (false);
    }

    void Update()
    {
        if (headset == null) {
            return;
        }
        if (Vector3.Distance (headset.position, this.transform.position) < haltRange) {
            //Debug.Log ("Stopping "+Vector3.Distance (headset.position, this.transform.position));
            shouldNavigate = false;
            navMeshAgent.isStopped = true;
            haltRange = originalHaltRange + haltRangeSensitivity;
            if (engineAudioSource.isPlaying) {
                
                engineAudioSource.Stop ();
            }

            turretGun.setFlashlightState (true);

            if (Random.Range (0, 3000) == 42) {
                SFXAudioSource.PlayOneShot (trackSound);
            }

            ts.setTarget (headset.position);
            ts.shouldTrack (true);
            //check if we can shoot the player yet
            RaycastHit rhit;
            if (Physics.Raycast (turret.transform.position, transform.forward, out rhit, fireRange)) {
                if (Vector3.Distance (headset.position, this.transform.position) < fireRange) {
                    //if (rhit.collider.gameObject.name.Contains ("Camera") || rhit.collider.gameObject.name.Contains ("Controller")) {
                    //we hit player;

                    this.fire ();
                    //}
                }
            }
        } else if (!inCoRoutine && navMeshAgent != null && navMeshAgent.isOnNavMesh && (Time.time - engineStartTime >= lastEngineStartTime)) {
            haltRange = originalHaltRange;
            shouldNavigate = true;
            StartCoroutine(DoNavigate());
            if (!engineAudioSource.isPlaying) {
                engineAudioSource.Play ();
            }
            ts.shouldTrack (false);
            turretGun.setFlashlightState (false);
            lastEngineStartTime = Time.time;
        }

    }

    IEnumerator DoNavigate()
    {
        inCoRoutine = true;
        if (isAlive() && shouldNavigate) {
            yield return new WaitForSeconds (timeForNewPath);
            if (Random.Range (0, 1000) == 42) {
                SFXAudioSource.volume = 0.5f;
                SFXAudioSource.PlayOneShot (randomSound);
            }
            GetNewPath ();
            validPath = navMeshAgent.CalculatePath (target, path);
            if (!validPath) {
                //Debug.Log ("Found an invalid Path"); TODO fix this!
            }
                
        }
        inCoRoutine = false;
    }

    public override void die() {
        this.alive = false; 
        this.navMeshAgent.enabled = false;
        Rigidbody rb = this.GetComponent<Rigidbody> ();
        if (rb != null) {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        SFXAudioSource.PlayOneShot (deathSound);
        engineAudioSource.Stop ();
        //Renderer rend = this.GetComponent<Renderer> ();
        //rend.material.color = Color.black;

        if (deathParticleSystem != null) {
            deathParticleSystem.SetActive (true);
        }

        Destroy (this.gameObject, 5f);
    }

    public void fire() {
        if (Time.time - fireRate >= lastFireTime) {
            turretGun.fireBullet ();

            lastFireTime = Time.time;
        }
    }

    void GetNewPath()
    {
        if (isAlive ()) {
            target = Vector3.MoveTowards (this.transform.position, headset.position, haltRange);//TODO; this halt range stuff doesn't work.
            navMeshAgent.SetDestination (target);
        }
    }
        
    public void MoveToLocation(Vector3 targetPoint)
    {
        navMeshAgent.destination = targetPoint;
        navMeshAgent.isStopped = false;
    }
}
