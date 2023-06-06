using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosions : MonoBehaviour
{
    public float radius;
    public float force;

    public bool isActive;

    public GameObject explosionEffect;

    private bool isExploded;

    public AK.Wwise.Event soundEvent;

    private void Update()
    {
        if (isActive)
        {
        ExplodeWithDelay();

        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<bullet>())
        {
            isActive = true;
        }
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
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        soundEvent.Post(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, radius / 2f);

    }




}
