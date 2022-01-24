using UnityEngine;

namespace MovementControl.PlayerPhysicStates
{
    public class CrouchingState : BaseState
    {
        [SerializeField] private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
        [SerializeField] private float slideForce = 500;
        [SerializeField] private float moveSpeed = 3000;
        [SerializeField] private float slideCounterMovement = 0.35f;
        [SerializeField] private float crouchGravityMultiplier = 10f;
        [SerializeField] private float crouchCooldown = 0.5f;
        [SerializeField] private float jumpCooldown = 0.30f;

        private Vector3 standardPlayerScale;

        public override void OnEnter()
        {
            standardPlayerScale = MovementManager.transform.localScale;
            MovementManager.transform.localScale = crouchScale;
            Vector3 position = MovementManager.GetPosition();
            MovementManager.SetPosition(new Vector3(position.x, position.y - 0.5f, position.z));
            
            MovementManager.isReadyToCrouch = false;
            // need this so we have time to land while crouching, and jump only after we land fully. 
            MovementManager.isReadyToJump = false;
            MovementManager.Invoke(nameof(MovementManager.ResetJump), jumpCooldown);
            

            if (MovementManager.GetVelocity().magnitude > 0.5f && MovementManager.isGrounded)
                MovementManager.Move(MovementManager.orientation.transform.forward * slideForce);
        }

        public override void OnExit()
        {
            var tr = MovementManager.transform;
            tr.localScale = standardPlayerScale;
            var position = MovementManager.GetPosition();
            tr.position = new Vector3(position.x, position.y + 0.5f, position.z);
            MovementManager.Invoke(nameof(MovementManager.ResetCrouch), crouchCooldown);
        }

        // ready for now
        public override void ProcessLogicUpdate()
        {
            if (!PlayerInputs.Crouching)
                stateMachine.TransitionTo(typeof(RunningState));
            else if (PlayerInputs.Jumping && MovementManager.isReadyToJump)
                stateMachine.TransitionTo(typeof(JumpingState));
        }

        public override void ProcessPhysicUpdate()
        {
            MovementManager.Move(Vector3.down * (Time.deltaTime * crouchGravityMultiplier));
            CounterMovement();
            if (MovementManager.isGrounded && MovementManager.isReadyToJump)
                MovementManager.Move(Vector3.down * (Time.deltaTime * 5000f));
        }

        private void CounterMovement()
        {
            MovementManager.Move(-MovementManager.GetVelocity().normalized *
                                 (moveSpeed * Time.deltaTime * slideCounterMovement));
        }
    }
}