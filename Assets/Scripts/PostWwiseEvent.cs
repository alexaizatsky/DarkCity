using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostWwiseEvent : MonoBehaviour
{
    public AK.Wwise.Event myEvent;

    // Start is called before the first frame update
    public void PlayZombieFootsteps()
    {
        myEvent.Post(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
