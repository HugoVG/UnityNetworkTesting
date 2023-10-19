using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class WhenAll : Signal<bool> 
{
    // Serializable field for gameobject that use ISignal<bool>
    
    [SerializeField] private List<Signal<bool>> signals = new List<Signal<bool>>();
    
    public NetworkVariable<bool> isActivated = new NetworkVariable<bool>(false);
    
    public bool StayOnAfterTrigger = true;
    private bool isTriggered = false;

    [SerializeField] private UnityEvent OnActive;
    [SerializeField] private UnityEvent OnUnActive;

    private void Start()
    {
        if(OnUnActive != null)
            OnUnActive.Invoke();
        isActivated.OnValueChanged += (value, newValue) =>
        {
            if (newValue)
            {
                if (isTriggered && StayOnAfterTrigger) return;
                OnActive?.Invoke();
                isTriggered = true;
            }
            else
            {
                if (isTriggered && StayOnAfterTrigger) return;
                OnUnActive?.Invoke();
            }
        };
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isActivated.Value = signals.TrueForAll(signal => signal.Value.Value);
    }

    public override NetworkVariable<bool> Value => isActivated;
}