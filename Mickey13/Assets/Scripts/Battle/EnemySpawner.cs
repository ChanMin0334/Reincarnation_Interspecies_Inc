using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform; // 따라갈 카메라

    private Vector3 initialPosition; // EnemySpawner의 초기 위치

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        // 카메라의 X 위치 + 초기 X 위치, Y/Z는 초기값 유지
        if (cameraTransform != null)
        {
            transform.position = new Vector3(
                cameraTransform.position.x + initialPosition.x,
                initialPosition.y,
                initialPosition.z
            );
        }
    }
}
