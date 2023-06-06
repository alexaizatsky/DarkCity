using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.AI;

public class zombie : MonoBehaviour
{
    [SerializeField] private Animator myAnim;
    [SerializeField] private NavMeshAgent myNavMesh;
    [SerializeField] private RagdollUtility myRagdollUtility;
    [SerializeField] private HitReaction myHitReaction;
    [SerializeField] private ZombiePartContainer[] zombieParts;
    private Sequence seq;
    private Transform heroT;

    public AK.Wwise.Event roaring;

    public enum State
    {
        idle,
        move,
        crawl,
        scream,
        attack,
        die,
    }

    public State myState;

 
    void Start()
    {
        Init();
    }
    
    public void Init()
    {
        for (int i = 0; i < zombieParts.Length; i++)
        {
            zombieParts[i].Init(this);
        }
        FindHero();
        SetState(State.move);
    }

    void SetState(State s)
    {
        print("SetState "+s+" "+System.DateTime.UtcNow.ToString("HH:mm:ss.fff "));
        myState = s;
        switch (s)
        {
            case State.idle:
                myAnim.SetInteger("animInt",0);
                break;
            case State.move:
                myAnim.SetInteger("animInt",1);
                break;
            case State.crawl:
                if(seq!=null) seq.Kill();
                if (!myNavMesh.enabled) myNavMesh.enabled = true;
                myAnim.SetInteger("animInt",2);
                break;
            case State.scream:
                myAnim.SetInteger("animInt",3);
                DOVirtual.DelayedCall(1.5f, AfterScreamAttack);
                roaring.Post(gameObject);
                break;
            case State.attack:
                myAnim.SetInteger("animInt",4);
                break;
            case State.die:
                if(seq!=null) seq.Kill();
                myNavMesh.enabled = false;
                myRagdollUtility.EnableRagdoll();
                DOVirtual.DelayedCall(2, DelayDie);
                break;
        }
    }

    void FindHero()
    {
        heroT = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public void GetHit(Collider _col, Vector3 _point, Vector3 _force)
    {
        GetHitToZombiePart(_col);
        myHitReaction.Hit(_col, _force, _point);
    }

    void AfterScreamAttack()
    {
        print("zombiew After Scream "+" "+System.DateTime.UtcNow.ToString("HH:mm:ss.fff "));
        SetState(State.attack);
        Vector3 dir = (heroT.position - this.transform.position).normalized;
        dir.y = 0;
        this.transform.forward = dir;
        Vector3 attackPos = this.transform.position;
        seq = DOTween.Sequence();
        seq.Append(this.transform.DOMove(attackPos + dir, 2f));
        seq.Append(this.transform.DOMove(attackPos, 2f));
        seq.Play().OnComplete(ReturnToScream);
    }

    void ReturnToScream()
    {
        print("zombiew Return Scream "+" "+System.DateTime.UtcNow.ToString("HH:mm:ss.fff "));
        SetState(State.scream);
    }


    void DelayDie()
    {
        this.gameObject.SetActive(false);
        myNavMesh.enabled = transform;
        myRagdollUtility.DisableRagdoll();
    }
    public void GetHitToZombiePart(Collider _col)
    {
        ZombiePartContainer zpc = new ZombiePartContainer();
        for (int i = 0; i < zombieParts.Length; i++)
        {
           zpc = zombieParts[i].GetPartToHit(_col);
           if (zpc.name!="EMPTY")
           {
               break;
           }
        }

        if (zpc.name!="EMPTY")
        {
            zpc.partHealth -= 2;
            if (zpc.partHealth <= 0)
            {
                if(zpc.myType== ZombiePartContainer.partType.leg) SetState(State.crawl);
                if(zpc.myType== ZombiePartContainer.partType.body) SetState(State.die);
                zpc.DeactivatePart();
            }
        }
    }


    void Update()
    {
        if (myState == State.move)
        {
            float dist = (heroT.position - this.transform.position).magnitude;
            if (dist < 3)
            {
                SetState(State.scream);
                myNavMesh.enabled = false;
            }
            else
                myNavMesh.SetDestination(heroT.position);
        }
        else if(myState == State.crawl)
        {
            float dist = (heroT.position - this.transform.position).magnitude;
            if (dist < 2f)
            {
               if(myNavMesh.enabled) myNavMesh.enabled = false;
            }
            else
            {
                if(!myNavMesh.enabled) myNavMesh.enabled = true;
                myNavMesh.SetDestination(heroT.position);
            }
        }
    }
}

[System.Serializable]
public class ZombiePartContainer
{
    [Space]
    [Header("----------------------------------------------------")]
    public string name;
    public GameObject[] skinChunk;
    public Rigidbody ragdollChunk;
    public Collider colChunk;
    public Transform boneChunk;
    public zombiePartCol scriptChunk;
    public float partHealth;
    public ZombiePartContainer[] zombieParts;
    public enum partType
    {
        body,
        head,
        leg,
        arm,
    }

    public partType myType;

    public void Init(zombie _zombie)
    {
        scriptChunk.Init(_zombie);
        for (int i = 0; i < zombieParts.Length; i++)
        {
            zombieParts[i].Init(_zombie);
        }
    }

    public ZombiePartContainer GetPartToHit(Collider _col)
    {
        if (_col == colChunk)
        {
            return this;
        }
        else
        {
            ZombiePartContainer zpc = new ZombiePartContainer();
            zpc.name = "EMPTY";
            for (int i = 0; i < zombieParts.Length; i++)
            {
                zpc = zombieParts[i].GetPartToHit(_col);
                if (zpc.name!="EMPTY")
                {
                    break;
                }
            }

          
            return zpc;
        }
    }

    public void DeactivatePart()
    {
        Debug.Log("DEACTIVATE CHUNK "+name);
        for (int i = 0; i < skinChunk.Length; i++)
        {
            skinChunk[i].SetActive(false);
        }

        for (int j = 0; j < zombieParts.Length; j++)
        {
            zombieParts[j].DeactivatePart();
        }
    }
}
