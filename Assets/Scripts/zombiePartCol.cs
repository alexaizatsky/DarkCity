using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zombiePartCol : MonoBehaviour
{
    public zombie myZombie;
    public Collider myCol;
    public void Init(zombie _zombie)
    {
        myZombie = _zombie;
        myCol = GetComponent<Collider>();
    }

    public void GetHit(Vector3 _point, Vector3 _startPoint)
    {
        Vector3 dir = (_point - _startPoint).normalized * 10;
        //print("ZOMBIE PART GET HIT "+myCol.gameObject.name+" "+System.DateTime.UtcNow.ToString("HH:mm:ss.fff "));
        myZombie.GetHit(myCol, _point, dir);
    }

    public void GetExplosionHit()
    {
        myZombie.GetExplosionHit();
    }
}
