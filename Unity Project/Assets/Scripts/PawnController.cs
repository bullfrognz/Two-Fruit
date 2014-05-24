using UnityEngine;
using System.Collections;

public class PawnController : MonoBehaviour
{
	
// Types
	
	
	public enum EState
	{
		IDLE,
		FLYING,
		FALLING,
		LANDING,
		GOUND_MOVING,
		JUMP_GETTING_READY,
		ABILITY_STAGE_1,
		ABILITY_STAGE_2,
		ABILITY_STAGE_3,
	}
	
	
	public enum EFacingDirection
	{
		LEFT,
		RIGHT
	}
	
	
// Functions
	
	
	// Public:
	
	
	public delegate void StateHandler(PawnController.EState _eNewState);
	public delegate void FacingDirectionHandler(PawnController.EFacingDirection _eNewFacingDirection);
	public event StateHandler 			StateChangeSubscribers;
	public event FacingDirectionHandler FacingDirectionChangeSubscribers;
	
	
	// Private:
	
	
	void Awake()
	{
		m_Pawn = GetComponent<Pawn>();
		m_Motor = GetComponent<CharacterMotor>();
		m_iCurrentLevel = (int)transform.position.z;
		
		
		// Setup input for controls
		if (m_Pawn.m_ePlayer == Pawn.EPlayer.BLOW)
		{
			m_sKeyLeft 		=  "a";
			m_sKeyRight 	=  "d";
			m_sKeyJump 		=  "w";
			m_sKeyAction	=  "e";
			m_sAxisX 		=  "Joystick 1 Axis X";
			m_sAxisY		=  "Joystick 1 Axis Y";
			m_sButtonJump 	=  "Joystick 1 Button A";
			m_sButtonAction =  "Joystick 1 Axis Z";
			
			m_cAbility       = gameObject.AddComponent<AbilityBlow>();
		}
		else if (m_Pawn.m_ePlayer == Pawn.EPlayer.SUCK)
		{
			m_sKeyLeft 		=  "left";
			m_sKeyRight 	=  "right";
			m_sKeyJump 		=  "up";
			m_sKeyAction	=  "/";
			m_sAxisX 		=  "Joystick 2 Axis X";
			m_sAxisY		=  "Joystick 2 Axis Y";
			m_sButtonJump 	=  "Joystick 2 Button A";
			m_sButtonAction =  "Joystick 2 Axis Z";
			
			m_cAbility       = gameObject.AddComponent<AbilitySuck>();
		}
	}


	void Start()
	{
		
	}
	

