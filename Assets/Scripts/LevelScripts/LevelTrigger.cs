using System;
using UnityEngine;

namespace LevelScripts
{
    public class LevelTrigger : MonoBehaviour
    {
        [SerializeField] private LevelTriggerType type;

        private LevelManager _levelManager;
        private bool _isTriggeredAlready = false;

        public void SetLevelManager(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggeredAlready || !other.gameObject.CompareTag("Player"))
            {
                return;
            }

            _isTriggeredAlready = true;
            _levelManager.TriggerWorked(type);
        }
    }
}
