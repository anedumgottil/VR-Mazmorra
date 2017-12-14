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
    public AudioClip componentDamageSound = null;//this one is emitted when one of it's entityparts receive enough damage to die
    public AudioClip driveSound = null;//emitted as the treadbot moves. should be set in it's audiosource
    public AudioClip deathSound = null;//explosion or something
    public AudioClip armorDeathSound = null;//armor piece destroyed sound
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
    private Transform headset = null;
    private TrackingSystem ts = null;
    private float lastFireTime = 0.0f;
    private float lastEngineStartTime = 0.0f;
    private float originalHaltRange;
    private Gun turretGun = null;
    private bool turretDisabled = false;
    private bool treadsDisabled = false;

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
        //initialize tracking system
        ts.setTarget (Player.getInstance ().getCollider().gameObject, fireRange);
        ts.addExtraTargetByName ("Player");
        ts.addExtraTargetByName ("Controller R");
        ts.addExtraTargetByName ("Controller L");//TODO: controller detection untested.
        turretGun.setFlashlightState (false);

        animator = this.GetComponent<Animator> ();
        if (animator == null) {
            Debug.LogError ("TreadBot: could not find animator component!");
        }
        animator.SetTrigger ("Moving");
    }

    void Update()
    {
        if (headset == null) {
            return;
        }
        //we're within range of the player:
        if (Vector3.Distance (headset.position, this.transform.position) < haltRange) {
            //Debug.Log ("Stopping "+Vector3.Distance (headset.position, this.transform.position));
            animator.SetTrigger ("Tracking");
            //we're not in stopping range of the player, start the engines:
        } else if (!inCoRoutine && navMeshAgent != null && navMeshAgent.isOnNavMesh && (Time.time - engineStartTime >= lastEngineStartTime) && !treadsDisabled) {
            animator.SetTrigger ("Moving");
        }

    }

    IEnumerator DoNavigate()
    {
        inCoRoutine = true;
        if (isAlive() && shouldNavigate && !treadsDisabled) {
            yield return new WaitForSeconds (timeForNewPath);
            GetNewPath ();
            navMeshAgent.CalculatePath (target, path);
                
        }
        inCoRoutine = false;
    }

    //this function handles all of the state-driven logic for our Entity, if it has any.
    public override void StateMachineEvent(string stateAction, string stateName, Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        ///////////////////////// ENTER //////////////////////////
        if (stateAction == "Enter") {
            switch (stateName) {
            case "MoveToTarget":
                if (!engineAudioSource.isPlaying) {
                    engineAudioSource.Play ();
                }
                break;
            case "TrackTarget":
                shouldNavigate = false;
                if (navMeshAgent.isOnNavMesh)
                    navMeshAgent.isStopped = true;
                if (engineAudioSource.isPlaying) {
                    engineAudioSource.Stop ();
                    SFXAudioSource.PlayOneShot (trackSound);
                }
                break;
            case "ShootTarget":
                if (ts.targetAcquired ()) {
                    this.fire ();
                }
                break;
            default:
                Debug.LogWarning ("TreadBot: Warning, TreadBot changed to an unhandled FSM Enter state");
                break;
            }
        }
        ///////////////////////// UPDATE //////////////////////////
        if (stateAction == "Update") {
            switch (stateName) {
            case "MoveToTarget":
                if (!inCoRoutine && navMeshAgent != null && navMeshAgent.isOnNavMesh && (Time.time - engineStartTime >= lastEngineStartTime) && !treadsDisabled) {
                    haltRange = originalHaltRange;
                    shouldNavigate = true;
                    StartCoroutine (DoNavigate ());
                    ts.shouldTrack (false);
                    turretGun.setFlashlightState (false);
                    lastEngineStartTime = Time.time;
                }
                break;
            case "TrackTarget":
                haltRange = originalHaltRange + haltRangeSensitivity;
                //rotate turret
                if (!turretDisabled) {
                    turretGun.setFlashlightState (true);
                    ts.shouldTrack (true);
                    //check if we can shoot the player yet
                    if (Vector3.Distance (headset.position, this.transform.position) < fireRange && ts.targetAcquired ()) {
                        //we're within firing range of the player
                        animator.SetTrigger ("Shooting");
                    } else if (Vector3.Distance (headset.position, this.transform.position) > fireRange) {
                        //outside of firing range, move
                        animator.SetTrigger ("Moving");
                    }
                }
                break;
            case "ShootTarget":
                if (Vector3.Distance (headset.position, this.transform.position) < fireRange && ts.targetAcquired ()) {
                    this.fire ();
                } else if (Vector3.Distance (headset.position, this.transform.position) > fireRange) {
                    //outside of firing range, move
                    animator.SetTrigger ("Moving");
                } else {
                    //target not locked, track
                    animator.SetTrigger ("Tracking");
                }
                break;
            default:
                Debug.LogWarning ("TreadBot: Warning, TreadBot changed to an unhandled FSM Update state");
                break;
            }
        }
        ///////////////////////// EXIT //////////////////////////
        if (stateAction == "Exit") {
            switch (stateName) {
            case "MoveToTarget":
                break;
            case "TrackTarget":
                break;
            case "ShootTarget":
                break;
            default:
                Debug.LogWarning ("TreadBot: Warning, TreadBot changed to an unhandled FSM Exit state");
                break;
            }
        }
    }

    public override void damage (GameObject damageCause, int damageAmount) {
        base.damage (damageCause, damageAmount);

        //some damageCauses only trigger once - when a EntityPart of ours dies. Handle these:
        if (damageCause.transform.IsChildOf (this.transform) && damageCause.name.Contains ("Armor")) {//one of our armor plates broke.
            if (armorDeathSound != null)
                SFXAudioSource.PlayOneShot (armorDeathSound);
            //Treads and MainBody have their colliders off by default, but now that the armor is detached/killed, we should turn them on to receive damage.
            BoxCollider bodyCollider = null;
            if (damageCause.name.Equals ("TreadArmor L")) {
                bodyCollider = this.transform.Find ("Tread L").GetComponentInChildren<BoxCollider> ();
                bodyCollider.enabled = true;
            } else if (damageCause.name.Equals ("TreadArmor R")) {
                bodyCollider = this.transform.Find ("Tread R").GetComponentInChildren<BoxCollider> ();
                bodyCollider.enabled = true;
            } else if (damageCause.name.Equals ("FrontArmor")) {
                bodyCollider = this.transform.Find ("MainBody").GetComponentInChildren<BoxCollider> ();
                bodyCollider.enabled = true;
            } else {
                Debug.LogWarning("TreadBot: damage: could not handle armor of type: "+damageCause.name);
                //some other armor part.
            }
        }
        if (damageCause.transform.IsChildOf (this.transform) && damageCause.name.Contains ("Tread") && !damageCause.name.Contains ("Armor")) {//one of our actual Treads broke.
            if (componentDamageSound != null)
                SFXAudioSource.PlayOneShot (componentDamageSound);
            disableTreads ();
        }
        if (damageCause.transform.IsChildOf (this.transform) && damageCause.name.Contains ("Turret")) {//our turret broke.
            if (componentDamageSound != null)
                SFXAudioSource.PlayOneShot (componentDamageSound);
            disableTurret ();
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
        SFXAudioSource.PlayOneShot (deathSound);
        engineAudioSource.Stop ();
        //Renderer rend = this.turret.GetComponentInChildren<Renderer> ();
        //rend.material.color = Color.black;

        if (deathParticleSystem != null) {
            deathParticleSystem.SetActive (true);
        }

        Destroy (this.gameObject, 5f);
    }

    public void fire() {
        if (turretDisabled) {
            return;
        }
        if (Time.time - fireRate >= lastFireTime) {
            turretGun.fireBullet ();

            lastFireTime = Time.time;
        }
    }

    void GetNewPath()
    {
        if (isAlive ()) {
            target = Vector3.MoveTowards (this.transform.position, headset.position, haltRange);
            navMeshAgent.SetDestination (target);
        }
    }
        
    public void MoveToLocation(Vector3 targetPoint)
    {
        navMeshAgent.destination = targetPoint;
        navMeshAgent.isStopped = false;
    }

    //tells the TreadBot it's Turret is destroyed or otherwise out of service
    public void disableTurret() {
        this.turretDisabled = true;
        //play a disable sound?
        //play disable particles?
        ts.shouldTrack (false);
        ts.enabled = false;//disable tracking system for good.
        //knocks out it's light
        turretGun.setFlashlightState (false);

        //set up the turret VRTK interectable object to make it work when the player goes to grab it.
        turretGun.isGrabbable = true;
        turretGun.isUsable = true;
        turretGun.touchHighlightColor = Color.magenta;
    }

    //tells the TreadBot it's Treads/engine is destroyed or otherwise out of service
    public void disableTreads() {
        this.treadsDisabled = true;
        //play a disable sound?
        //play disable particles?
        this.engineAudioSource.Stop();
        navMeshAgent.isStopped = true;
        this.engineAudioSource.mute = true;
        shouldNavigate = false;
        //no engine sounds play when it's treads are disabled.
    }
}
