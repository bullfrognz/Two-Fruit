using UnityEngine;
using System.Collections;


public class CameraController : MonoBehaviour
{
	
// Member Types

	
	
// Member Functions
	
	
	// Public:
	
	
	// Private:
	
	
	void Start()
	{
		m_oBlow = GameObject.Find("Blow");
		m_oSuck = GameObject.Find("Suck");
	}
	

	void Update()
	{
		ProcessTargetPosition();
		ProcessTargetOrthoSize();
		ProcessPosition();
		ProcessOrthoSize();
	}
	
	
	void ProcessTargetPosition()
	{
		Vector3 vPlayersDisplacement = m_oBlow.transform.position - m_oSuck.transform.position;
		Vector3 vMedian = m_oSuck.transform.position + (vPlayersDisplacement / 2.0f);
		
		
		m_vTargetPosition = vMedian;
		m_vTargetPosition.z = transform.position.z;
	}
	
	
	void ProcessTargetOrthoSize()
	{
		float fCameraHeight = Camera.main.orthographicSize * 2;
  		float fCameraWidth  = Camera.main.aspect * fCameraHeight;
		float fPlayersDistance = (m_oBlow.transform.position - m_oSuck.transform.position).magnitude;
		Vector3 vDisplacement = m_oBlow.transform.position - m_oSuck.transform.position;
		vDisplacement.x = Mathf.Abs(vDisplacement.x);
		vDisplacement.y = Mathf.Abs(vDisplacement.y);
		
		
		float fWidth  = (vDisplacement.x / Camera.main.aspect) / 2.0f;
		float fHeight = (vDisplacement.y) / 2.0f;
		
		
		//Debug.Log(vDisplacement);
	
		if (fWidth > fHeight)
		{
			m_fTargetOrthoSize = fWidth;
		}
		else
		{
			m_fTargetOrthoSize = fHeight;
		}
		
		
		
		m_fTargetOrthoSize += m_kfZoomBuffer;
	}
	
	
	void ProcessPosition()
	{
		transform.position = Vector3.MoveTowards(transform.position, m_vTargetPosition, m_kfMoveVelocity * Time.deltaTime);
	}
	
	
	void ProcessOrthoSize()
	{
		if (Camera.main.orthographicSize != m_fTargetOrthoSize)
		{
			float fDifference = m_fTargetOrthoSize - Camera.main.orthographicSize;
			
			
			if (fDifference <  0.05f &&
				fDifference > -0.05f)
			{
				Camera.main.orthographicSize = m_fTargetOrthoSize;
			}
			else if (fDifference > 0.0f)
			{
				Camera.main.orthographicSize += m_kfMoveVelocity * Time.deltaTime;
			}
			else
			{
				Camera.main.orthographicSize -= m_kfMoveVelocity * Time.deltaTime;
			}
		}
	}
	
	
	// Events:
	
	
// Member Variables
	
	
	// Public:
	
	
	// Private:
	
	
	GameObject m_oBlow;
	GameObject m_oSuck;
	
	
	Vector3 m_vTargetPosition;
	
	
	const float m_kfMoveVelocity =  5.0f;
	const float m_kfZoomSpeed 	 =  1.0f;
	const float m_kfZoomBuffer   =  6.0f;
	
	
	float m_fTargetOrthoSize = 15;
	
	
}
