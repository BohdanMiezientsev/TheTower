using UnityEngine;

namespace MovementControl.PlayerPhysicStates
{
    public class JumpingState : BaseState
    {
        [SerializeField] private float jumpCooldown = 0.25f;
        [SerializeField] private float runCooldown = 0.25f;
        [SerializeField] private float jumpForce = 550f;

        public override void OnEnter()
        {
            if (MovementManager.isGrounded)
            {
                MovementManager.isReadyToJump = false;
                // need this, because IsGrounded might not update on time, so
                // Air Lingering state will change to Running instantly. 
                // probably should move it to Running state.
                MovementManager.isReadyToRun = false;

                // resetting Y-axis velocity to 0, so jumping on slopes feel better. COMPULSORY. 
                Vector3 velocity = MovementManager.GetVelocity();
                velocity.y = 0f;
                MovementManager.SetVelocity(velocity);

                // add jump forces. normals works fine. WORKS GREAT AFTER RESETTING VELOCITY. 
                MovementManager.Move(Vector2.up * (jumpForce * 1.5f));
                MovementManager.Move(MovementManager.normalVector * (jumpForce * 0.5f));
            }
        }

        public override void OnExit()
        {
            MovementManager.Invoke(nameof(MovementManager.ResetJump), jumpCooldown);
            MovementManager.Invoke(nameof(MovementManager.ResetRun), runCooldown);
        }

        // ready for now
        public override void ProcessLogicUpdate()
        {
            stateMachine.TransitionTo(typeof(AirLingeringState));
        }

        public override void ProcessPhysicUpdate()
        {
        }
    }
}