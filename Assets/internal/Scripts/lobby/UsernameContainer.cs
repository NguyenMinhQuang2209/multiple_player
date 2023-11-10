using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UsernameContainer : MonoBehaviour
{
    [SerializeField] private TMP_InputField userNameInput;
    [SerializeField] private Button registerBtn;
    private void Start()
    {
        registerBtn.onClick.AddListener(() =>
        {
            if (userNameInput.text == string.Empty)
            {
                return;
            }
            LobbyController.instance.AccessToSystem(userNameInput.text);
            Hide();
        });
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
