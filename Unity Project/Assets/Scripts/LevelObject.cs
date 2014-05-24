using UnityEngine;
using System.Collections;

public class LevelObject : MonoBehaviour 
{

	void Start() 
	{
	}
	
	void Update()
	{
		Rigidbody rigidBody = GetComponent<Rigidbody>();
		
		if(!m_UsesGravity)
		{
			if(rigidBody.velocity.magnitude > 0.1f)
			{
				rigidBody.velocity -= rigidBody.velocity.normalized * Time.deltaTime * 10.0f;
			}
			else if(rigidBody.velocity.magnitude < 0.1f)
			{
				rigidBody.velocity = Vector3.zero;
			}
			
			if(rigidBody.angularVelocity.magnitude > 0.1f)
			{
				rigidBody.angularVelocity -= rigidBody.angularVelocity.normalized * Time.deltaTime * 3.141f;
			}
			else if(rigidBody.velocity.magnitude < 0.1f)
			{
				rigidBody.angularVelocity = Vector3.zero;
			}
		}
	}
	
	public bool m_UsesGravity = false;
}
