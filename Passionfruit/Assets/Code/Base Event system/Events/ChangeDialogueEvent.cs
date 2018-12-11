using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDialogueEvent : Event {

    public GameObject targetGO;
    public DialogueSO[] dialogueSOs;
    public override void Call()
    {
        base.Call();
        DisplayTextEvent target = targetGO.GetComponent<DisplayTextEvent>();
        if (target != null)
            target.dialogue = dialogueSOs;
        else
            Debug.LogWarning(targetGO.name + " does not have a display text event component");
        
    }
}