	void Update()
	{
		//ProcessKeyboardInput();
		ProcessJoystickInput();
		ProcessJumping();
		ProcessMovement();
	}
	
	
	void ProcessKeyboardInput()
	{
		m_bMoveLeft = false;
		m_bMoveRight = false;
		m_bJump = false;
		
		
		// Left
		if ( Input.GetKeyDown(m_sKeyLeft) &&
			!Input.GetKeyDown(m_sKeyRight))
		{
			m_bMoveLeft = true;
			m_fMoveRatio = -1.0f;
			SetFacingDirection(EFacingDirection.LEFT);
		}
		
		// Right
		else if (!Input.GetKeyDown(m_sKeyLeft) &&
				  Input.GetKeyDown(m_sKeyRight))
		{
			m_bMoveRight = true;
			m_fMoveRatio = -1.0f;
			SetFacingDirection(EFacingDirection.RIGHT);
		}
		
		
		// Jump
		if (Input.GetKeyDown(m_sKeyJump))
		{
			m_bJump = true;
			
			
			// Allow jumping queuing when landing
			if (m_eState == EState.LANDING)
			{
				m_bQueueJump = true;
			}
		}
		
		
		// Cannot action in keyboard mode
	}
	
	
	void ProcessJoystickInput()
	{
		m_bMoveIn = false;
		m_bMoveOut = false;
		m_bMoveRight = false;
		m_bMoveLeft = false;
		m_bJump = false;
		
		
		// Left
		if(Input.GetAxis(m_sAxisX) < -0.4)
		{
			m_bMoveLeft = true;
			SetFacingDirection(EFacingDirection.LEFT);
		}
		
		// Right
		else if(Input.GetAxis(m_sAxisX) > 0.4)
		{
			m_bMoveRight = true;
			SetFacingDirection(EFacingDirection.RIGHT);
		}	
		
		// Out
		if(Input.GetAxis(m_sAxisY) < -0.4)
		{
			m_bMoveIn = true;
		}
		
		// In
		else if(Input.GetAxis(m_sAxisY) > 0.4)
		{
			m_bMoveOut = true;
		}
		
		
		// Amount ratio
		m_fMoveRatio = Input.GetAxis(m_sAxisX);

		
		// Jump
		if(Input.GetButton(m_sButtonJump))
		{
			m_bJump = true;
			
			
			// Allow jumping queuing when landing
			if (m_eState == EState.LANDING)
			{
				m_bQueueJump = true;
			}
		}
		
		
		// Action
		if(Input.GetAxis(m_sButtonAction) < 0)
		{
			m_cAbility.Charging();
			
			// Pass controler axis to the ability
			m_cAbility.SetAxisDirection(Input.GetAxis(m_sAxisX), Input.GetAxis(m_sAxisY));
			
			// Stop player from moving.
			m_fMoveRatio = 0.0f;
		}
		else if(m_cAbility.IsCharging())
		{
			m_cAbility.Release();
		}
	}
	
	
	void ProcessJumping()
	{
		m_fJumpDelayTimer -= Time.deltaTime;
		
		
		// Wants to jump
		if (m_bJump ||
			m_bQueueJump)
		{
			// Has to be gounded
			if (m_Motor.IsGrounded())
			{
				// On a grounded state
				if (m_eState == EState.IDLE ||
					m_eState == EState.GOUND_MOVING)
				{
					// Not getting ready to jump
					if (!IsGoingToJump())
					{
						SetState(EState.JUMP_GETTING_READY);
						m_fJumpDelayTimer = m_kfJumpDelayDuration;
						m_bQueueJump = false;
					}
				}
			}
		}
		

		// Been allowed to jump
		if (IsGoingToJump())
		{
			// The delay has expired
			if (m_fJumpDelayTimer < 0.0f)
			{
				m_Motor.inputJump = true;
			}
		}
		else
		{
			m_Motor.inputJump = false;
		}
		
		
		// Going up
		if (!m_Motor.IsGrounded() &&
			 m_Motor.GetVelocity().y > 0.01f)
		{
			SetState(EState.FLYING);
		}
		
		// Going down
		else if (!m_Motor.IsGrounded() &&
				  m_Motor.GetVelocity().y < -0.01f)
		{
			SetState(EState.FALLING);
		}
		
		// Landing
		else if (m_Motor.IsGrounded() &&
				 m_eState == EState.FALLING)
		{
			SetState(EState.LANDING);
			m_fLandTimer = m_kfLandDuration;
		}
		
		// Finished landing
		else if (m_Motor.IsGrounded() &&
				 m_eState == EState.LANDING)
		{
			m_fLandTimer -= Time.deltaTime;
			
			
			if (m_fLandTimer < 0.0f)
			{
				SetState(EState.IDLE);
			}
		}
	}
	
	
	void ProcessMovement()
	{
		m_Motor.inputMoveDirection = Vector3.zero;
		
		
		// Ability not active
		if (!m_cAbility.IsCharging() &&
			!m_cAbility.IsReleasing())
		{
			// Cannot move during landing
			//if (m_eState != EState.LANDING)
			{
				// Moving left
				if (m_bMoveLeft)
				{
					m_Motor.inputMoveDirection = new Vector3(m_fMoveRatio, 0.0f, 0.0f);
				}
				
				// Moving right
				else if (m_bMoveRight)
				{
					m_Motor.inputMoveDirection = new Vector3(m_fMoveRatio, 0.0f, 0.0f);
				}
			}
			
			// Reseting the moveinouttimeout timer
			if(m_fMoveInOutTimer != 0.0f)
			{
				m_fMoveInOutTimer += Time.deltaTime;
				if(m_fMoveInOutTimer > m_fMoveInOutTimeout)
				{
					m_fMoveInOutTimer = 0.0f;
				}
			}
			else
			{
				// Moving in
				/*if(m_bMoveIn && m_iCurrentLevel != 2)
				{		
					if(!CheckObjectObstructingInOut(true))
					{
						Vector3 postion = transform.position;
						postion.z += 1;
						
						m_iCurrentLevel = (int)postion.z;
						
						transform.position = postion;
						
						m_fMoveInOutTimer += Time.deltaTime;
					}
				}
				
				// Moving out
				else if(m_bMoveOut && m_iCurrentLevel != 1)
				{
					if(!CheckObjectObstructingInOut(false))
					{
						Vector3 postion = transform.position;
						postion.z += -1;
						
						m_iCurrentLevel = (int)postion.z;
						
						transform.position = postion;
						
						m_fMoveInOutTimer += Time.deltaTime;
					}
				}*/
			}
		}
		
		// Make sure the pawn is locked on the z
		Vector3 Pos = transform.position;
		Pos.z = 2.0f;
		transform.position = Pos;
		
		// Has to be grounded
		if (m_Motor.IsGrounded())
		{
			// Not going to jump
			if (!IsGoingToJump())
			{
				// Not landing
				if (m_eState != EState.LANDING)
				{
					// Moving in either direction
					if (m_bMoveLeft ||
						m_bMoveRight)
					{
						SetState(EState.GOUND_MOVING);
					}
					else
					{
						SetState(EState.IDLE);
					}
				}
			}
		}
	}
	
	
	void SetState(EState _eNewState)
	{
		if (m_eState != _eNewState)
		{
			m_eState = _eNewState;
			
			
			if (StateChangeSubscribers != null)
			{
				StateChangeSubscribers(m_eState);
			}
			
			
			Debug.Log("State: " + _eNewState.ToString());
		}
	}
	
	
	void SetFacingDirection(EFacingDirection _eFacingDirection)
	{
		if (m_eFacingDirection != _eFacingDirection)
		{
			m_eFacingDirection = _eFacingDirection;
			
			
			if (FacingDirectionChangeSubscribers != null)
			{
				FacingDirectionChangeSubscribers(m_eFacingDirection);
			}
			
			
			Debug.Log("Facing Direction: " + _eFacingDirection.ToString());
		}
	}
	
	
	bool IsGoingToJump()
	{
		return (m_eState == EState.JUMP_GETTING_READY);
	}
	
