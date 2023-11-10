using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InsideLobbyItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hostRoleTxt;
    [SerializeField] private TextMeshProUGUI userNameTxt;
    [SerializeField] private Button kickBtn;
    [SerializeField] private GameObject readyCheck;
    private string playerId = null;
    private void Start()
    {
        kickBtn.onClick.AddListener(() =>
        {
            LobbyController.instance.KickedLobby(playerId);
        });
    }
    public void Initialised(string playerId, string hostRole, string userName, bool kickBtn, bool ready)
    {
        this.playerId = playerId;
        hostRoleTxt.text = hostRole;
        userNameTxt.text = userName;
        this.kickBtn.gameObject.SetActive(kickBtn);
        readyCheck.gameObject.SetActive(ready);
    }
}
