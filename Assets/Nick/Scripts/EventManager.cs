using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{ 
    public static EventManager Instance;

    public UnityAction fireAction;
    public UnityAction aimAction;
    public UnityAction damageAction;
    public UnityAction equipRightAction;
    public UnityAction rifleHit;
    public UnityAction<Talisman> talismanObtained;
    public UnityAction enemyDie;
    public UnityAction talismanUsed;

    public UnityAction nearPlayer;
    public UnityAction leavePlayer;
    public UnityAction<bool> fadeToBlack;

    public UnityAction<ItemPickUp> inItem;
    public UnityAction<ItemPickUp> outItem;

    public UnityAction toggleInventory;

    public UnityAction startTalisman;

    public UnityAction crosshairTrue;
    public UnityAction crosshairFalse;

    private void Awake()
    {
        if (EventManager.Instance != null)
        {
            Debug.LogWarning("Multiple event managers in the scene. (" + Instance + ")");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}