	bool CheckObjectObstructingInOut(bool _In)
	{
		Vector3 v3Direction = Vector3.zero;
		Vector3 vPostion = transform.position;// + new Vector3(100, 100, 100);
		
		if(_In)
		{
			v3Direction = Vector3.forward;
		}
		else
		{
			v3Direction = Vector3.back;
		}
		
		CharacterController charContr = GetComponent<CharacterController>();
		
		Vector3 vPostion1 = charContr.bounds.min + v3Direction;
		Vector3 vPostion2 = charContr.bounds.max + v3Direction;
		
		bool bReturn = Physics.CheckCapsule(vPostion1, vPostion2, charContr.radius);
		
		return(bReturn);
	}

	
// Variables
	
	
	// Public:
	
	
	// Projected:
	
	
	// Private:
	
	
	Pawn 			m_Pawn;
	CharacterMotor 	m_Motor;
	PawnAbility     m_cAbility;
	
	
	string m_sKeyLeft;
	string m_sKeyRight;
	string m_sKeyJump;
	string m_sKeyAction;
	string m_sAxisX;
	string m_sAxisY;
	string m_sButtonJump;
	string m_sButtonAction;
	
	
	EState m_eState;
	EFacingDirection m_eFacingDirection;
	
	float m_fMoveInOutTimer;
	float m_fMoveInOutTimeout = 0.4f;
	public int m_iCurrentLevel;
	
	
	const float m_kfJumpDelayDuration = -0.1f;
	const float m_kfLandDuration = -0.1f;
	float m_fMoveRatio;
	float m_ActionCharge;
	float m_fJumpDelayTimer;
	float m_fLandTimer;
	
	bool m_bMoveIn;
	bool m_bMoveOut;	
	bool m_bMoveLeft;
	bool m_bMoveRight;
	bool m_bJump;
	bool m_bQueueJump;
	
	
}
