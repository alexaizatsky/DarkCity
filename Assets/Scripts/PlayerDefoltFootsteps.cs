using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDefoltFootsteps : MonoBehaviour
{
    public AK.Wwise.Switch footStepSwitchDefolt;
    public GameObject playerRef;

    void Start()
    {
        footStepSwitchDefolt.SetValue(playerRef);
    }


}
