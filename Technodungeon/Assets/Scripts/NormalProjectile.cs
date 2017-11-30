using UnityEngine;
using System.Collections;

public class NormalProjectile : BaseProjectile {
    public AudioClip[] impactNoises;

	Vector3 m_direction;
	bool m_fired;
	GameObject m_launcher;
	GameObject m_target;
	int m_damage;

    AudioSource audioSource = null;

    void Start () {
        audioSource = this.gameObject.GetComponent<AudioSource> ();
    }

	// Update is called once per frame
	void Update () {
		if(m_fired){
			transform.position += m_direction * (speed * Time.deltaTime);
		}
	}

    public override void FireProjectile(GameObject launcher, Vector3 normalizedDirection, int damage, float attackSpeed){
		if(launcher){
            m_direction = normalizedDirection;
			m_fired = true;
			m_launcher = launcher;
			m_damage = damage;

			Destroy(gameObject, 10.0f);
		}
	}

	void OnCollisionEnter(Collision other)
	{
        if (other.gameObject.Equals (m_launcher)) {
            return;//it'll probably collide with the gun on the way out, ignore it.
        }
        Entity otherEnt = other.gameObject.GetComponent<Entity> ();
        if(otherEnt != null)
		{
            //here is where we would do damage to the Entity we hit.
            //otherEnt.damage (m_launcher, m_damage);
		}
        //Play a bullet reflection sound.
        if (!audioSource.isActiveAndEnabled) {
            audioSource.enabled = true;
        }
        audioSource.pitch += ((float) (Random.Range (0,11)*0.01f)) + (-0.05f);//tweak pitch from range (0.05 to -0.05)
        if (impactNoises != null && impactNoises.Length > 0) {
            int selectedsound = Random.Range (0, impactNoises.Length);
            Debug.Log("Playing "+ selectedsound); 
            audioSource.PlayOneShot (impactNoises[selectedsound]);
        }

		if(other.gameObject.GetComponent<BaseProjectile>() == null)
			Destroy(gameObject, 1.0f);
	}
}
