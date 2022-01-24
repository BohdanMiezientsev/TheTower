using System.Collections.Generic;
using UnityEngine;

namespace MovementControl
{
    public class StateMachine : MonoBehaviour
    {
        [Header("StateMachine debug")] 
        [SerializeField] private List<BaseState> allStates = new List<BaseState>();
        [SerializeField] private BaseState current = null;
        private MovementManager _movementManager;

        private void Awake()
        {
            allStates.Clear();
        }

        public void SetMovementControl(MovementManager manager)
        {
            this._movementManager = manager;
        }

        // update function for physics.
        public void FixedUpdateState()
        {
            current.ProcessPhysicUpdate();
        }

        // update function for other stuff. called ones per frame.
        public void UpdateState()
        {
            current.ProcessLogicUpdate();
        }


        private void InitState(System.Type type)
        {
            BaseState state = AttachToGameObject(type);
            state.Initialize(this, _movementManager);

            if (!allStates.Contains(state))
                allStates.Add(state);

            TransitionTo(state.GetType());
        }

        
        private BaseState AttachToGameObject(System.Type type)
        {
            GameObject obj = new GameObject();
            obj.transform.parent = this.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.name = type.Name;

            BaseState state = (BaseState) obj.AddComponent(type);

            return state;
        }

        public void TransitionTo(System.Type type)
        {
            BaseState state = GetState(type);

            if (state == null)
            {
                InitState(type);
            }
            else
            {
                if(current)
                    current.OnExit();
                
                Debug.Log("Transitioned to: " + type.Name);
                current = state;
                current.OnEnter();
            }
        }

        private BaseState GetState(System.Type type)
        {
            foreach (BaseState state in allStates)
                if (state.GetType() == type)
                    return state;
            return null;
        }
    }
}