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
			transform.position += m_direction * (this.speed * Time.deltaTime);
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
        ProjectileImpactPoint.ImpactType impactType = ProjectileImpactPoint.ImpactType.METAL;
        if (other.gameObject.Equals (m_launcher)) {
            return;//it'll probably collide with the gun on the way out, ignore it.
        }
        if (other.collider.transform.gameObject.name.Equals ("Player")) {
            //we hit the player
            impactType = ProjectileImpactPoint.ImpactType.FLESH;
        }
        Entity otherEnt = other.gameObject.GetComponent<Entity> ();
        if(otherEnt != null)
		{
            //here is where we would do damage to the Entity we hit.
            otherEnt.damage (m_launcher, m_damage);
		}
        //Play a bullet reflection sound and impact particles
        if (impactObject != null) {
            GameObject impactObjClone = Instantiate (impactObject, other.contacts [0].point, Quaternion.FromToRotation (Vector3.up, other.contacts[0].normal));
            ProjectileImpactPoint projImpactPt = impactObjClone.GetComponent<ProjectileImpactPoint> ();
            if (projImpactPt != null) {
                projImpactPt.setImpactType (impactType);
                projImpactPt.triggerImpactPoint ();
            }
        }
		if(other.gameObject.GetComponent<BaseProjectile>() == null)
			Destroy(gameObject);
	}
}
