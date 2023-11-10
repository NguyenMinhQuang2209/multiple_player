using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class PlayerBuilding : NetworkBehaviour
{
    [SerializeField] private BuildingObject testObject;

    private BuildingObject currentBuildingObject = null;
    Transform currentPreview;
    Vector3 currentPos;
    Vector3 currentRot;
    private Camera mainCamera;
    [SerializeField] private float rayCastDistance = 10f;

    public struct BuildingObjectData : INetworkSerializable
    {
        public ulong id;
        public ItemName itemName;
        public ItemType itemType;
        public Vector3 pos;
        public Quaternion rot;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref id);
            serializer.SerializeValue(ref itemName);
            serializer.SerializeValue(ref itemType);
            serializer.SerializeValue(ref pos);
            serializer.SerializeValue(ref rot);
        }
    }

    private void Start()
    {
        mainCamera = GetComponent<PlayerMovement>().GetMainCamera();
    }
    private void Update()
    {
        if (!IsOwner) return;

        if (currentPreview != null)
        {
            Show();
            if (Input.GetKeyDown(KeyCode.B))
            {
                SpawnObject();
            }
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeBuildingObject(testObject);
        }
    }
    public void Show()
    {
        Vector2 centerPoint = new(Screen.width / 2f, Screen.height / 2f);
        Ray ray = mainCamera.ScreenPointToRay(centerPoint);
        if (Physics.Raycast(ray, out RaycastHit hit, rayCastDistance))
        {
            Vector3 point = hit.point;
            currentPos = point;
            currentPreview.position = currentPos;
        }
    }
    public void SpawnObject()
    {
        if (currentBuildingObject != null)
        {
            BuildingObjectData buildingObjectData = new()
            {
                id = OwnerClientId,
                itemName = currentBuildingObject.itemName,
                itemType = currentBuildingObject.itemType,
                pos = currentPos,
                rot = Quaternion.Euler(currentRot)
            };
            SpawnGameObjectServerRpc(buildingObjectData);
            ChangeBuildingObject(null);
        }
    }
    public void ChangeBuildingObject(BuildingObject newObject)
    {
        currentPreview = null;
        currentBuildingObject = newObject;
        if (currentBuildingObject != null)
        {
            GameObject previewObject = Instantiate(currentBuildingObject.previewObject, currentPos, Quaternion.Euler(currentRot));
            currentPreview = previewObject.transform;
        }
    }

    // RPC

    [ServerRpc]
    public void SpawnGameObjectServerRpc(BuildingObjectData obj)
    {
        GameObject item = ObjectReferenceController.instance.GetGameObject(obj.itemType, obj.itemName);
        if (item != null)
        {
            GameObject worldItem = Instantiate(item, obj.pos, obj.rot);
            worldItem.GetComponent<NetworkObject>().Spawn(true);
        }
    }

}
[System.Serializable]
public class BuildingObject
{
    public GameObject previewObject;
    public ItemName itemName;
    public ItemType itemType;
}