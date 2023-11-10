using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogController : MonoBehaviour
{
    public static LogController instance;
    [SerializeField] private TextMeshProUGUI logTxt;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void Log(GameObject parent, string logMessage)
    {
        Debug.Log(logMessage, parent);
        logTxt.text += logMessage;
    }
    public void Log(string logMessage)
    {
        logTxt.text += logMessage;
    }
}
