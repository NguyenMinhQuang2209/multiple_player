using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListContainer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userName;
    [SerializeField] private TMP_InputField seachInput;
    [SerializeField] private Button searchBtn;
    [SerializeField] private Button createBtn;
    [SerializeField] private Button reloadBtn;
    [SerializeField] private TMP_InputField codeInput;
    [SerializeField] private Button codeBtn;
    [SerializeField] private GameObject container;


    [SerializeField] private LobbyItem lobbyItem;
    private void Start()
    {
        LobbyController.instance.OnJoinedSystem += OnJoinedSystem_Event;
        LobbyController.instance.OnKickedLobby += OnKickedLobby_Event;
        LobbyController.instance.OnJoinLobby += OnJoinedLobby_Event;
        reloadBtn.onClick.AddListener(() =>
        {
            LobbyController.instance.GetLobbyList(string.Empty);
        });
        searchBtn.onClick.AddListener(() =>
        {
            LobbyController.instance.GetLobbyList(seachInput.text);
        });
        createBtn.onClick.AddListener(() =>
        {
            LobbyController.instance.OpenCreateLobbyUI(true);
        });
        codeBtn.onClick.AddListener(() =>
        {
            LobbyController.instance.JoinLobbyByCode(codeInput.text);
        });
        Hide();
    }

    private void OnKickedLobby_Event(object sender, LobbyController.LobbyEventArgs e)
    {
        LobbyController.instance.GetLobbyList(string.Empty);
    }

    private void OnJoinedSystem_Event(object sender, LobbyController.LobbyListEventArgs e)
    {
        UpdateLobbyList(e.lobbies);
    }

    private void OnJoinedLobby_Event(object sender, LobbyController.LobbyEventArgs e)
    {
        Hide();
    }


    public void UpdateLobbyList(List<Lobby> lobbies)
    {
        ClearLobby();
        userName.text = LobbyController.instance.GetPlayerName();
        foreach (Lobby lob in lobbies)
        {
            LobbyItem item = Instantiate(lobbyItem, container.transform);
            string playerName = null;
            foreach (Player player in lob.Players)
            {
                if (player.Id == LobbyController.instance.GetPlayerID())
                {
                    playerName = player.Data[LobbyController.KEY_PLAYER_NAME].Value;
                    break;
                }
            }
            item.Initialised(lob.Id, playerName, lob.Name, lob.Players.Count + "/" + lob.MaxPlayers);
        }
        Show();
    }
    private void ClearLobby()
    {
        foreach (Transform child in container.transform)
        {
            Destroy(child.gameObject);
        }
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
}
