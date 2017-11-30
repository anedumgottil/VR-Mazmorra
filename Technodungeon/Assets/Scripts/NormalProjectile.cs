using UnityEngine;
using System.Collections;

public class NormalProjectile : BaseProjectile {
    public GameObject impactObject;//the object to spawn on impact

	Vector3 m_direction;
	bool m_fired;
	GameObject m_launcher;
	GameObject m_target;
	int m_damage;

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
        //Play a bullet reflection sound and impact particles
        if (impactObject != null) {
            Instantiate (impactObject, other.contacts [0].point, -this.m_direction);
        }
		if(other.gameObject.GetComponent<BaseProjectile>() == null)
			Destroy(gameObject);
	}
}
