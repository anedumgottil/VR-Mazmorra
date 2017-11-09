
using UnityEngine;
using System.Collections;

public class TrackingProjectile : BaseProjectile {
	GameObject m_target;
	GameObject m_launcher;
	int m_damage;

	Vector3 m_lastKnownPosition;

	// Update is called once per frame
	void Update () {
		m_target = Player.getInstance ().getHead ();
		if(m_target){
			m_lastKnownPosition = m_target.transform.position;
		}
		else
		{
			if(transform.position == m_lastKnownPosition)
			{
				Destroy(gameObject);
			}
		}

		transform.position = Vector3.MoveTowards(transform.position, m_lastKnownPosition, speed * Time.deltaTime);
	}

	public override void FireProjectile(GameObject launcher, GameObject target, int damage, float attackSpeed){
		if(target){
			m_target = target;
			m_lastKnownPosition = target.transform.position;
			m_launcher = launcher;
			m_damage = damage;
		}
	}

//	void OnCollisionEnter(Collision other)
//	{
//		if(other.gameObject == m_target)
//		{
//			DamageData dmgData = new DamageData();
//			dmgData.damage = m_damage;
//
//			MessageHandler msgHandler = m_target.GetComponent<MessageHandler>();
//
//			if(msgHandler)
//			{
//				msgHandler.GiveMessage(MessageType.DAMAGED, m_launcher, dmgData);
//			}
//		}
//
//		if(other.gameObject.GetComponent<BaseProjectile>() == null)
//			Destroy(gameObject);
//	}
}
