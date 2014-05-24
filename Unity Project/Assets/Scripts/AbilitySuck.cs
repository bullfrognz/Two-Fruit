using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AbilitySuck : PawnAbility
{
	
// Member Types

	
// Member Functions	

	// Public
	
	public override void ReleaseAbility()
	{
		if(m_fAccumulatedForce > 0.0f)
		{	
			List<AbilityReleaseInfo> abilityHitInfo = new List<AbilityReleaseInfo>();
			
			GetTargets(ref abilityHitInfo);
			
			foreach(AbilityReleaseInfo aRI in abilityHitInfo)
			{			
				Vector3 v3ForceDirection = Vector3.zero;
				float fForce = 0.0f; 
				
				GetActionForce(aRI.go, ref fForce, ref v3ForceDirection);
				v3ForceDirection.Normalize();
				
				if(aRI.go.GetComponent<Pawn>())
				{	
					Vector3 Velocity = aRI.go.GetComponent<CharacterMotor>().GetVelocity();
					Velocity += (v3ForceDirection * fForce);
					
					aRI.go.GetComponent<CharacterMotor>().SetVelocity(Velocity);
				}
				else
				{
					Rigidbody TargetBody = aRI.go.GetComponent<Rigidbody>();
					
					if(!TargetBody)
					{
						Debug.Log(string.Format("Couldn't find body for: {0}", aRI.go.ToString()));
						continue;
					}
					
					// Add the force to the locked on target
					TargetBody.AddForceAtPosition((v3ForceDirection * fForce), aRI.hit.point, ForceMode.Impulse);
				}
			}
			
			m_fAccumulatedForce -= m_fAccumalationRate * Time.deltaTime;
		}
		else
		{
			m_fAccumulatedForce = 0.0f;
			m_bReleasing = false;
		}
	}
	
	// Protected
	
	protected override void AccumulateForce()
	{
		m_fAccumulatedForce += m_fAccumalationRate * Time.deltaTime;
		
		if(m_fAccumulatedForce > m_fMaxAccumalationForce)
		{
			m_fAccumulatedForce = m_fMaxAccumalationForce;
		}
	}
	
	protected void GetActionForce(GameObject _Go, ref float _fForce, ref Vector3 _vForceDirection)
	{
		float fDistance = Vector3.Distance(transform.position, _Go.transform.position);
		
		float fScaleCap = 0.8f;
		if(fDistance < m_fMaxDistance * fScaleCap)
		{
			fDistance = m_fMaxDistance * fScaleCap;
		}
		
		_fForce = m_fAccumulatedForce * (fDistance / m_fMaxDistance);
		
		if(_Go.GetComponent<Pawn>())
		{	
			_vForceDirection = transform.position - _Go.transform.position;
			
			_fForce *= 1.0f / 5.0f;
		}
		else
		{
			_vForceDirection = -m_vTargetingDirection;
		}
		
		_vForceDirection.Normalize();
	}
	
	// Private
	
	void Start()
	{
		m_bCharging = false;
		m_bReleasing = false;
		m_iNumberOfRays = 10;
		m_fConeRadius = 30;
		m_fMaxAccumalationForce = 5;
		m_fAccumalationRate = 10;
		m_fMaxDistance = 10;
		m_fAccumulatedForce = 0;
	}
	
	
// Member Variables
	
	
	// Public:
	
	
	// Private:
	
	
}
