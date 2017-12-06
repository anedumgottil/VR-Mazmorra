using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class allows us to define sections of an Entity that treat incoming collisions differently. 
//Note: this needs to be part of the GameObject that has the associated EntityPart Collider on it, or a parent of said GameObject, or the NormalProjectile will not be able to find this script
public class EntityPart : MonoBehaviour {
    public int startingHealth = 50;//how much damage we can absorb before we trigger our death action (breakoff or just disappear, and pass damage)
    public Entity parentEntity = null;//the gameobject of the parent entity. if not set, it will default to the parent gameobject.
    public bool passDamageOnDeath = true;//if true, it absorbs damage only to release it all times it's modifier on death action. if false, passes whatever damage its given once-per-collision * modifier of course.
    public float damageMultiplyer = 1.0f;//how much it should scale the damage it passes through. set to zero to completely prevent the part from passing damage to it's parent.
    public bool killParentOnDeath = false;//if true, when this dies it kills the parent, after it's finished passing damage according to the above rules.
    public bool destroyOnDeath = true;//whether or not to destroy the component after it dies.
    public float breakawayDestroyWaitTime = 5.0f;//how long the broken off part should float around for
    public bool isBreakaway = false;//should the piece break off and float around before dying? requires rigidbody.
    public Collider partCollider = null;//specifying this forces the part to use this collider rather than whatever it can find
    private Rigidbody rigid;
    private bool isAlive = false;
    private int health;


    public void Start() {
        health = startingHealth;
        if (partCollider == null) {
            partCollider = this.gameObject.GetComponent<Collider> ();
            if (partCollider == null)
                partCollider = this.gameObject.GetComponentInChildren<Collider> ();
        }
        if (parentEntity == null) {
            parentEntity = this.transform.parent.gameObject.GetComponent<Entity> ();
            if (parentEntity == null) {
                Debug.LogError ("EntityPart: was not given an entity, and couldn't find one in parent.");
                this.isAlive = false;
                this.isBreakaway = false;
            }
        }
        rigid = this.GetComponent<Rigidbody>();
        if (rigid == null) {//no rigidbody? no breakaway.
            isBreakaway = false;
        }
        isAlive = true;
    }

    public void damage(GameObject damager, int damage) {
        if (isAlive) {
            if (!passDamageOnDeath) {
                //if we're not saving up our damage, let's pass it on.
                parentEntity.damage (damager, (int)(damage * damageMultiplyer));
            }
            //in any case, hurt ourselves, so we can eventually die.
            this.health -= damage;
            if (health <= 0) {
                this.die ();
            }
        }
    }

    public void die() {
        this.isAlive = false;
        if (passDamageOnDeath) {
            //send damage to parent entity. We do this before we breakaway, because the parent needs to be able to tell what our original parent was.
            //damage is our full health value, plus whatever overflow'd:

            //Note: tells the parentEntity that IT is damaging the parent. Due to the fact we don't know what made up the cumulative damage to this part, and it's a special case.
            parentEntity.damage (this.gameObject, (int)((this.startingHealth - this.health)*damageMultiplyer));
        }
        if (isBreakaway) {
            this.breakaway ();//perform breakaway
            if (destroyOnDeath) {
                Destroy (this.gameObject, breakawayDestroyWaitTime);//die after 5 seconds
            }
            return;
        }
        if (killParentOnDeath) {
            parentEntity.die ();
        }
        if (destroyOnDeath) {
            Destroy (this.gameObject);
            return;
        }
    }

    public void setDamageMultiplyer(float damageMultiplyer) {
        this.damageMultiplyer = damageMultiplyer;
    }

    public void breakaway() {
        Vector3 oldParentPosition = this.transform.parent.position;
        //deparent
        this.transform.SetParent (null);
        //breakoff
        //if (partCollider != null) partCollider.enabled = false;
        rigid.isKinematic = false;
        rigid.useGravity = true;
        rigid.AddForceAtPosition (Vector3.Normalize(this.transform.position - oldParentPosition), oldParentPosition, ForceMode.Impulse);
    }
	
}
