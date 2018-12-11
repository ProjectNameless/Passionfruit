using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddReputationEvent : Event {
    public int amount;
    public override void Call()
    {
        base.Call();
        if (DialogueEngine.instance != null)
            DialogueEngine.instance.ChangeReputation(amount);
        else
            Debug.LogWarning("This scene lacks a DialogueEngine. Reputation is currently tied to the DialogueEngine.");
    }

}
