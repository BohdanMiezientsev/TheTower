using TMPro;
using UnityEngine;

namespace MovementControl.PlayerPhysicStates
{
    public class VaultingState : BaseState
    {
        [SerializeField] private float vaultCooldown = 0.5f;

        [SerializeField] private bool inAction;
        [SerializeField] private bool verticalDone;
        [SerializeField] private Vector3 targetPos;
        [SerializeField] private Vector3 verticalTargetPos;
        private Vector3 initialVelocity;
        private Vector3 playerPosition;

        public override void OnEnter()
        {
            inAction = false;
            playerPosition = MovementManager.transform.position;

            // find center point (transform) for future CapsuleCast 1.5f forward and 3f higher from players transform. 
            Vector3 forwardCapsuleCenterPoint =
                playerPosition + MovementManager.orientation.forward * 1.5f + Vector3.up * 3f;

            // cast capsule same as player to find position to move.  
            Physics.CapsuleCast(forwardCapsuleCenterPoint + (Vector3.up * 0.25f),
                forwardCapsuleCenterPoint + (Vector3.up * -0.25f),
                0.5f, Vector3.down, out RaycastHit hit, 3f, 1 << 8);

            // checking angle. if "not walkable", then do not vault on it and put vaulting on cooldown.
            // and if hit.normal == "0,0,0" (there is no hit), then IsFloor will return false. So no worries here.
            if (!MovementManager.IsFloor(hit.normal))
            {
                Debug.Log("hit point = " + hit.point + " hit.normal = " + hit.normal);
                MovementManager.isReadyToVault = false;
                return;
            }

            // this is our target position to move.
            targetPos = hit.point + Vector3.up * (MovementManager.transform.localScale.y + 0.2f);

            // calculating distance between cast hitPoint and players transform (always positive)
            float distance = hit.point.y - playerPosition.y;


            // another capsule cast, but this time over the player with height taken from target position.
            // if we collide with something -> do not make any vaulting. 
            if (!Physics.CapsuleCast(playerPosition + (Vector3.up * 0.25f), playerPosition + (Vector3.up * -0.25f),
                0.5f, Vector3.up, out RaycastHit hit2, distance + 1.5f, 1 << 8))
            {
                verticalTargetPos = new Vector3(playerPosition.x, targetPos.y, playerPosition.z);
                verticalDone = false;
                inAction = true;
                MovementManager.SetGravity(false);
                initialVelocity = MovementManager.GetVelocity();
                initialVelocity.y = 0f;
                MovementManager.SetVelocity(Vector3.zero);
            }
        }

        public override void OnExit()
        {
            MovementManager.SetGravity(true);
            MovementManager.Invoke(nameof(MovementManager.ResetVault), vaultCooldown);
        }

        // its okay
        public override void ProcessLogicUpdate()
        {
            if (!inAction)
                stateMachine.TransitionTo(typeof(AirLingeringState));
        }

        public override void ProcessPhysicUpdate()
        {
            // if action is dismissed. return. next logic update will go to another state
            if (!inAction) return;


            playerPosition = MovementManager.transform.position;
            if (!verticalDone)
            {
                MovementManager.transform.position = Vector3.MoveTowards(playerPosition, verticalTargetPos, 0.25f);
                if (verticalTargetPos.Compare(playerPosition, 100))
                    verticalDone = true;
            }
            else
                MovementManager.transform.position = Vector3.MoveTowards(playerPosition, targetPos, 0.25f);

            if (!targetPos.Compare(playerPosition, 100)) return;

            inAction = false;
            MovementManager.SetVelocity(initialVelocity * 0.7f);
        }
    }
}