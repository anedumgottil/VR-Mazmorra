using UnityEngine;
using System.Collections;

public abstract class BaseProjectile : MonoBehaviour {
    protected float speed = 5.0f;

    public void setSpeed(float speed) {
        this.speed = speed;
    }

    public float getSpeed() {
        return speed;
    }

    public abstract void FireProjectile(GameObject launcher, Vector3 normalizedDirection, int damage, float attackSpeed);
}
