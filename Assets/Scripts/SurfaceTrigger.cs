using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceTrigger : MonoBehaviour
{
    public GameObject playerRef;
    public AK.Wwise.Switch footStepSwitch;
    public AK.Wwise.Switch footStepSwitchDefolt;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerRef)
        {
            footStepSwitch.SetValue(playerRef);
            Debug.Log("Enter Zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerRef)
        {
            footStepSwitchDefolt.SetValue(playerRef);
            Debug.Log("Exit Zone");
        }
    }
}
