using System.Collections.Generic;
using UnityEngine;

namespace MovementControl
{
    public abstract class BaseState : MonoBehaviour
    {
        protected StateMachine stateMachine;
        protected MovementManager MovementManager;

        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void ProcessLogicUpdate();
        public abstract void ProcessPhysicUpdate();

        public void Initialize(StateMachine machine, MovementManager manager)
        {
            this.stateMachine = machine;
            this.MovementManager = manager;
        }
    }
}