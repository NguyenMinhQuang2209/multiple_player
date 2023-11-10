using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hostName;
    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private TextMeshProUGUI slot;
    [SerializeField] private Button joinedBtn;
    string lobbyId = null;
    private void Start()
    {
        joinedBtn.onClick.AddListener(() =>
        {
            LobbyController.instance.JoinLobbyByID(lobbyId);
        });
    }
    public void Initialised(string lobbyId, string playerName, string lobbyName, string slot)
    {
        this.lobbyId = lobbyId;
        hostName.text = playerName;
        this.lobbyName.text = lobbyName;
        this.slot.text = slot;
    }
}
