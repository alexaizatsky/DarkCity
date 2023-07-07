using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zombiePoolSystem : MonoBehaviour
{
    public void GeneratePool(ref GameObject[] _pool, int _count, Transform _parent, GameObject _prefab)
    {
        _pool = new GameObject[_count];
        for (int i = 0; i < _count; i++)
        {
            GameObject c = Instantiate(_prefab);
            c.transform.SetParent(_parent);
            c.transform.localPosition = Vector3.zero;
            c.transform.localEulerAngles = Vector3.zero;
            c.transform.localScale = new Vector3(1,1,1);
            c.gameObject.SetActive(false);
            _pool[i] = c;
        }
    }

    public int GetPoolElementIndex(ref GameObject[] _pool)
    {
        int n = -1;
        for (int i = 0; i < _pool.Length; i++)
        {
            if (!_pool[i].activeSelf)
            {
                n = i;
                break;
            }
        }

        return n;
    }
    public GameObject GetPoolElement(ref GameObject[] _pool)
    {
        int n = -1;
        for (int i = 0; i < _pool.Length; i++)
        {
            if (!_pool[i].activeSelf)
            {
                n = i;
                break;
            }
        }
        print("GET ZOMBIE FROM POOL numb "+n);
        if (n < 0)
            return null;
        else
            return _pool[n];
        
    }
}
