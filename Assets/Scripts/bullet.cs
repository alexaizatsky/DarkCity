using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [SerializeField] private bool lineDebug;
    [SerializeField] private Rigidbody myRigi;
    [SerializeField] private LineRenderer debugLine;
    private Transform heroPos;
  
    public void StartShoot(Transform _pos)
    {
        heroPos = _pos;
        if (lineDebug)
        {
            debugLine.gameObject.SetActive(true);
            debugLine.positionCount = 1;
            debugLine.SetPosition(0, this.transform.position);    
        }
        else
            debugLine.gameObject.SetActive(false);
        
        myRigi.isKinematic = false;
        myRigi.AddForce(this.transform.forward * 70, ForceMode.Impulse);
    }

    public void StopShoot()
    {
        myRigi.isKinematic = true;
    }
    void OnCollisionEnter(Collision col)
    {
        //print(col.gameObject.name+" BULLET COL"+" "+System.DateTime.UtcNow.ToString("HH:mm:ss.fff "));
        if (col.gameObject.layer == 9)
        {
            //print(col.gameObject.name+" BULLET COL vel"+myRigi.velocity.magnitude);
            if(myRigi.velocity.magnitude > 13)
                gunLevelSystem.RegisterBulletCollision(new bulletCollisionDataContainer(col.contacts[0].point, heroPos, 9));
        }
        else if(col.gameObject.layer ==11)
        {
            if (myRigi.velocity.magnitude > 13)
            {

                // print("ZOMBIE BULLET COLLISION "+col.gameObject.name+" "+System.DateTime.UtcNow.ToString("HH:mm:ss.fff "));
                if (col.gameObject.GetComponent<zombiePartCol>() != null)
                {
                    //print("ZOMBIE BULLET COLLISION "+col.gameObject.name+" "+System.DateTime.UtcNow.ToString("HH:mm:ss.fff "));
                   // col.gameObject.GetComponent<zombiePartCol>().GetHit(col.contacts[0].point, heroPos.position);
                   // StopShoot();
                    //this.gameObject.SetActive(false);
                }
            }
        }
    }


    void Update()
    {
        if (lineDebug)
        {
            debugLine.positionCount++;
            debugLine.SetPosition(debugLine.positionCount-1, this.transform.position);   
        }

    }
}
