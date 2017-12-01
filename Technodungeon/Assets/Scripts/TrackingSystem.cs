using UnityEngine;
using System.Collections;

public class TrackingSystem : MonoBehaviour {
	public float speed = 3.0f;
    public bool rotateXaxis = true;
    public bool rotateYaxis = true;
    public bool rotateZaxis = true;

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

            if (transform.rotation != m_lookAtRotation) {
                if (!rotateXaxis) {
                    m_lookAtRotation.x = transform.rotation.x;
                }
                if (!rotateYaxis) {
                    m_lookAtRotation.y = transform.rotation.y;
                }
                if (!rotateZaxis) {
                    m_lookAtRotation.z = transform.rotation.z;
                }
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
