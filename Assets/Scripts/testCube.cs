using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testCube : MonoBehaviour
{
    private Rigidbody myRigi;
    [SerializeField] private bool lineDebug;
    [SerializeField] private LineRenderer debugLine;
    private Transform heroPos;
        
    // Start is called before the first frame update
    void Start()
    {
        myRigi = GetComponent<Rigidbody>();
        if (lineDebug)
        {
            debugLine.gameObject.SetActive(true);
            debugLine.positionCount = 1;
            debugLine.SetPosition(0, this.transform.position);    
        }
        else
            debugLine.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (lineDebug)
        {
            debugLine.positionCount++;
            debugLine.SetPosition(debugLine.positionCount-1, this.transform.position);   
        }
        if(Input.GetKeyDown(KeyCode.Space))
            MakeShoot();
    }
    void OnCollisionEnter(Collision col)
    {
        print("TEST BULLET COLLISION "+col.gameObject.name+" "+System.DateTime.UtcNow.ToString("HH:mm:ss.fff "));
        if(col.gameObject.layer ==11)
        {
           // if (myRigi.velocity.magnitude > 13)
           // {

                // print("ZOMBIE BULLET COLLISION "+col.gameObject.name+" "+System.DateTime.UtcNow.ToString("HH:mm:ss.fff "));
                if (col.gameObject.GetComponent<zombiePartCol>() != null)
                {
                    //=print("ZOMBIE BULLET COLLISION "+col.gameObject.name+" "+System.DateTime.UtcNow.ToString("HH:mm:ss.fff "));
                    // col.gameObject.GetComponent<zombiePartCol>().GetHit(col.contacts[0].point, heroPos.position);
                   // StopShoot();
                    //this.gameObject.SetActive(false);
                }
          //  }
        }
    }
    void MakeShoot()
    {
        myRigi.isKinematic = false;
        myRigi.AddForce(this.transform.forward * 50, ForceMode.Impulse);
    }

}
