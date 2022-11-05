using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class handAnim : MonoBehaviour
{
    public UnityEvent animEvent1;
    public UnityEvent animEvent2;


    
    public void AnimEvent1()
    {
        animEvent1.Invoke();
    }
    public void AnimEvent2()
    {
        animEvent2.Invoke();
    }
}
