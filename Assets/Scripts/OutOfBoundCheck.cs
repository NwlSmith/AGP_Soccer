using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundCheck : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!Services.GameStateController.isPaused && other.CompareTag("Ball"))
        {
            Debug.Log("Ball exited field.");
            Services.ball.position = Services.SceneObjectIndex.ballInitPos;
        }
    }
}
