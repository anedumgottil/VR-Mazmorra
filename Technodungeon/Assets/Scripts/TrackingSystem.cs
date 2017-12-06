using UnityEngine;
using System.Collections;

public class TrackingSystem : MonoBehaviour {
	public float speed = 3.0f;
    public bool rotateXAxis = true;
    public bool rotateYAxis = true;
    public bool rotateZAxis = true;

	GameObject m_target = null;
    Vector3 m_target_vector;
	Vector3 m_lastKnownPosition = Vector3.zero;
	Quaternion m_lookAtRotation;
    private bool track = false;

	// Update is called once per frame
	void Update () {
        if (track) {
            if (m_lastKnownPosition != m_target_vector) {
                m_lastKnownPosition = m_target_vector;
                m_lookAtRotation = Quaternion.LookRotation (m_lastKnownPosition - transform.position);
            }
            Vector3 lookAtRotEulers = m_lookAtRotation.eulerAngles;
            if (transform.rotation != m_lookAtRotation) {
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
            }
        }
	}

    public void shouldTrack(bool yesno) {
        track = yesno;
    }

	public void setTarget(GameObject target){
		m_target = target;
        m_target_vector = target.transform.position;
	}
    public void setTarget(Vector3 target){
        m_target_vector = target;
    }
}
