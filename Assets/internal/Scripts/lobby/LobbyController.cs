using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    public const string KEY_PLAYER_NAME = "PlayerName";
    public const string KEY_PLAYER_READY = "PlayerReady";
    public const string KEY_LOBBY_START = "LobbyStart";
    public static LobbyController instance;
    private string playerName = null;

    [Header("UI")]
    [SerializeField] private CreateLobbyContain createLobbyUI;

    private Lobby joinedLobby = null;

    public event EventHandler<LobbyListEventArgs> OnJoinedSystem;

    public event EventHandler<LobbyEventArgs> OnJoinLobby;
    public event EventHandler<LobbyEventArgs> OnKickedLobby;

    float heartLobbyTimer = 0f;
    float loadLobbyDetail = 0f;

    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }
    public class LobbyListEventArgs : EventArgs
    {
        public List<Lobby> lobbies;
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        ReloadLobbyTime();
        LobbyPolling();
    }
    private async void LobbyPolling()
    {
        if (joinedLobby != null)
        {
            try
            {
                loadLobbyDetail -= Time.deltaTime;
                if (loadLobbyDetail <= 0f)
                {
                    loadLobbyDetail = 1.1f;
                    joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                    OnJoinLobby?.Invoke(this, new LobbyEventArgs()
                    {
                        lobby = joinedLobby
                    });
                }

                if (!PlayerInLobby())
                {
                    OnKickedLobby?.Invoke(this, new LobbyEventArgs()
                    {
                        lobby = null
                    });
                    joinedLobby = null;
                }

                if (joinedLobby.Data[KEY_LOBBY_START].Value != "0")
                {
                    RelayController.instance.JoinLobby(joinedLobby.Data[KEY_LOBBY_START].Value);
                    joinedLobby = null;
                    SceneController.SceneObject sceneObject = new()
                    {
                        sceneName = SceneController.SceneName.Map_1,
                        single = true
                    };
                    SceneController.instance.StartSceneServerRpc(sceneObject);
                }
            }
            catch (LobbyServiceException e)
            {
                LogController.instance.Log(gameObject, e.Message);
            }
        }
    }
    private bool PlayerInLobby()
    {
        if (joinedLobby != null)
        {
            foreach (Player player in joinedLobby.Players)
            {
                if (player.Id == GetPlayerID())
                {
                    return true;
                }
            }
        }
        return false;
    }
    public async void AccessToSystem(string playerName)
    {
        try
        {
            this.playerName = playerName;
            InitializationOptions options = new();
            options.SetProfile(this.playerName);
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            GetLobbyList(string.Empty);
        }
        catch (Exception e)
        {
            LogController.instance.Log(gameObject, e.Message);
        }
    }
    public string GetPlayerID()
    {
        return AuthenticationService.Instance.PlayerId;
    }
    public bool IsOwnHost()
    {
        if (joinedLobby != null)
        {
            if (joinedLobby.HostId == GetPlayerID())
            {
                return true;
            }
        }
        return false;
    }
    public Lobby GetCurrentLobby()
    {
        return joinedLobby;
    }
    public void OpenCreateLobbyUI(bool status)
    {
        createLobbyUI.gameObject.SetActive(status);
    }
    public async void GetLobbyList(string search)
    {
        try
        {
            QueryLobbiesOptions options = new();
            options.Filters = new()
            {
                new QueryFilter(
                    field:QueryFilter.FieldOptions.Name,
                    op:QueryFilter.OpOptions.CONTAINS,
                    value:search)
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(options);
            List<Lobby> list = new();
            foreach (Lobby lobby in queryResponse.Results)
            {
                list.Add(lobby);
            }
            OnJoinedSystem?.Invoke(this, new LobbyListEventArgs()
            {
                lobbies = list
            });
        }
        catch (LobbyServiceException e)
        {
            LogController.instance.Log(gameObject, e.Message);
        }
    }
    public async void CreateNewLobby(string lobbyName, bool isPrivate, int slot)
    {
        try
        {
            CreateLobbyOptions options = new()
            {
                IsPrivate = isPrivate,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    { KEY_LOBBY_START,new DataObject(DataObject.VisibilityOptions.Member,"0")}
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, slot, options);
            joinedLobby = lobby;
            OnJoinLobby?.Invoke(this, new LobbyEventArgs()
            {
                lobby = lobby
            });
        }
        catch (LobbyServiceException e)
        {
            LogController.instance.Log(gameObject, e.Message);
        }
    }
    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new()
            {
                Player = GetPlayer(),
            });
            joinedLobby = lobby;
            OnJoinLobby?.Invoke(this, new LobbyEventArgs()
            {
                lobby = lobby
            });
        }
        catch (LobbyServiceException e)
        {
            LogController.instance.Log(gameObject, e.Message);
        }
    }
    public async void ReloadLobbyTime()
    {
        if (joinedLobby != null && IsOwnHost())
        {
            try
            {
                heartLobbyTimer -= Time.deltaTime;
                if (heartLobbyTimer <= 0f)
                {
                    heartLobbyTimer = 15f;
                    await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
                }
            }
            catch (LobbyServiceException e)
            {
                LogController.instance.Log(gameObject, e.Message);
            }
        }
    }
    private Player GetPlayer()
    {
        return new Player(GetPlayerID(), null, new Dictionary<string, PlayerDataObject>
        {
            {KEY_PLAYER_NAME,new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public,playerName) },
            {KEY_PLAYER_READY,new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,"0") }
        });
    }
    public async void JoinLobbyByID(string lobbyId)
    {
        try
        {

            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, new()
            {
                Player = GetPlayer()
            });
            joinedLobby = lobby;
            OnJoinLobby?.Invoke(this, new LobbyEventArgs()
            {
                lobby = lobby
            });
        }
        catch (LobbyServiceException e)
        {
            LogController.instance.Log(gameObject, e.Message);
        }
    }
    public async void KickedLobby(string playerId)
    {
        if (IsOwnHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                LogController.instance.Log(gameObject, e.Message);
            }
        }
    }
    public async void LeaveLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, GetPlayerID());
                joinedLobby = null;
                OnKickedLobby?.Invoke(this, new LobbyEventArgs()
                {
                    lobby = null
                });
            }
            catch (LobbyServiceException e)
            {
                LogController.instance.Log(gameObject, e.Message);
            }
        }
    }
    public async void ChangePlayerReadyStatus()
    {
        if (joinedLobby != null)
        {
            try
            {
                string status = "0";
                foreach (Player player in joinedLobby.Players)
                {
                    if (player.Id == GetPlayerID())
                    {
                        status = player.Data[KEY_PLAYER_READY].Value == "1" ? "0" : "1";
                        break;
                    }
                }
                await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, GetPlayerID(), new()
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {KEY_PLAYER_READY,new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,status) }
                    }
                });
            }
            catch (LobbyServiceException e)
            {
                LogController.instance.Log(gameObject, e.Message);
            }
        }
    }
    public string GetPlayerName()
    {
        return playerName;
    }
    public async void StartGame()
    {
        if (joinedLobby != null && IsOwnHost())
        {
            try
            {
                string lobbyCode = await RelayController.instance.CreateLobby(5);

                await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new()
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {KEY_LOBBY_START,new DataObject(DataObject.VisibilityOptions.Member,lobbyCode) }
                    }
                });
                joinedLobby = null;
                SceneController.instance.ChangeSceneNetwork(SceneController.SceneName.Map_1);
            }
            catch (LobbyServiceException e)
            {
                LogController.instance.Log(gameObject, e.Message);
            }
        }
    }
}
