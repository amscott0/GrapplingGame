using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	//This class is the client of the Command pattern.
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool dodge;
		public bool slide;
		public bool grapple;

		public bool switchGrapple;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{

			MoveInput(new Move(value).Execute());
			//consider making a concrete class here and in the other functions that implements a command interface and execute calls MoveInput
			//eg. ConcreteMoveCommand.execute() returns Vector2 newMoveDirection 
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{

				LookInput(new Look(value).Execute());
			}
		}

		public void OnJump(InputValue value)
		{

			JumpInput(new Jump(value).Execute());
		}

		public void OnSprint(InputValue value)
		{

			SprintInput(new Sprint(value).Execute());
		}
		public void OnDodge(InputValue value){

			DodgeInput(new Dodge(value).Execute());
		}
		public void OnSlide(InputValue value){

			SlideInput(new Slide(value).Execute());
		}
		public void OnGrapple(InputValue value){

			GrappleInput(new Grapple(value).Execute());
		}
		public void OnSwitchGrapple(InputValue value){

			SwitchGrappleInput(new SwitchGrapple(value).Execute());
		}
		public void OnExit(InputValue value){
			Application.Quit();
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

		public void SprintInput(bool newSprintState)
		{
			if(newSprintState)
				sprint = !sprint;//newSprintState;
		}
		public void DodgeInput(bool newDodgeState){
			dodge = newDodgeState;
		}
		public void SlideInput(bool newSlideState){
			slide = newSlideState;
		}
		public void GrappleInput(bool newGrappleState){
			grapple = newGrappleState;
		}
		public void SwitchGrappleInput(bool newSwitchGrapple){
			switchGrapple = newSwitchGrapple;
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