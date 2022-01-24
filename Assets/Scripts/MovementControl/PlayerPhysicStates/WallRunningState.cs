using System;
using UnityEngine;

namespace MovementControl.PlayerPhysicStates
{
    public class WallRunningState : BaseState
    {
        [SerializeField] private float wallRunForce = 2000f;
        [SerializeField] private float maxWallSpeed = 25;
        [SerializeField] private float wallRunCooldown = 0.50f;
        [SerializeField] private float fallDelay = 0.50f;

        private float delay = 0f;
        public override void OnEnter()
        {
            // disabling gravity for player so we have full control on Y-axis movement while wall running. 
            MovementManager.SetGravity(false);
            // some flags.
            MovementManager.isWallRunning = true;
            MovementManager.isReadyToWallRun = false;
            delay = fallDelay;
            // removing Y-axis velocity on state enter, so we have consistent up/down movement.
            Vector3 velocity = MovementManager.GetVelocity();
            MovementManager.SetVelocity(new Vector3(velocity.x, 7f, velocity.z));
        }

        public override void OnExit()
        {
            MovementManager.isWallRunning = false;
            MovementManager.Invoke(nameof(MovementManager.ResetWallRun), wallRunCooldown);
            // returning gravity to player back.
            MovementManager.SetGravity(true);
        }

        // ready for now
        public override void ProcessLogicUpdate()
        {
            // if you dont hold any X-axis buttons, delay value reduces.
            // good solution. I like it. wall running is done.
            if (Math.Abs(PlayerInputs.X) < 0.5f)
                delay -= Time.deltaTime;
            else
                delay = fallDelay;
            
            if (!MovementManager.isWallLeft && !MovementManager.isWallRight || delay <= 0f ||MovementManager.isGrounded)
                stateMachine.TransitionTo(typeof(AirLingeringState));
            else if (PlayerInputs.Jumping && MovementManager.isReadyToJump)
                stateMachine.TransitionTo(typeof(WallJumpingState));
        }

        public override void ProcessPhysicUpdate()
        {
            Transform orientation = MovementManager.orientation;

            if (MovementManager.GetVelocity().magnitude <= maxWallSpeed)
            {
                MovementManager.Move(orientation.forward * (wallRunForce * Time.deltaTime));

                if (MovementManager.isWallRight)
                    MovementManager.Move(orientation.right * wallRunForce / 10 * Time.deltaTime);
                else if (MovementManager.isWallLeft)
                    MovementManager.Move(-orientation.right * wallRunForce / 10 * Time.deltaTime);
            }

            MovementManager.Move(-orientation.up * wallRunForce / 4 * Time.deltaTime);
        }
    }
}