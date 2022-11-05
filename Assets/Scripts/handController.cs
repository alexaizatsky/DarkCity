using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handController : MonoBehaviour
{
    [SerializeField] private Animator handAnim;
    [SerializeField] private Camera myCam;
    [SerializeField] private GameObject handObj;
    [SerializeField] private GameObject shootFxPrefab;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform gunPoint;
    [SerializeField] private int bulletPoolCount = 30;
    [SerializeField] private int fxPoolCount = 10;
    public enum State
    {
        idle,
        walk,
        run,
        shoot,
        reload,
        aimIdle,
        aimShoot,
        aimWalk,
    }

    public State myState;
    private bool isRun;
    private bool isShoot;
    private bool isAiming;

    private GameObject[] bulletPool;
    private GameObject[] fxPool;

    private gunPoolSystem poolSystem;
    public void SetState(State s)
    {
        myState = s;
        if(s == State.idle)
            handAnim.SetInteger("animInt", 0);
        else if(s == State.walk)
            handAnim.SetInteger("animInt", 1);
        else if(s == State.run)
            handAnim.SetInteger("animInt", 2);
        else if(s == State.shoot)
            handAnim.SetInteger("animInt", 3);
        else if(s == State.aimIdle)
            handAnim.SetInteger("animInt", 5);
        else if(s == State.aimShoot)
            handAnim.SetInteger("animInt", 6);
        else if(s == State.aimWalk)
            handAnim.SetInteger("animInt", 7);
    }
    
    void Start()
    {
        if (poolSystem == null)
            poolSystem = this.gameObject.AddComponent<gunPoolSystem>();
        poolSystem.GeneratePool(ref bulletPool, bulletPoolCount, gunPoint, bulletPrefab);
        poolSystem.GeneratePool(ref fxPool, fxPoolCount, gunPoint, shootFxPrefab);
    }

    void Update()
    {
        GetInput();
    }


    void ActivateAim(bool _active)
    {
        isAiming = _active;

            StartCoroutine(AimMove(_active, 0.3f));
        
    }

    void PLayShootAudio()
    {
        // PLAY AUDIO HERE
    }

    public void MakeShoot()
    {
        if (myState == State.shoot || myState == State.aimShoot)
        {


            int bullIndex = poolSystem.GetPoolElementIndex(ref bulletPool);
            //print("MAKE SHOOT bull index "+bullIndex+" "+System.DateTime.UtcNow.ToString("HH:mm:ss.fff "));
            if (bullIndex < 0)
                Debug.LogWarning("NO FREE BULLET IN POOL");
            else
            {
                bulletPool[bullIndex].transform.SetParent(gunPoint);
                bulletPool[bullIndex].transform.localPosition = Vector3.zero;
                bulletPool[bullIndex].transform.localEulerAngles = Vector3.zero;
                bulletPool[bullIndex].transform.SetParent(null);
                bulletPool[bullIndex].SetActive(true);
                bulletPool[bullIndex].GetComponent<bullet>().StartShoot(gunPoint);
                StartCoroutine(ObjectActivation(bulletPool[bullIndex], 4, true));
            }

            int fxIndex = poolSystem.GetPoolElementIndex(ref fxPool);
            if (fxIndex < 0 )
                Debug.LogWarning("NO FREE FX IN POOL");
            else
            {
                StartCoroutine(ObjectActivation(fxPool[fxIndex], 1.5f));
            }
            PLayShootAudio();
        }
    }
    
    void GetInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ActivateAim(true);
        }


        if (Input.GetMouseButtonUp(1))
        {
            ActivateAim(false);
            if (myState == State.aimShoot) 
                SetState(State.shoot);
        }
        
        
        float hor = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.LeftShift))
            isRun = true;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            isRun = false;
        
        if (hor != 0 || vert != 0)
        {
            if (isAiming)
            {
                if (!isShoot)
                {
                    if(myState!=State.aimWalk)
                        SetState(State.aimWalk);
                }
                
            }
            else
            {
                
                if (!isShoot)
                {
                    if (isRun)
                    {
                        if (myState != State.run)
                            SetState(State.run);
                    }
                    else
                    {
                        if (myState != State.walk)
                            SetState(State.walk);
                    }
                }
            }
        }

        if (vert == 0 && hor == 0)
        {
            if (isAiming)
            {
                if (!isShoot)
                {
                    if(myState!=State.aimIdle)
                        SetState(State.aimIdle);
                }
            }
            else
            {
                if (!isShoot)
                {
                    if (myState != State.idle)
                    {
                        SetState(State.idle);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            isShoot = true;
            if (isAiming)
            {
                if(myState!= State.aimShoot)
                    SetState(State.aimShoot);
            }
            else
            {
                if(myState!= State.shoot)
                    SetState(State.shoot);
            }
           
        }

        if (Input.GetMouseButton(0))
        {
            isShoot = true;
            if (isAiming)
            {
                if(myState!= State.aimShoot)
                    SetState(State.aimShoot);
            }
            else
            {
                if(myState!= State.shoot)
                    SetState(State.shoot);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isShoot = false;
           
        }

       
           
        
        
    }

    IEnumerator AimMove(bool _active, float _time)
    {
        float timer = 0;
        Vector3 closePos = new Vector3(0,-1.68f, -0.11f);
        Vector3 farPos = new Vector3(0,-1.78f, 0.1f);
        float closeFov = 40;
        float farFov = 60;
        
        Vector3 sp = Vector3.zero;
        Vector3 ep = Vector3.zero;
        float sf = 0;
        float ef = 0;
        if (_active)
        {
            sp = farPos;
            ep = closePos;
            sf = farFov;
            ef = closeFov;
        }
        else
        {
            sp = closePos;
            ep = farPos;
            sf = closeFov;
            ef = farFov;
        }
        
        while (timer<_time)
        {
            timer += Time.deltaTime;
            float prog = Mathf.InverseLerp(0, _time, timer);
            myCam.fieldOfView = Mathf.Lerp(sf, ef, prog);
            handObj.transform.localPosition = Vector3.Lerp(sp, ep, prog);
            yield return null;
        }
    }

    IEnumerator ObjectActivation(GameObject _eff, float _time, bool _bullet = false)
    {
        _eff.SetActive(true);
        yield return new WaitForSeconds(_time);
        if(_bullet)
            _eff.GetComponent<bullet>().StopShoot();
        _eff.transform.SetParent(gunPoint);
        _eff.transform.localPosition = Vector3.zero;
        _eff.transform.localEulerAngles = Vector3.zero;
        _eff.SetActive(false);
    }
}
