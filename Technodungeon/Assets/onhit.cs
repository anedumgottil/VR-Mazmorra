using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://stackoverflow.com/questions/30280187/emit-particle-oncollision-in-unity-3d

public class onhit : MonoBehaviour {
	public ParticleSystem collisionParticlePrefab; //Assign the Particle from the Editor (You can do this from code too)
	private ParticleSystem tempCollisionParticle;

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "turret")
		{
			tempCollisionParticle = Instantiate (collisionParticlePrefab, transform.position, Quaternion.identity) as ParticleSystem;
			tempCollisionParticle.Play ();
		}

	}
}
