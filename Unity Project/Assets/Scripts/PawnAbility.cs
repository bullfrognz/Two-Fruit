using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PawnAbility : MonoBehaviour
{
	
// Member Types
	
	public enum EState
	{
		INVALID = -1,
		
		NONE,
		CHARGING_LVL1,
		CHARGING_LVL2,
		CHARGING_LVL3,
		RELEASING,
		
		MAX
	}
	
	public class AbilityReleaseInfo
	{
		public GameObject go;
		public RaycastHit hit;
	}
	
// Member Functions
	
	
	// Events:
	
	
	public delegate void StateHandler(PawnAbility.EState _eNewState);
	public event StateHandler StateChangeSubscribers;
	
	// Public:
	public void Charging()
	{
		m_bCharging = true;
		
		if(m_bReleasing)
		{
			m_fAccumulatedForce = 0.0f;
			m_bReleasing = false;
		}
	}
	
	public void Release() 
	{
		ReleaseParticles();
		m_bReleasing = true;
		m_bCharging = false;
	}
	
	public void SetAxisDirection(float _X, float _Y)
	{
		if(m_bCharging)
		{
			m_vTargetingDirection = new Vector3(_X, -_Y, 0.0f);
			m_vTargetingDirection.Normalize();
		}
	}	
	
	public virtual void ReleaseAbility() {}
	
	// Private:
	void Update()
	{
		if(m_bCharging)
		{	
			AccumulateForce();
			RotateEmitter();
		}
		else if(m_bReleasing)
		{
			ReleaseAbility();
		}
		
		if(!m_bReleasing) 
		{
			StopParticles();
		}
		
		DrawAbility();
		UpdateState();
	}
	
	void RotateEmitter()
	{
		transform.FindChild("Particles").transform.rotation = Quaternion.LookRotation(m_vTargetingDirection);
	}
	
	protected void ReleaseParticles()
	{
		ParticleSystem ps = transform.FindChild("Particles").GetComponent<ParticleSystem>();
		ps.Play();
	}
	
	protected void StopParticles()
	{
		ParticleSystem ps = transform.FindChild("Particles").GetComponent<ParticleSystem>();
		ps.Stop();
	}
	
	void DrawAbility()
	{
		Vector3 Start = GetComponent<Transform>().position;
		Vector3 vDirectionUp = m_vTargetingDirection;
		Vector3 vDirectionDown = m_vTargetingDirection;
		Quaternion incrementRotAmountUp = Quaternion.identity;
		incrementRotAmountUp = Quaternion.AngleAxis((m_fConeRadius/2)/((float)m_iNumberOfRays/2), Vector3.back);
		Quaternion incrementRotAmountDown = Quaternion.Inverse(incrementRotAmountUp);
		Color color = Color.white;
		if(m_bReleasing)
		{
			color.a = 0.0f;
		}
		
		// Center
		Vector3 End = m_vTargetingDirection * m_fMaxDistance + Start;
		
		LineRenderer lr = gameObject.GetComponent<LineRenderer>();
		lr.SetPosition(0, Start);
		lr.SetPosition(1, End);
		
		lr.SetWidth(0.0f, (m_fAccumulatedForce / m_fMaxAccumalationForce) * 10.0f);
		
		lr.SetColors(color, color);
	}
	
	protected virtual void AccumulateForce() {}
		
	protected void GetTargets(ref List<AbilityReleaseInfo> _rObjects)
	{		
		List<GameObject> ObjectsInList = new List<GameObject>();
		
		// Center
		RaycastHit[] hitInfo = Physics.RaycastAll(transform.position, m_vTargetingDirection, m_fMaxDistance);
		
		int j = 0;	
		while(j < hitInfo.Length)
		{
			if(!ObjectsInList.Exists(delegate(GameObject x) {return x == hitInfo[j].collider.gameObject;}))
			{
				AbilityReleaseInfo newTarget = new AbilityReleaseInfo();
				newTarget.hit = hitInfo[j];
				
				if(hitInfo[j].collider.gameObject.GetComponent<Rigidbody>() || hitInfo[j].collider.gameObject.GetComponent<Pawn>())
				{
					newTarget.go = hitInfo[j].collider.gameObject;
					_rObjects.Add(newTarget);
				
					ObjectsInList.Add(newTarget.go);
				}
				else if(hitInfo[j].collider.gameObject.transform.parent.gameObject.GetComponent<Rigidbody>() &&
					    !ObjectsInList.Exists(delegate(GameObject x) {return x == hitInfo[j].collider.transform.parent.gameObject;}))
				{
					newTarget.go = hitInfo[j].collider.gameObject.transform.parent.gameObject;
					_rObjects.Add(newTarget);
				
					ObjectsInList.Add(newTarget.go);
				}
			}
			++j;
		}
		
		Vector3 vDirectionUp = m_vTargetingDirection;
		Vector3 vDirectionDown = m_vTargetingDirection;
		
		Quaternion incrementRotAmountUp = Quaternion.identity;
		incrementRotAmountUp = Quaternion.AngleAxis((m_fConeRadius/2)/((float)m_iNumberOfRays/2), Vector3.back);
		Quaternion incrementRotAmountDown = Quaternion.Inverse(incrementRotAmountUp);
		
		for(int i = 0; i < m_iNumberOfRays/2; ++i)
		{
			// Upwards	
			vDirectionUp = incrementRotAmountUp * vDirectionUp;
			
			hitInfo = Physics.RaycastAll(transform.position, vDirectionUp, m_fMaxDistance);
			
			j = 0;	
			while(j < hitInfo.Length)
			{
				if(!ObjectsInList.Exists(delegate(GameObject x) {return x == hitInfo[j].collider.gameObject;}))
				{
					AbilityReleaseInfo newTarget = new AbilityReleaseInfo();
					newTarget.hit = hitInfo[j];
					
					if(hitInfo[j].collider.gameObject.GetComponent<Rigidbody>() || hitInfo[j].collider.gameObject.GetComponent<Pawn>())
					{
						newTarget.go = hitInfo[j].collider.gameObject;
						_rObjects.Add(newTarget);
					
						ObjectsInList.Add(newTarget.go);
					}
					else if(hitInfo[j].collider.gameObject.transform.parent.gameObject.GetComponent<Rigidbody>() &&
						    !ObjectsInList.Exists(delegate(GameObject x) {return x == hitInfo[j].collider.transform.parent.gameObject;}))
					{
						newTarget.go = hitInfo[j].collider.gameObject.transform.parent.gameObject;
						_rObjects.Add(newTarget);
					
						ObjectsInList.Add(newTarget.go);
					}
				}
				++j;
			}
			
			// Downwards
			vDirectionDown = incrementRotAmountDown * vDirectionDown;
			
			hitInfo = Physics.RaycastAll(transform.position, vDirectionDown, m_fMaxDistance);
			
			j = 0;	
			while(j < hitInfo.Length)
			{
				if(!ObjectsInList.Exists(delegate(GameObject x) {return x == hitInfo[j].collider.gameObject;}))
				{
					AbilityReleaseInfo newTarget = new AbilityReleaseInfo();
					newTarget.hit = hitInfo[j];
					
					if(hitInfo[j].collider.gameObject.GetComponent<Rigidbody>() || hitInfo[j].collider.gameObject.GetComponent<Pawn>())
					{
						newTarget.go = hitInfo[j].collider.gameObject;
						_rObjects.Add(newTarget);
					
						ObjectsInList.Add(newTarget.go);
					}
					else if(hitInfo[j].collider.gameObject.transform.parent.gameObject.GetComponent<Rigidbody>() &&
						    !ObjectsInList.Exists(delegate(GameObject x) {return x == hitInfo[j].collider.transform.parent.gameObject;}))
					{
						newTarget.go = hitInfo[j].collider.gameObject.transform.parent.gameObject;
						_rObjects.Add(newTarget);
					
						ObjectsInList.Add(newTarget.go);
					}
				}
				++j;
			}
		}
	}
	
	
	void UpdateState()
	{
		if (m_bCharging)
		{
			float fAccumalatedRatio = m_fAccumulatedForce / m_fMaxAccumalationForce;
			
			
			if (fAccumalatedRatio < 0.33f)
			{
				ChangeState(EState.CHARGING_LVL1);
			}
			else if (fAccumalatedRatio < 0.66f)
			{
				ChangeState(EState.CHARGING_LVL2);
			}
			else
			{
				ChangeState(EState.CHARGING_LVL3);
			}
		}
		else if (m_bReleasing)
		{
			ChangeState(EState.RELEASING);
		}
		else
		{
			ChangeState(EState.NONE);
		}
	}
	
	
	void ChangeState(EState _eNewState)
	{
		if (m_eState != _eNewState)
		{
			m_eState = _eNewState;
			
			
			if (StateChangeSubscribers != null)
			{
				StateChangeSubscribers(m_eState);
			}
			
			
			Debug.Log("[Ability] State: " + _eNewState.ToString());
		}
	}
	
	
	public bool IsCharging()
	{
		return (m_bCharging);
	}
	
	
	public bool IsReleasing()
	{
		return (m_bReleasing);
	}
	
	
	
// Member Variables
	
	
	// Public:
	
	
	// Private:
	
	
	EState m_eState;
	
	
	public float m_fMaxAccumalationForce;
	public float m_fAccumalationRate;
	public float m_fMaxDistance;
	public float m_fAccumulatedForce;
	
	public float m_fConeRadius;
	public int m_iNumberOfRays;
	public Vector3 m_vTargetingDirection;
	
	public bool m_bCharging;
	public bool m_bReleasing;
	
	
}
