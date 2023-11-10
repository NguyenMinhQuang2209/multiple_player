using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public static CursorController instance;

    List<GameObject> currentObject = null;
    string currentState = string.Empty;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public void ChangeCursor(string newState, List<GameObject> objectReference = null)
    {
        LogController.instance.Log(" " + (newState != currentState).ToString());
        LogController.instance.Log(" " + objectReference.Count + "");
        if (newState != currentState)
        {
            currentState = newState;
            if (currentObject != null)
            {
                foreach (GameObject obj in currentObject)
                {
                    obj.SetActive(false);
                }
            }
            if (objectReference != null)
            {
                foreach (GameObject obj in objectReference)
                {
                    obj.SetActive(true);
                }
            }
            currentObject = objectReference;
        }
        else
        {
            currentState = string.Empty;
            if (currentObject != null)
            {
                foreach (GameObject obj in currentObject)
                {
                    obj.SetActive(false);
                }
            }
            currentObject = null;
        }
        Cursor.lockState = currentObject == null ? CursorLockMode.Locked : CursorLockMode.None;
    }
    public bool CursorFree()
    {
        return currentObject == null;
    }
}
