using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    [SerializeField] private Transform player;

    private void Update() {
        transform.position = player.transform.position;
    }
}