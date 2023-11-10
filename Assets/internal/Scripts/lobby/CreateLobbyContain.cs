using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyContain : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyName;
    [SerializeField] private Button lobbyMode;
    [SerializeField] private TextMeshProUGUI lobbyModeTxt;
    [SerializeField] private Button lobbySlot;
    [SerializeField] private TextMeshProUGUI lobbySlotTxt;
    [SerializeField] private Button createLobby;
    [SerializeField] private Button cancelLobby;

    private string mode = "public";
    [SerializeField]
    private List<int> slotOption = new()
    {
       5
    };
    private int option = 5;

    private void Start()
    {
        option = slotOption[0];
        lobbyModeTxt.text = mode;
        lobbySlotTxt.text = option.ToString();
        lobbyMode.onClick.AddListener(() =>
        {
            mode = mode == "public" ? "private" : "public";
            lobbyModeTxt.text = mode;
        });
        createLobby.onClick.AddListener(() =>
        {
            if (lobbyName.text == string.Empty)
            {
                return;
            }
            LobbyController.instance.CreateNewLobby(lobbyName.text, mode != "public", option);
            Hide();
        });
        cancelLobby.onClick.AddListener(() =>
        {
            LobbyController.instance.OpenCreateLobbyUI(false);
        });
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}

