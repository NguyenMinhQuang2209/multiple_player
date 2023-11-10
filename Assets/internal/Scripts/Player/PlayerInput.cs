using Unity.Netcode;
using UnityEngine;

public class PlayerInput : NetworkBehaviour
{
    private PlayerInputActions actions;
    private PlayerInputActions.OnFootActions onFoot;

    public override void OnNetworkSpawn()
    {
        actions = new PlayerInputActions();
        onFoot = actions.OnFoot;
        actions?.Enable();
    }
    private void Update()
    {
        if (!IsOwner) return;

        if (onFoot.Inventory.triggered)
        {
            GameObject inventory = PreferenceController.instance.inventoryUI;
            GameObject equipment = PreferenceController.instance.equipmentContainer;
            CursorController.instance.ChangeCursor("inventory", new() { inventory, equipment });
        }
    }
    public PlayerInputActions.OnFootActions GetOnFoot()
    {
        return onFoot;
    }
    private void OnDisable()
    {
        actions?.Disable();
    }
}
