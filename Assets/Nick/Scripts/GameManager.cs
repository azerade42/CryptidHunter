using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CoreAI cryptid;

    // not sure why OnEnable doesn't work here, EventManager.Instance is null, something to do with order of execution
    private void Start()
    {
        EventManager.Instance.talismanObtained += SpawnCryptid;
    }

    private void OnDisable()
    {
       EventManager.Instance.talismanObtained -= SpawnCryptid; 
    }

    private void SpawnCryptid(Talisman talisman)
    {
        cryptid.gameObject.SetActive(true);
        cryptid.Spawn(talisman);
    }
}
