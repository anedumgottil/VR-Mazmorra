using UnityEngine;
using System.Collections;

public abstract class BaseProjectile : MonoBehaviour {
	public float speed = 5.0f;

    public abstract void FireProjectile(GameObject launcher, Vector3 normalizedDirection, int damage, float attackSpeed);
}
