using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue option", menuName = "Dialogue/option", order = 2)]
public class DialogueOptionSO : ScriptableObject
{
    public string text;
    public DialogueSO[] nextDialogues;
    public bool callOnGO;
    public string nextEvent;
    public void OnSelect(GameObject caller)
    {
        if (nextDialogues != null)
            DialogueEngine.instance.StartDisplayTextInTime(nextDialogues, caller);
        if (nextEvent != null)
        {
            if (callOnGO)
            {
                Component _Event = caller.GetComponent(nextEvent);
                if (_Event != null)
                {
                    if (_Event is Event)
                    {
                        ((Event)_Event).Call();
                    }
                    else
                    {
                        Debug.LogWarning("Component named: " + nextEvent + " was found on the gameobject but, is not of the Type event. Please check your spelling.");
                    }
                }
                else
                {
                    Debug.LogWarning("Event named: " + nextEvent + " was not found on the gameobject. Please check your spelling.");
                }
            }else
            EventEngine.instance.Call(nextEvent);
        }
    }
}
