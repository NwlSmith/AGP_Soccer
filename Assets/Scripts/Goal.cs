using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Creator: Nate Smith
 * Date: 2/5/2021
 * Description: Simple goal script.
 */
public class Goal : MonoBehaviour
{
    public bool isBlue = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Debug.Log("Ball entered goal");
            Services.EventManager.Fire(new GoalScored(isBlue));
        }
    }

}
