using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PawnAnimator : MonoBehaviour
{
	
// Types
	
	
	public enum EAnimation
	{
		INVALID = -1,
		
		IDLE_LEFT,
		IDLE_RIGHT,
		
		IDLE2_LEFT,
		IDLE2_RIGHT,
		
		MOVE_LEFT,
		MOVE_RIGHT,
		
		JUMP_INITIATE_LEFT,
		JUMP_INITIATE_RIGHT,
		
		FLY_LEFT,
		FLY_RIGHT,
		
		FLY_PEAK_LEFT,
		FLY_PEAK_RIGHT,
		
		FALL_LEFT,
		FALL_RIGHT,
		
		FALL_LAND_LEFT,
		FALL_LAND_RIGHT,
		
		ACTION_CHARGING_LVL_1_LEFT,
		ACTION_CHARGING_LVL_1_RIGHT,
		
		ACTION_CHARGING_LVL_2_LEFT,
		ACTION_CHARGING_LVL_2_RIGHT,
		
		ACTION_CHARGING_LVL_3_LEFT,
		ACTION_CHARGING_LVL_3_RIGHT,
		
		ACTION_RELEASING_LEFT,
		ACTION_RELEASING_RIGHT,
		
		MAX
	}
	
	
	public struct TSpriteSheet
	{
		public Texture2D cTexture2D;
		public string sFilename;
		public float fSpeed;
		public uint uiDevisionCount;
		public uint uiSpriteCount;
	}
	
	
// Functions
	
	
	// Public:
	
	
	public void ChangeAnimation(EAnimation _eAnimation)
	{
		ChangeAnimation(_eAnimation, 0);
	}
	
	
	public void ChangeAnimation(EAnimation _eAnimation, uint _uiStartFrame)
	{
		if (m_eActiveAnimation != _eAnimation)
		{
			m_eActiveAnimation = _eAnimation;
			
			
			renderer.material.mainTexture = GetActiveSheet().cTexture2D;
			renderer.material.shader = Shader.Find("Transparent/Diffuse");
			renderer.material.color = m_Color;
			
			
			m_fSpriteTimer = GetActiveSheet().fSpeed;
			m_uiCurrentSprite = 0;
			
			
			UpdateTile();
			
			
			//Debug.Log(string.Format("Changed Animation: {0} Value: {1}", _eAnimation.ToString(), (int)_eAnimation));
		}
	}
	
	
	public void ClearQueue()
	{
		m_eQueuedAnaimation = EAnimation.INVALID;
	}
	
	
	public void QueueAnimation(EAnimation _eAnimation)
	{
		m_eQueuedAnaimation = _eAnimation;
	}
	
	
	public TSpriteSheet GetSheet(EAnimation _eAnimation)
	{
		return (m_tSheets[(int)_eAnimation]);
	}
	
	
	public TSpriteSheet GetActiveSheet()
	{
		return (m_tSheets[(int)m_eActiveAnimation]);
	}
	
	
	// Private:
	

	void Start()
	{
		m_tSheets = new TSpriteSheet[(int)EAnimation.MAX];
		Pawn cPawn = GetComponent<Pawn>();
		GetComponent<PawnController>().StateChangeSubscribers += new PawnController.StateHandler(MotorStateChangeHandler);
		GetComponent<PawnController>().FacingDirectionChangeSubscribers += new PawnController.FacingDirectionHandler(MotorFacingDirectionHandler);
		m_Color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		
		// Setup animations
		if (cPawn.m_ePlayer == Pawn.EPlayer.BLOW)
		{
			GetComponent<AbilityBlow>().StateChangeSubscribers += new AbilityBlow.StateHandler(AbilityStateChangeHandler);
			
			
			m_tSheets[(int)EAnimation.IDLE_LEFT].sFilename 	 = "Sprites/B_PearIdleLeft";
			m_tSheets[(int)EAnimation.IDLE2_LEFT].sFilename  = "Sprites/B_PearIdleLeftBreath";
			m_tSheets[(int)EAnimation.IDLE_RIGHT].sFilename  = "Sprites/B_PearIdleRight";
			m_tSheets[(int)EAnimation.IDLE2_RIGHT].sFilename = "Sprites/B_PearIdleRightBreath";
			
			
			m_tSheets[(int)EAnimation.MOVE_LEFT].sFilename 	= "Sprites/B_PearWalkLeft";
			m_tSheets[(int)EAnimation.MOVE_RIGHT].sFilename = "Sprites/B_PearWalkRight";
			
			
			m_tSheets[(int)EAnimation.JUMP_INITIATE_RIGHT].sFilename = "Sprites/B_PearJumpRightInitiate";
			m_tSheets[(int)EAnimation.FLY_RIGHT].sFilename 		 	 = "Sprites/B_PearJumpRightUp";
			m_tSheets[(int)EAnimation.FLY_PEAK_RIGHT].sFilename 	 = "Sprites/B_PearJumpRightPeak";
			m_tSheets[(int)EAnimation.FALL_RIGHT].sFilename 	 	 = "Sprites/B_PearJumpRightFall";
			m_tSheets[(int)EAnimation.FALL_LAND_RIGHT].sFilename 	 = "Sprites/B_PearJumpRightLand";
			
			
			m_tSheets[(int)EAnimation.JUMP_INITIATE_LEFT].sFilename = "Sprites/B_PearJumpLeftInitiate";
			m_tSheets[(int)EAnimation.FLY_LEFT].sFilename 			= "Sprites/B_PearJumpLeftUp";
			m_tSheets[(int)EAnimation.FLY_PEAK_LEFT].sFilename 		= "Sprites/B_PearJumpLeftPeak";
			m_tSheets[(int)EAnimation.FALL_LEFT].sFilename 			= "Sprites/B_PearJumpLeftFall";
			m_tSheets[(int)EAnimation.FALL_LAND_LEFT].sFilename 	= "Sprites/B_PearJumpLeftLand";
			
			
			m_tSheets[(int)EAnimation.ACTION_CHARGING_LVL_1_LEFT].sFilename  = "Sprites/B_PearSuckChargeUpLvl1Left";
			m_tSheets[(int)EAnimation.ACTION_CHARGING_LVL_1_RIGHT].sFilename = "Sprites/B_PearSuckChargeUpLvl1Right";
			m_tSheets[(int)EAnimation.ACTION_CHARGING_LVL_2_LEFT].sFilename  = "Sprites/B_PearSuckChargeUpLvl2Left";
			m_tSheets[(int)EAnimation.ACTION_CHARGING_LVL_2_RIGHT].sFilename = "Sprites/B_PearSuckChargeUpLvl2Right";
			m_tSheets[(int)EAnimation.ACTION_CHARGING_LVL_3_LEFT].sFilename  = "Sprites/B_PearSuckChargeUpLvl2Left";
			m_tSheets[(int)EAnimation.ACTION_CHARGING_LVL_3_RIGHT].sFilename = "Sprites/B_PearSuckChargeUpLvl2Right";
			m_tSheets[(int)EAnimation.ACTION_RELEASING_LEFT].sFilename 		 = "Sprites/B_PearSuckChannelLeft";
			m_tSheets[(int)EAnimation.ACTION_RELEASING_RIGHT].sFilename 	 = "Sprites/B_PearSuckChannelRight";
		}
		else if (cPawn.m_ePlayer == Pawn.EPlayer.SUCK)
		{
			GetComponent<AbilitySuck>().StateChangeSubscribers += new PawnAbility.StateHandler(AbilityStateChangeHandler);
			
			
			m_tSheets[(int)EAnimation.IDLE_LEFT].sFilename 	 = "Sprites/PearIdleLeft";
			m_tSheets[(int)EAnimation.IDLE2_LEFT].sFilename  = "Sprites/PearIdleLeftBreath";
			m_tSheets[(int)EAnimation.IDLE_RIGHT].sFilename  = "Sprites/PearIdleRight";
			m_tSheets[(int)EAnimation.IDLE2_RIGHT].sFilename = "Sprites/PearIdleRightBreath";
			
			
			m_tSheets[(int)EAnimation.MOVE_LEFT].sFilename 	= "Sprites/PearWalkLeft";
			m_tSheets[(int)EAnimation.MOVE_RIGHT].sFilename = "Sprites/PearWalkRight";
			
			
			m_tSheets[(int)EAnimation.JUMP_INITIATE_RIGHT].sFilename = "Sprites/PearJumpRightInitiate";
			m_tSheets[(int)EAnimation.FLY_RIGHT].sFilename 		 	 = "Sprites/PearJumpRightUp";
			m_tSheets[(int)EAnimation.FLY_PEAK_RIGHT].sFilename 	 = "Sprites/PearJumpRightPeak";
			m_tSheets[(int)EAnimation.FALL_RIGHT].sFilename 	 	 = "Sprites/PearJumpRightFall";
			m_tSheets[(int)EAnimation.FALL_LAND_RIGHT].sFilename 	 = "Sprites/PearJumpRightLand";
			
			
			m_tSheets[(int)EAnimation.JUMP_INITIATE_LEFT].sFilename = "Sprites/PearJumpLeftInitiate";
			m_tSheets[(int)EAnimation.FLY_LEFT].sFilename 			= "Sprites/PearJumpLeftUp";
			m_tSheets[(int)EAnimation.FLY_PEAK_LEFT].sFilename 		= "Sprites/PearJumpLeftPeak";
			m_tSheets[(int)EAnimation.FALL_LEFT].sFilename 			= "Sprites/PearJumpLeftFall";
			m_tSheets[(int)EAnimation.FALL_LAND_LEFT].sFilename 	= "Sprites/PearJumpLeftLand";
			
			
			m_tSheets[(int)EAnimation.ACTION_CHARGING_LVL_1_LEFT].sFilename  = "Sprites/PearSuckChargeUpLvl1Left";
			m_tSheets[(int)EAnimation.ACTION_CHARGING_LVL_1_RIGHT].sFilename = "Sprites/PearSuckChargeUpLvl1Right";
			m_tSheets[(int)EAnimation.ACTION_CHARGING_LVL_2_LEFT].sFilename  = "Sprites/PearSuckChargeUpLvl2Left";
			m_tSheets[(int)EAnimation.ACTION_CHARGING_LVL_2_RIGHT].sFilename = "Sprites/PearSuckChargeUpLvl2Right";
			m_tSheets[(int)EAnimation.ACTION_CHARGING_LVL_3_LEFT].sFilename  = "Sprites/PearSuckChargeUpLvl3Left";
			m_tSheets[(int)EAnimation.ACTION_CHARGING_LVL_3_RIGHT].sFilename = "Sprites/PearSuckChargeUpLvl3Right";
			m_tSheets[(int)EAnimation.ACTION_RELEASING_LEFT].sFilename 		 = "Sprites/PearSuckChannelLeft";
			m_tSheets[(int)EAnimation.ACTION_RELEASING_RIGHT].sFilename 	 = "Sprites/PearSuckChannelRight";
		}
		
		
		for (int i = 0; i < (int)EAnimation.MAX; ++ i)
		{
			m_tSheets[i].cTexture2D = Resources.Load(m_tSheets[i].sFilename, typeof(Texture2D)) as Texture2D;
			m_tSheets[i].fSpeed = 0.07f;
			m_tSheets[i].uiDevisionCount = (uint)(m_tSheets[i].cTexture2D.width / m_tSheets[i].cTexture2D.height);
		}
		
		
		m_tSheets[(int)EAnimation.IDLE_LEFT].uiSpriteCount  = 17;
		m_tSheets[(int)EAnimation.IDLE2_LEFT].uiSpriteCount  = 17;
		m_tSheets[(int)EAnimation.IDLE_RIGHT].uiSpriteCount  = 17;
		m_tSheets[(int)EAnimation.IDLE2_RIGHT].uiSpriteCount = 17;
		m_tSheets[(int)EAnimation.MOVE_LEFT].uiSpriteCount 	 = 14;
		m_tSheets[(int)EAnimation.MOVE_RIGHT].uiSpriteCount  = 14;
		

		m_tSheets[(int)EAnimation.JUMP_INITIATE_RIGHT].uiSpriteCount = 4;
		m_tSheets[(int)EAnimation.FLY_RIGHT].uiSpriteCount 		 	 = 3;
		m_tSheets[(int)EAnimation.FLY_PEAK_RIGHT].uiSpriteCount 	 = 5;
		m_tSheets[(int)EAnimation.FALL_RIGHT].uiSpriteCount 	 	 = 4;
		m_tSheets[(int)EAnimation.FALL_LAND_RIGHT].uiSpriteCount 	 = 4;
		
		
		m_tSheets[(int)EAnimation.JUMP_INITIATE_LEFT].uiSpriteCount = 4;
		m_tSheets[(int)EAnimation.FLY_LEFT].uiSpriteCount 		 	= 3;
		m_tSheets[(int)EAnimation.FLY_PEAK_LEFT].uiSpriteCount 	 	= 5;
		m_tSheets[(int)EAnimation.FALL_LEFT].uiSpriteCount 	 		= 4;
		m_tSheets[(int)EAnimation.FALL_LAND_LEFT].uiSpriteCount 	= 4;
		
		
		m_tSheets[(int)EAnimation.ACTION_CHARGING_LVL_1_LEFT].uiSpriteCount  = 16;
		m_tSheets[(int)EAnimation.ACTION_CHARGING_LVL_1_RIGHT].uiSpriteCount = 16;
		m_tSheets[(int)EAnimation.ACTION_CHARGING_LVL_2_LEFT].uiSpriteCount  = 16;
		m_tSheets[(int)EAnimation.ACTION_CHARGING_LVL_2_RIGHT].uiSpriteCount = 16;
		m_tSheets[(int)EAnimation.ACTION_CHARGING_LVL_3_LEFT].uiSpriteCount  = 16;
		m_tSheets[(int)EAnimation.ACTION_CHARGING_LVL_3_RIGHT].uiSpriteCount = 16;
		m_tSheets[(int)EAnimation.ACTION_RELEASING_LEFT].uiSpriteCount 		 = 19;
		m_tSheets[(int)EAnimation.ACTION_RELEASING_RIGHT].uiSpriteCount 	 = 19;
		
	
		ChangeAnimation(EAnimation.IDLE_RIGHT);
	}
	

	void Update()
	{
		bool bUpdateTile = false;
		
		
		m_fSpriteTimer -= Time.deltaTime;
		
		
		if (m_fSpriteTimer < 0.0f)
		{
			++ m_uiCurrentSprite;
			m_fSpriteTimer = GetActiveSheet().fSpeed;
			
			
			if (m_uiCurrentSprite >= GetActiveSheet().uiSpriteCount)
			{
				if (m_eQueuedAnaimation != EAnimation.INVALID)
				{
					ChangeAnimation(m_eQueuedAnaimation);
					ClearQueue();
				}
				else
				{
					m_uiCurrentSprite = 0;
				}
			}
			
			
			bUpdateTile = true;
		}
		
		
		if (bUpdateTile)
		{
			UpdateTile();
		}
		
		// Change color based on layer
		//float fColor = (float)(3 - (GetComponent<PawnController>().m_iCurrentLevel)) * 1/2;
		//m_Color = new Color(fColor, fColor, fColor, 1.0f);
		//renderer.material.color = m_Color;
	}
	
	
	void UpdateTile()
	{
		float fTileSizeX = 1.0f / GetActiveSheet().uiDevisionCount;
		float fTileOffsetX = fTileSizeX * m_uiCurrentSprite;
		
		
		renderer.material.mainTextureScale = new Vector2(fTileSizeX, 1.0f);
		renderer.material.mainTextureOffset = new Vector2(fTileOffsetX, 0.0f);
	}
	
	
	void UpdateAnimation()
	{
		int iFacingRightIncrement = 0;
		
		
		if (m_ePawnFacingDirection == PawnController.EFacingDirection.RIGHT)
		{
			iFacingRightIncrement = 1;
		}
		
		if (m_eAbilityState == PawnAbility.EState.NONE)
		{
			switch (m_ePawnState)
			{
			case PawnController.EState.GOUND_MOVING:
				ClearQueue();
				if (m_eActiveAnimation == EAnimation.FALL_LAND_LEFT ||
					m_eActiveAnimation == EAnimation.FALL_LAND_RIGHT)
				{
					QueueAnimation(EAnimation.MOVE_LEFT + iFacingRightIncrement);
				}
				else
				{
					ChangeAnimation(EAnimation.MOVE_LEFT + iFacingRightIncrement);
				}
				break;
				
			case PawnController.EState.IDLE:
				QueueAnimation(EAnimation.IDLE_LEFT + iFacingRightIncrement);
				break;
				
			case PawnController.EState.LANDING:
				ClearQueue();
				ChangeAnimation(EAnimation.FALL_LAND_LEFT + iFacingRightIncrement);
				QueueAnimation(EAnimation.IDLE_LEFT + iFacingRightIncrement);
				break;
				
			case PawnController.EState.JUMP_GETTING_READY:
				ClearQueue();
				ChangeAnimation(EAnimation.JUMP_INITIATE_LEFT + iFacingRightIncrement);
				break;
				
			case PawnController.EState.FLYING:
				ChangeAnimation(EAnimation.FLY_LEFT + iFacingRightIncrement);
				break;
				
			case PawnController.EState.FALLING:
				ClearQueue();
				ChangeAnimation(EAnimation.FLY_PEAK_LEFT + iFacingRightIncrement);
				QueueAnimation(EAnimation.FALL_LEFT + iFacingRightIncrement);
				break;
			}
		}
	}
	
	
	// Events:
	
	
	void MotorStateChangeHandler(PawnController.EState _eNewState)
	{
		m_ePawnState = _eNewState;
		UpdateAnimation();
	}
	
	
	void MotorFacingDirectionHandler(PawnController.EFacingDirection _eNewFacingDirection)
	{
		m_ePawnFacingDirection = _eNewFacingDirection;
		UpdateAnimation();
		AbilityStateChangeHandler(m_eAbilityState);
	}
	
	
	void AbilityStateChangeHandler(PawnAbility.EState _eNewState)
	{
		m_eAbilityState = _eNewState;
		int iFacingRightIncrement = 0;
		
		
		if (m_ePawnFacingDirection == PawnController.EFacingDirection.RIGHT)
		{
			iFacingRightIncrement = 1;
		}
		
		
		if (_eNewState == PawnAbility.EState.CHARGING_LVL1)
		{
			ChangeAnimation(EAnimation.ACTION_CHARGING_LVL_1_LEFT + iFacingRightIncrement);
		}
		else if (_eNewState == PawnAbility.EState.CHARGING_LVL2)
		{
			ChangeAnimation(EAnimation.ACTION_CHARGING_LVL_2_LEFT + iFacingRightIncrement);
		}
		else if (_eNewState == PawnAbility.EState.CHARGING_LVL3)
		{
			ChangeAnimation(EAnimation.ACTION_CHARGING_LVL_3_LEFT + iFacingRightIncrement);
		}
		else if (_eNewState == PawnAbility.EState.RELEASING)
		{
			ChangeAnimation(EAnimation.ACTION_RELEASING_LEFT + iFacingRightIncrement);
		}
		else
		{
			ChangeAnimation(EAnimation.IDLE2_LEFT + iFacingRightIncrement);
		}
	}
	
	
// Variables
	
	
	// Public:
	
	
	// Projected:
	
	
	// Private:
	
	
	TSpriteSheet[] m_tSheets;
	
	
	EAnimation m_eActiveAnimation  = EAnimation.INVALID;
	EAnimation m_eQueuedAnaimation = EAnimation.INVALID;
	PawnController.EFacingDirection m_ePawnFacingDirection;
	PawnController.EState m_ePawnState;
	PawnAbility.EState m_eAbilityState;
	
	
	float m_fSpriteTimer;
	
	
	Color m_Color;
	
	
	uint m_uiCurrentSprite;
	
	
}
