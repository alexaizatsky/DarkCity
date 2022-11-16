using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunLevelSystem : MonoBehaviour
{
    public static gunLevelSystem Instance;

    [SerializeField] private bool lineDebug;
    [SerializeField] private GameObject lineDebugPrefab;
    [SerializeField] private GameObject bulletDecalPrefab;
    [SerializeField] private int bulletDecalPoolCount;
     private LineRenderer[] debugLines;
    private GameObject[] decalPool;
    private LayerMask lm;
    
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        GenerateDecalPool();
        if (lineDebug)
        {
            debugLines = new LineRenderer[3];
            for (int i = 0; i < debugLines.Length; i++)
            {
                GameObject c = Instantiate(lineDebugPrefab);
                c.transform.SetParent(this.transform);
                debugLines[i] = c.GetComponent<LineRenderer>();
                debugLines[i].positionCount = 0;
            }
        }
    }

    void GenerateDecalPool()
    {
        decalPool = new GameObject[bulletDecalPoolCount];
        for (int i = 0; i < decalPool.Length; i++)
        {
            GameObject c = Instantiate(bulletDecalPrefab);
            c.transform.SetParent(this.transform);
            c.SetActive(false);
            decalPool[i] = c;
        }
    }

    public static void RegisterBulletCollision(bulletCollisionDataContainer _data)
    {
        if(Instance == null) return;

        Instance.ActivateBulletDecal(_data);
    }

    public void ActivateBulletDecal(bulletCollisionDataContainer _data)
    {
        int n = -1;
        for (int i = 0; i < decalPool.Length; i++)
        {
            if (!decalPool[i].activeSelf)
            {
                n = i;
                break;
            }
        }
        if(n<0)
            Debug.LogWarning("NO FREE DECAL IN POOL");
        else
        {
            Vector3 sendPoint = _data.heroPoint.position;
            Vector3 dir1 = _data.heroPoint.forward;
            Vector3 dir2 = _data.heroPoint.forward - _data.heroPoint.right * 0.1f;
            Vector3 dir3 = _data.heroPoint.forward + _data.heroPoint.up * 0.1f;
            //Vector3 sendPoint = _data.bulletPoint + (_data.heroPoint - _data.bulletPoint).normalized;
            //Vector3 dir1 = (_data.bulletPoint - sendPoint).normalized;
            //Vector3 dir2 = dir1+new Vector3(0.1f,0,0);
            //print("DECAL bullet "+_data.bulletPoint+" hero "+_data.heroPoint+" layer "+_data.colLayer+" "+System.DateTime.UtcNow.ToString("HH:mm:ss.fff "));
            //print("DECAL sendPoint "+sendPoint+" dir1 "+dir1+" dir2r "+dir2+" "+System.DateTime.UtcNow.ToString("HH:mm:ss.fff "));
            Vector3 decalPoint = new Vector3();
            Vector3 rightPoint = new Vector3();
            Vector3 upPoint = new Vector3();

            if (lineDebug)
            {
                debugLines[0].positionCount = 2;
                debugLines[0].SetPosition(0, sendPoint);
                debugLines[0].SetPosition(1, sendPoint+dir1*10);
                debugLines[1].positionCount = 2;
                debugLines[1].SetPosition(0, sendPoint);
                debugLines[1].SetPosition(1, sendPoint+dir2*10);
                debugLines[2].positionCount = 2;
                debugLines[2].SetPosition(0, sendPoint);
                debugLines[2].SetPosition(1, sendPoint+dir3*10);
            }


            
            RaycastHit hit;
            lm = new LayerMask();
            lm +=  (1 << _data.colLayer);
            int check = 0;
            if (Physics.Raycast(sendPoint, dir1, out hit, 100, lm))
            {
                //print(hit.collider.gameObject.name+" CHECK 1 "+hit.collider.gameObject.layer+" "+System.DateTime.UtcNow.ToString("HH:mm:ss.fff "));
                decalPoint = hit.point;
                check++;
            }
            else
            {
                if (Physics.Raycast(sendPoint, dir1, out hit, 100))
                {
                    //print(hit.collider.gameObject.name+" FAIL CHECK 1 "+hit.collider.gameObject.layer+" "+System.DateTime.UtcNow.ToString("HH:mm:ss.fff "));
                }
                else
                {
                   // print("FAIL CHECK 1 AT ALL");
                }
                
            }
            if (Physics.Raycast(sendPoint, dir2, out hit, 100,lm))
            {
               // print(hit.collider.gameObject.name+" CHECK 2 "+hit.collider.gameObject.layer+" "+System.DateTime.UtcNow.ToString("HH:mm:ss.fff "));
                rightPoint = hit.point;
                check++;
            }
            else
            {
                if (Physics.Raycast(sendPoint, dir2, out hit, 100))
                {
                    //print(hit.collider.gameObject.name+" FAIL CHECK 2 "+hit.collider.gameObject.layer+" "+System.DateTime.UtcNow.ToString("HH:mm:ss.fff "));
                }
                else
                {
                    //print("FAIL CHECK 2 AT ALL");
                }
            }
            if (Physics.Raycast(sendPoint, dir3, out hit, 100,lm))
            {
                //print(hit.collider.gameObject.name+" CHECK 3 "+hit.collider.gameObject.layer+" "+System.DateTime.UtcNow.ToString("HH:mm:ss.fff "));
                upPoint = hit.point;
                check++;
            }
            else
            {
                if (Physics.Raycast(sendPoint, dir3, out hit, 100))
                {
                    //print(hit.collider.gameObject.name+" FAIL CHECK 3 "+hit.collider.gameObject.layer+" "+System.DateTime.UtcNow.ToString("HH:mm:ss.fff "));
                }
                else
                {
                    //print("FAIL CHECK 3 AT ALL");
                }
            }
            if (check >= 3)
            {
                //print("YES ckeck Bullet decal"+" "+System.DateTime.UtcNow.ToString("HH:mm:ss.fff "));
                decalPool[n].SetActive(true);
                decalPool[n].transform.position = decalPoint;
                Vector3 dirR = (rightPoint - decalPoint).normalized;
                Vector3 dirU = (upPoint - decalPoint).normalized;
                decalPool[n].transform.forward = Vector3.Cross(dirR, dirU);
                //decalPool[n].transform.right = dir;
                //decalPool[n].transform.eulerAngles+=new Vector3(0,180,0);
                StartCoroutine(DelayDeactivateObj(decalPool[n], 3));
            }
            else
            {
                //print("NO ckeck Bullet decal "+check+" "+System.DateTime.UtcNow.ToString("HH:mm:ss.fff "));
            }
           
        }
    }

    IEnumerator DelayDeactivateObj(GameObject _obj, float _delay, bool _scale = false)
    {
        yield return new WaitForSeconds(_delay);
        _obj.SetActive(false);
    }
}

public class bulletCollisionDataContainer
{
    public Vector3 bulletPoint;
    public Transform heroPoint;
    public int colLayer;

    public bulletCollisionDataContainer(Vector3 _bulletPoint, Transform _heroPoint, int _layer)
    {
        bulletPoint = _bulletPoint;
        heroPoint = _heroPoint;
        colLayer = _layer;

    }
}