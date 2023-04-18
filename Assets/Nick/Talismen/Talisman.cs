using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talisman : MonoBehaviour
{
    [SerializeField] public Transform spawnPointHolder;

    [HideInInspector]
    public List<Transform> spawnPoints;

    private bool triggered;

    private void Start()
    {
        foreach (Transform trans in spawnPointHolder.GetComponentsInChildren<Transform>())
        {
            if (trans.localPosition == Vector3.zero) continue;

            spawnPoints.Add(trans);
            Debug.Log(trans.localPosition);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (triggered) return;
        triggered = true;

        NickPlayerController pc = col.gameObject.GetComponent<NickPlayerController>();

        if (pc != null && EventManager.Instance.talismanObtained != null)
        {
            EventManager.Instance.talismanObtained.Invoke(this);
        }
    }
    private void OnTriggerStay(Collider col)
    {

    }
}
