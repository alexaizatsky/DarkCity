using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Explosions : MonoBehaviour
{
    public float radius;
    public float force;

    public bool isActive;

    public GameObject explosionEffect;

    private bool isExploded;

    public AK.Wwise.Event soundEvent;

    private GameObject fxObj;
  

/*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<bullet>())
        {
            ExplodeWithDelay();
        }
    }
*/
    public void GetBullet()
    {
       Explode();
    }
    
    public void ExplodeWithDelay()
    {
        if (isExploded) return;
        isExploded = true;
        Invoke("Explode", 0.5f);
    }

    void Explode()
    {

        Collider[] overlapedColliders = Physics.OverlapSphere(transform.position, radius);

        for (int i = 0; i < overlapedColliders.Length; i++)
        {
            print("GET COLLIDER IN EXPLOSION "+overlapedColliders[i].gameObject.name);
            if (overlapedColliders[i].GetComponent<zombiePartCol>() != null)
            {
                overlapedColliders[i].GetComponent<zombiePartCol>().GetExplosionHit(); 
            }
            Rigidbody rigidbody = overlapedColliders[i].attachedRigidbody;
            if (rigidbody)
            {
                rigidbody.AddExplosionForce(force, transform.position, radius, 1f);

                Explosions explosions = rigidbody.GetComponent<Explosions>();
                if (explosions)
                {
                    if (Vector3.Distance(transform.position, rigidbody.position) <
                        radius / 2f)
                    {
                        explosions.ExplodeWithDelay();

                    }
                }
            }
        }

        Destroy(gameObject);
        fxObj = Instantiate(explosionEffect, transform.position+new Vector3(0,.5f, 0), Quaternion.Euler(new Vector3(-90,0,0)));
        soundEvent.Post(gameObject);
        DOVirtual.DelayedCall(5, () =>
        {
            if(fxObj!=null) Destroy(fxObj.gameObject);
        });
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, radius / 2f);

    }




}
