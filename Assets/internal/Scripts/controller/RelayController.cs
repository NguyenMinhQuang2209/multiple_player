using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayController : MonoBehaviour
{
    public static RelayController instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public async Task<string> CreateLobby(int slot)
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(slot);

            string getJoinedCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData serverData = new(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);

            NetworkManager.Singleton.StartHost();

            return getJoinedCode;
        }
        catch (RelayServiceException e)
        {
            LogController.instance.Log(gameObject, e.Message);
            return null;
        }
    }

    public async void JoinLobby(string lobbyCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(lobbyCode);

            RelayServerData serverData = new(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            LogController.instance.Log(gameObject, e.Message);
        }
    }
}
