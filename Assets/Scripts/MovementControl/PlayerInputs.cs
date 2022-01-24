using UnityEngine;

namespace MovementControl
{
    public class PlayerInputs
    {
        // static field of itself for providing singleton.
        private static PlayerInputs _instance;

        // get method of this class(object) for providing singleton.
        public static PlayerInputs Instance =>
            _instance ?? (_instance = new PlayerInputs());

        // private constructor
        private PlayerInputs()
        {
        }

        // changeable variables for mouse sensitivity. 
        public static float Sensitivity { get; set; } = 50f;
        public static float SensitivityMultiplier { get; set; } = 1f;


        // x - right and left buttons, y - back and forward buttons.
        // mouseX - Horizontal mouse axis, mouseY - Vertical mouse axis.
        private static float _x, _y, _mouseX, _mouseY = 0;
        // booleans for pressed / unpressed buttons.
        private static bool _jumping, _crouching, _use, _drop, _mouse0, _mouse1 = false;

        // should be called manually. Made for easier disabling.
        public static void RefreshInputs()
        {
            _x = Input.GetAxisRaw("Horizontal");
            _y = Input.GetAxisRaw("Vertical");
            _mouseX = Input.GetAxis("Mouse X") * Sensitivity * Time.fixedDeltaTime * SensitivityMultiplier;
            _mouseY = Input.GetAxis("Mouse Y") * Sensitivity * Time.fixedDeltaTime * SensitivityMultiplier;
            _jumping = Input.GetButton("Jump");
            _crouching = Input.GetKey(KeyCode.LeftShift);
            _use = Input.GetKey(KeyCode.E);
            _drop = Input.GetKey(KeyCode.Q);
            _mouse0 = Input.GetMouseButton(0);
            _mouse1 = Input.GetMouseButton(1);
        }

        // getters for input variables. All sets occur in RefreshInputs() method.
        public static float X => _x;

        public static float Y => _y;

        public static float MouseX => _mouseX;

        public static float MouseY => _mouseY;

        public static bool Jumping => _jumping;

        public static bool Crouching => _crouching;

        public static bool Use => _use;

        public static bool Drop => _drop;
        
        public static bool Mouse0 => _mouse0;

        public static bool Mouse1 => _mouse1;
    }
}