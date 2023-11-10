using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactible : MonoBehaviour
{
    public string promptMessage;
    public bool useEvent;
    public void BaseInteract()
    {
        if (useEvent)
        {
            GetComponent<InteractibleEvent>().OnInteract?.Invoke();
        }
        Interact();
    }
    public virtual void Interact()
    {

    }
}
