using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool shrink;
        public bool grow;
		public bool action;
        public bool reset;

        [Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnShrink(InputValue value)
		{
			ShrinkInput(value.isPressed);
		}

        public void OnGrow(InputValue value)
        {
            GrowInput(value.isPressed);
        }

		public void OnAction(InputValue value)
		{
			ActionInput(value.isPressed);
		}

		public void OnReset(InputValue value)
		{
			ResetInput(value.isPressed);
		}
#endif


        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void ShrinkInput(bool newShrinkState)
		{
			shrink = newShrinkState;
		}

        public void GrowInput(bool newGrowState)
        {
            grow = newGrowState;
        }

		public void ActionInput(bool newActionState)
		{
			action = newActionState;
		}

        public void ResetInput(bool newResetState)
        {
            reset = newResetState;
        }

        private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}