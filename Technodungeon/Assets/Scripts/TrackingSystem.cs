using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackingSystem : MonoBehaviour {
    public static bool isDebug = false;//set to on to output debug information

	public float speed = 10.0f;
    public bool rotateXAxis = true;
    public bool rotateYAxis = true;
    public bool rotateZAxis = true;
    public float dampingPeriod = 0.01f; //how much to wait before tracking again. prevents rapid oscillations.

	GameObject m_target = null;
    Vector3 m_target_vector;
	Vector3 m_lastKnownPosition = Vector3.zero;
	Quaternion m_lookAtRotation;
    private float m_range;
    private bool track = false;
    private bool hasTargetLock = false;
    private List<string> extraTargets;
    private float lastTrackTime = 0;

    void Awake () {
        extraTargets = new List<string> ();
        lastTrackTime = dampingPeriod;
        track = false;
    }
	// Update is called once per frame
	void Update () {
        if (track && Time.time > lastTrackTime + dampingPeriod) {
            if (m_target != null)
                m_target_vector = m_target.transform.position;
            if (m_lastKnownPosition != m_target_vector) {
                if (isDebug)
                    Debug.Log ("DEBUG: TrackingSystem: ("+this.gameObject.name+") Updating last known position vector.");
                m_lastKnownPosition = m_target_vector;
                m_lookAtRotation = Quaternion.LookRotation (m_lastKnownPosition - transform.position);
            }
            Vector3 lookAtRotEulers = m_lookAtRotation.eulerAngles;
            if (transform.rotation != m_lookAtRotation) {
                if (isDebug)
                    Debug.Log ("DEBUG: TrackingSystem: (" + this.gameObject.name + ") Tracking.");
                lastTrackTime = Time.time;
                if (!rotateXAxis) {
                    lookAtRotEulers.x = 0;
                }
                if (!rotateYAxis) {
                    lookAtRotEulers.y = 0;
                }
                if (!rotateZAxis) {
                    lookAtRotEulers.z = 0;
                }
                m_lookAtRotation.eulerAngles = lookAtRotEulers;
                transform.rotation = Quaternion.RotateTowards (transform.rotation, m_lookAtRotation, speed * Time.deltaTime);

                this.hasTargetLock = false;//no target lock if we just needed to rotate.
            } else if (transform.rotation == m_lookAtRotation && m_target == null) {
                if (isDebug)
                    Debug.Log ("DEBUG: TrackingSystem: (" + this.gameObject.name + ") Target acquisition: rotation towards LookAt position complete.");
                //we just have a position to rotate to, not an actual target. signal target acquisition
                this.hasTargetLock = true;//target acquired, we have reached lookat rotation.
            } else if (transform.rotation == m_lookAtRotation) {
                //we are looking at our target, and our target is not null, meaning we should check for a raycast.
                //check if we can see the target as defined from our gameObject
                RaycastHit rhit;
                if (m_target != null && Physics.Raycast (this.transform.position, this.transform.forward, out rhit, m_range)) {
                    if (isDebug)
                        Debug.Log ("DEBUG: TrackingSystem: ("+this.gameObject.name+") Raycast hit: ("+rhit.collider.gameObject.name+").");
                    //raycast hit something
                    bool isExtraTarget = false;
                    foreach (string extra in extraTargets) {
                        if (rhit.collider.gameObject.name.Equals (extra)) {
                            isExtraTarget = true;//we've hit some part of our target
                            break;
                        }
                    }
                    if (isExtraTarget || rhit.collider.gameObject.name.Equals (m_target.name)) {
                        //we raycast hit player; only shoots player:
                        if (isDebug)
                            Debug.Log ("DEBUG: TrackingSystem: ("+this.gameObject.name+") Target acquisition: target not occluded.");
                        this.hasTargetLock = true; //target acquired
                    } else {
                        this.hasTargetLock = false;//no target lock if target is occluded
                    }
                } else if (m_target != null) {
                    this.hasTargetLock = false;//no target lock if we're not in range
                }

            } else {
                this.hasTargetLock = false;//still tracking, just not at the moment
            }
        } else if (!track) {
            this.hasTargetLock = false;//no target lock if we don't track
        }
	}

    public void shouldTrack(bool yesno) {
        track = yesno;
    }

    public void setTarget(GameObject target, float range){
		m_target = target;
        m_target_vector = target.transform.position;
        m_range = range;
	}

    public void setDampingPeriod(int damperAmount) {
        dampingPeriod = damperAmount;
    }

    //add additional targets to be considered as part of our rotational target.
    public void addExtraTarget(GameObject extraTarget) {
        extraTargets.Add (extraTarget.name);
    }
    //add additional targets to be considered as part of our rotational target.
    public void addExtraTargetByName(string extraTarget) {
        extraTargets.Add (extraTarget);
    }
    public void setTarget(Vector3 target){
        m_target_vector = target;
    }

    //If this object is currently pointed at the target, and has a clear line of sight to it's target or extra targets.
    //If just a position was specified as a target, then returns true when pointed in the correct direction.
    public bool targetAcquired() {
        return hasTargetLock;
    }
}
