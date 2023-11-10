using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class InsideLobbyContainer : MonoBehaviour
{
    [SerializeField] private Button backBtn;
    [SerializeField] private Button copyBtn;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI codeTxt;
    [SerializeField] private GameObject container;
    [SerializeField] private Button readyBtn;
    [SerializeField] private TextMeshProUGUI readyTxt;

    [SerializeField] private InsideLobbyItem insideLobbyItem;
    private void Start()
    {
        LobbyController.instance.OnJoinLobby += JoinLobby_Event;
        LobbyController.instance.OnKickedLobby += KickedLobby_Event;
        backBtn.onClick.AddListener(() =>
        {
            LobbyController.instance.LeaveLobby();
        });
        copyBtn.onClick.AddListener(() =>
        {
            GUIUtility.systemCopyBuffer = codeTxt.text;
        });
        readyBtn.onClick.AddListener(() =>
        {
            if (LobbyController.instance.IsOwnHost())
            {
                bool isAllReady = true;
                foreach (Player player in LobbyController.instance.GetCurrentLobby().Players)
                {
                    if (player.Data[LobbyController.KEY_PLAYER_READY].Value == "0" && player.Id != LobbyController.instance.GetCurrentLobby().HostId)
                    {
                        isAllReady = false;
                        break;
                    }
                }
                if (isAllReady)
                {
                    LobbyController.instance.StartGame();
                }
            }
            else
            {
                LobbyController.instance.ChangePlayerReadyStatus();
            }
        });
        Hide();
    }

    private void KickedLobby_Event(object sender, LobbyController.LobbyEventArgs e)
    {
        Hide();
    }

    private void JoinLobby_Event(object sender, LobbyController.LobbyEventArgs e)
    {
        UpdateInsideLobby();
    }
    private void UpdateInsideLobby()
    {
        ClearLobby();
        Lobby lobby = LobbyController.instance.GetCurrentLobby();
        codeTxt.text = lobby.LobbyCode;
        nameTxt.text = lobby.Name;
        string readyStatus = "Sẵn sàng";
        if (LobbyController.instance.IsOwnHost())
        {
            readyStatus = "Bắt đầu";
        }
        foreach (Player player in lobby.Players)
        {
            InsideLobbyItem item = Instantiate(insideLobbyItem, container.transform);
            string playerName = player.Data[LobbyController.KEY_PLAYER_NAME].Value;
            bool kickBtn = LobbyController.instance.IsOwnHost() && player.Id != LobbyController.instance.GetPlayerID();
            bool ready = player.Data[LobbyController.KEY_PLAYER_READY].Value == "1";
            string hostRole = player.Id == lobby.HostId ? "Chủ lobby" : "Khách";
            item.Initialised(player.Id, playerName, hostRole, kickBtn, ready);
            if (player.Id == LobbyController.instance.GetPlayerID() && !LobbyController.instance.IsOwnHost())
            {
                readyStatus = player.Data[LobbyController.KEY_PLAYER_READY].Value == "1" ? "Không sẵn sàng" : "Sẵn sàng";
            }
        }
        readyTxt.text = readyStatus;
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
