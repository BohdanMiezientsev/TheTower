using UnityEngine;

namespace MovementControl.PlayerPhysicStates
{
    public class WallJumpingState : BaseState
    {
        [SerializeField] private float jumpCooldown = 0.25f;
        [SerializeField] private float jumpForce = 550f;

        public override void OnEnter()
        {
            MovementManager.isReadyToJump = false;
            MovementManager.Move(Vector2.up * (jumpForce * 1f));

            if (MovementManager.isWallRight)
                MovementManager.Move(-MovementManager.orientation.right *
                                     (jumpForce * (PlayerInputs.X <= -0.5f ? 3.2f : 1f)));
            if (MovementManager.isWallLeft)
                MovementManager.Move(
                    MovementManager.orientation.right * (jumpForce * (PlayerInputs.X >= 0.5f ? 3.2f : 1f)));

            //Always add forward force
            MovementManager.Move(MovementManager.orientation.forward * (jumpForce * 1f));
        }

        public override void OnExit()
        {
            MovementManager.Invoke(nameof(MovementManager.ResetJump), jumpCooldown);
        }

        //ready for now
        public override void ProcessLogicUpdate()
        {
            stateMachine.TransitionTo(typeof(AirLingeringState));
        }

        public override void ProcessPhysicUpdate()
        {
        }
    }
}