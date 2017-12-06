using UnityEngine;
using System.Collections;

public class NormalProjectile : BaseProjectile {
    public static bool LogCollisions = true;

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
        if (LogCollisions) {
            Debug.Log ("NORMALPROJECTILE COLLISION: " + other.gameObject.ToString ());
        }
        ProjectileImpactPoint.ImpactType impactType = ProjectileImpactPoint.ImpactType.METAL;
        if (m_launcher != null && other.gameObject.Equals (m_launcher) || other.transform.IsChildOf (m_launcher.transform) || m_launcher.transform.IsChildOf (other.transform)) {
            return;//ignore any collisions that may come from an object that is a parent of, child of, or literally is the launcher.
        }
        if (other.collider.transform.gameObject.name.Equals ("Player")) {
            //we hit the player
            impactType = ProjectileImpactPoint.ImpactType.FLESH;
        }

        EntityPart otherEntPart = other.gameObject.GetComponent<EntityPart> ();
        if (otherEntPart == null && other.transform.parent != null) {
            //this object may not have had the Entity Part, but it's parent might. check that too!
            otherEntPart = other.transform.parent.GetComponent<EntityPart> ();
        }
        if (otherEntPart != null) {
            //here is where we do damage to the EntityPart we hit, which will handle passing this damage to it's parent entity.
            otherEntPart.damage (m_launcher, m_damage);
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
