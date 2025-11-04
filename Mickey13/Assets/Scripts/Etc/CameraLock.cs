using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLock : MonoBehaviour
{
    public Transform target; // 따라갈 타겟

    void Update()
    {
        if (target != null)
        {
            transform.position = new Vector3(target.position.x + 2, transform.position.y, transform.position.z);
        }
    }
}
