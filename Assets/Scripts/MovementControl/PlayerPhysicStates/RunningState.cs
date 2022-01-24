using System;
using UnityEngine;

namespace MovementControl.PlayerPhysicStates
{
    public class RunningState : BaseState
    {
        [SerializeField] private float gravityMultiplier = 10f;
        [SerializeField] private float moveSpeed = 3000;
        [SerializeField] private float maxSpeed = 25;
        [SerializeField] private float counterMovement = 0.175f;
        [SerializeField] private float threshold = 0.01f;

        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
        }

        // ready for now
        public override void ProcessLogicUpdate()
        {
            if (PlayerInputs.Crouching)
                stateMachine.TransitionTo(typeof(CrouchingState));
            else if (PlayerInputs.Jumping && MovementManager.isReadyToJump)
                stateMachine.TransitionTo(typeof(JumpingState));
            else if(PlayerInputs.Y >=0.5 && MovementManager.isWallForward && MovementManager.isReadyToVault)
                stateMachine.TransitionTo(typeof(VaultingState));
            else if (MovementManager.GetVelocity().y < -0.1)
                stateMachine.TransitionTo(typeof(AirLingeringState));
        }

        public override void ProcessPhysicUpdate()
        {
            MovementManager.Move(Vector3.down * (Time.deltaTime * gravityMultiplier));
            Vector2 magnitude = MovementManager.GetVelocityRelativeToLook();
            float magnitudeX = magnitude.x, magnitudeY = magnitude.y;

            float x = PlayerInputs.X, y = PlayerInputs.Y;
            CounterMovement(magnitude, x, y);

            float max = this.maxSpeed;

            // if speed is larger than maxSpeed, cancel out the input so you don't go over max speed
            if (x > 0 && magnitudeX > max) x = 0;
            if (x < 0 && magnitudeX < -max) x = 0;
            if (y > 0 && magnitudeY > max) y = 0;
            if (y < 0 && magnitudeY < -max) y = 0;

            MovementManager.Move(MovementManager.orientation.transform.forward * (y * moveSpeed * Time.deltaTime));
            MovementManager.Move(MovementManager.orientation.transform.right * (x * moveSpeed * Time.deltaTime));
        }

        private void CounterMovement(Vector2 magnitude, float x, float y)
        {
            //counter movement
            if (Math.Abs(magnitude.x) > threshold && Math.Abs(x) < 0.05f || (magnitude.x < -threshold && x > 0) ||
                (magnitude.x > threshold && x < 0))
                MovementManager.Move(MovementManager.orientation.transform.right *
                                     (moveSpeed * Time.deltaTime * -magnitude.x * counterMovement));
            
            if (Math.Abs(magnitude.y) > threshold && Math.Abs(y) < 0.05f || (magnitude.y < -threshold && y > 0) ||
                (magnitude.y > threshold && y < 0))
                MovementManager.Move(MovementManager.orientation.transform.forward *
                                     (moveSpeed * Time.deltaTime * -magnitude.y * counterMovement));

            Vector3 velocity = MovementManager.GetVelocity();
            // limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
            if (Mathf.Sqrt(Mathf.Pow(velocity.x, 2) + Mathf.Pow(velocity.z, 2)) > maxSpeed)
            {
                Vector3 n = velocity.normalized * maxSpeed;
                MovementManager.SetVelocity(new Vector3(n.x, velocity.y, n.z));
            }
        }
    }
}