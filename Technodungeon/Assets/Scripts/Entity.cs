using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//barebones parent class for Entities in the game, basically everything that exists on top of the generated map gridspaces.
//You should not inherit this class, in most cases, you should inherit from the child classes Stationary and Mobile Entity.

public abstract class Entity : MonoBehaviour {
    public int startingHealth = 100;
    protected int health = 100;
    protected bool alive = true;

    void Start() {
        health = startingHealth;
    }

    public virtual void damage(GameObject damageCause, int damageAmount) {
        health -= damageAmount;
        if (health <= 0) {
            this.die ();
        }
    }

    public int getHealth() {
        return health;
    }

    public void setHealth(int healthvalue) {
        health = healthvalue;
    }

    public bool isAlive() {
        return alive;
    }

    public virtual void die() {
        alive = false;
        Destroy (this);
    }
}