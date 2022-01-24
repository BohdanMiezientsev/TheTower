using System;
using MovementControl.PlayerPhysicStates;
using Unity.Collections;
using UnityEngine;

// TODO split this class on All necessary transforms, updates, checks AND cooldown flags, invoke methods.Or move them to state classes.
namespace MovementControl
{
    public class MovementManager : MonoBehaviour
    {
        [SerializeField] private StateMachine stateMachine;

        // necessary
        [SerializeField] public Transform playerCam;
        [SerializeField] public Transform orientation;

        // rb of player
        private Rigidbody _rigidBody;

        // rotation and look
        private float xRotation;
        private float desiredX;

        // for checkers
        [SerializeField] public float maxSlopeAngle = 35f;
        [SerializeField] public LayerMask whatIsGround;
        [SerializeField] public LayerMask whatIsWall;
        [ReadOnly] public Vector3 normalVector = Vector3.up;

        // bool flags
        [ReadOnly] public bool isGrounded;
        [ReadOnly] public bool isWallRunning;
        [ReadOnly] public bool isReadyToJump = true;
        [ReadOnly] public bool isReadyToRun = true;
        [ReadOnly] public bool isReadyToWallRun = true;
        [ReadOnly] public bool isReadyToVault = true;
        [ReadOnly] public bool isReadyToCrouch = true;

        // wall flags
        [ReadOnly] public bool isWallRight;
        [ReadOnly] public bool isWallLeft;
        [ReadOnly] public bool isWallForward;

        // camera tilt
        [SerializeField] public float maxWallRunCameraTilt;
        [SerializeField] public float wallRunCameraTilt;

        public static bool IsLookUnlocked = true;
        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            // locking cursor 
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1;
            IsLookUnlocked = true;

            // adding StateMachine if not connected
            if (!stateMachine)
            {
                GameObject obj = new GameObject("StateMachine");
                obj.transform.parent = this.transform;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                stateMachine = obj.AddComponent<StateMachine>();
            }

            stateMachine.SetMovementControl(this);
            stateMachine.TransitionTo(typeof(RunningState));
        }

        private void Update()
        {
            PlayerInputs.RefreshInputs();
            Look();
            CheckForWall();
            stateMachine.UpdateState();
        }

        private void FixedUpdate()
        {
            stateMachine.FixedUpdateState();
        }

        private void Look()
        {
            if (!IsLookUnlocked)
            {
                return;
            }
            
            float mouseX = PlayerInputs.MouseX;
            float mouseY = PlayerInputs.MouseY;

            // find current look rotation
            Vector3 rot = playerCam.transform.localRotation.eulerAngles;
            desiredX = rot.y + mouseX;

            // change x rotation variable. By rotating on x-axis, y-axis changes,
            // and also make sure we dont over- or under-rotate.
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // perform the rotations
            playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, wallRunCameraTilt);
            orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);

            // while WallRunning
            // tilts camera in .5 second
            if (Math.Abs(wallRunCameraTilt) < maxWallRunCameraTilt && isWallRunning && isWallRight)
                wallRunCameraTilt += Time.deltaTime * maxWallRunCameraTilt * 2;
            if (Math.Abs(wallRunCameraTilt) < maxWallRunCameraTilt && isWallRunning && isWallLeft)
                wallRunCameraTilt -= Time.deltaTime * maxWallRunCameraTilt * 2;

            // tilts camera back again
            if (wallRunCameraTilt > 0 && !isWallRunning)
                wallRunCameraTilt -= Time.deltaTime * maxWallRunCameraTilt * 2;
            if (wallRunCameraTilt < 0 && !isWallRunning)
                wallRunCameraTilt += Time.deltaTime * maxWallRunCameraTilt * 2;
        }

        public void Move(Vector3 force)
        {
            _rigidBody.AddForce(force);
        }

        public Vector3 GetVelocity()
        {
            return _rigidBody.velocity;
        }

        public void SetVelocity(Vector3 velocity)
        {
            _rigidBody.velocity = velocity;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public Vector2 GetVelocityRelativeToLook()
        {
            float lookAngle = orientation.transform.eulerAngles.y;
            float moveAngle = Mathf.Atan2(_rigidBody.velocity.x, _rigidBody.velocity.z) * Mathf.Rad2Deg;

            float u = Mathf.DeltaAngle(lookAngle, moveAngle);
            float v = 90 - u;

            float magnitude = _rigidBody.velocity.magnitude;
            float yMag = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
            float xMag = magnitude * Mathf.Cos(v * Mathf.Deg2Rad);

            return new Vector2(xMag, yMag);
        }

        private void CheckForWall() //make sure to call in regular Update
        {
            isWallRight = Physics.Raycast(transform.position, orientation.right, 1f, whatIsWall);
            isWallLeft = Physics.Raycast(transform.position, -orientation.right, 1f, whatIsWall);
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("entered");
            int layer = other.gameObject.layer;
            if (whatIsGround != (whatIsGround | (1 << layer))) return;

            isWallForward = true;
        }
        
        private void OnTriggerExit(Collider other)
        {
            Debug.Log("exited");
            int layer = other.gameObject.layer;
            if (whatIsGround != (whatIsGround | (1 << layer))) return;

            isWallForward = false;
        }

        private void OnCollisionStay(Collision other)
        {
            // make sure we are only checking for walkable layers
            int layer = other.gameObject.layer;
            if (whatIsGround != (whatIsGround | (1 << layer))) return;

            // iterate through every collision in a physics update
            for (int i = 0; i < other.contactCount; i++)
            {
                Vector3 normal = other.contacts[i].normal;
                if (IsFloor(normal))
                {
                    isGrounded = true;
                    normalVector = normal;
                }
            }
        }

        private void OnCollisionExit(Collision other)
        {
            int layer = other.gameObject.layer;
            if (whatIsGround != (whatIsGround | (1 << layer))) return;

            isGrounded = false;
        }

        public bool IsFloor(Vector3 v)
        {
            if (v == Vector3.zero)
                return false;
            
            float angle = Vector3.Angle(Vector3.up, v);
            return angle < maxSlopeAngle;
        }

        public void ResetJump()
        {
            isReadyToJump = true;
        }

        public void ResetRun()
        {
            isReadyToRun = true;
        }

        public void ResetWallRun()
        {
            isReadyToWallRun = true;
        }

        public void ResetVault()
        {
            isReadyToVault = true;
        }

        public void ResetCrouch()
        {
            isReadyToCrouch = true;
        }

        public void SetGravity(bool gravity)
        {
            _rigidBody.useGravity = gravity;
        }
    }
}