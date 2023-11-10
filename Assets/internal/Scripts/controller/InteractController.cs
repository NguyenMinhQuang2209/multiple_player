using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractController : MonoBehaviour
{
    public static InteractController instance;

    [SerializeField] private TextMeshProUGUI interactText;
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
        ChangeTextView(string.Empty);
    }
    public void ChangeTextView(string txt)
    {
        if (interactText != null)
        {
            interactText.text = txt;
        }
    }
}
