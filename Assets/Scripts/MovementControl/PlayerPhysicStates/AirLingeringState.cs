using System;
using UnityEngine;

namespace MovementControl.PlayerPhysicStates
{
    public class AirLingeringState : BaseState
    {
        [SerializeField] private float gravityMultiplier = 10f;
        [SerializeField] private float maxSpeed = 25;
        [SerializeField] private float maxDiagonalSpeed = 35;
        [SerializeField] private float airMoveSpeed = 3000;
        [SerializeField] private float counterMovement = 0.007f;
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
            if (MovementManager.isGrounded && MovementManager.isReadyToRun)
                stateMachine.TransitionTo(typeof(RunningState));

            else if ((PlayerInputs.X >= 0.5f && MovementManager.isWallRight ||
                      PlayerInputs.X <= -0.5f && MovementManager.isWallLeft) && !MovementManager.isGrounded &&
                     MovementManager.isReadyToWallRun)
                stateMachine.TransitionTo(typeof(WallRunningState));

            else if (PlayerInputs.Y >= 0.5 && MovementManager.isWallForward && MovementManager.isReadyToVault)
                stateMachine.TransitionTo(typeof(VaultingState));
        }

        public override void ProcessPhysicUpdate()
        {
            MovementManager.Move(Vector3.down * (Time.deltaTime * gravityMultiplier));

            Vector2 magnitude = MovementManager.GetVelocityRelativeToLook();
            float magnitudeX = magnitude.x, magnitudeY = magnitude.y;
            float x = PlayerInputs.X, y = PlayerInputs.Y;
            CounterMovement(magnitude, x, y);

            float max = this.maxSpeed;

            //If speed is larger than maxSpeed, cancel out the input so you don't go over max speed
            if (x > 0 && magnitudeX > max) x = 0;
            if (x < 0 && magnitudeX < -max) x = 0;
            if (y > 0 && magnitudeY > max) y = 0;
            if (y < 0 && magnitudeY < -max) y = 0;

            MovementManager.Move(MovementManager.orientation.transform.forward *
                                 (y * airMoveSpeed * Time.deltaTime * 0.5f));
            MovementManager.Move(MovementManager.orientation.transform.right * (x * airMoveSpeed * Time.deltaTime));
        }


        private void CounterMovement(Vector2 magnitude, float x, float y)
        {
            //counter movement
            if (Math.Abs(magnitude.x) > threshold && Math.Abs(x) < 0.05f || (magnitude.x < -threshold && x > 0) ||
                (magnitude.x > threshold && x < 0))
                MovementManager.Move(MovementManager.orientation.transform.right *
                                     (airMoveSpeed * Time.deltaTime * -magnitude.x * counterMovement));

            if (Math.Abs(magnitude.y) > threshold && Math.Abs(y) < 0.05f || (magnitude.y < -threshold && y > 0) ||
                (magnitude.y > threshold && y < 0))
                MovementManager.Move(MovementManager.orientation.transform.forward *
                                     (airMoveSpeed * Time.deltaTime * -magnitude.y * counterMovement));
            
            Vector3 velocity = MovementManager.GetVelocity();
            // limit diagonal swinging. max speed increased.
            if (Mathf.Sqrt(Mathf.Pow(velocity.x, 2) + Mathf.Pow(velocity.z, 2)) > maxDiagonalSpeed)
            {
                Vector3 n = velocity.normalized * maxDiagonalSpeed;
                MovementManager.SetVelocity(new Vector3(n.x, velocity.y, n.z));
            }
        }
    }
}