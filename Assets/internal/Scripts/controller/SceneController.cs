using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : NetworkBehaviour
{
    public static SceneController instance;

    public enum SceneName
    {
        Lobby,
        Map_1
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
    public void ChangeSceneNetwork(SceneName name, bool single = true)
    {
        if (single)
        {
            NetworkManager.SceneManager.LoadScene(name.ToString(), LoadSceneMode.Single);
        }
        else
        {
            NetworkManager.SceneManager.LoadScene(name.ToString(), LoadSceneMode.Additive);
        }
    }

    public void ChangeScene(SceneName name, bool single = true)
    {
        if (single)
        {
            SceneManager.LoadScene(name.ToString(), LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene(name.ToString(), LoadSceneMode.Additive);
        }
    }
    public struct SceneObject : INetworkSerializable
    {
        public SceneName sceneName;
        public bool single;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref sceneName);
            serializer.SerializeValue(ref single);
        }
    }

    [ServerRpc]
    public void StartSceneServerRpc(SceneObject sceneObject)
    {
        if (sceneObject.single)
        {
            NetworkManager.SceneManager.LoadScene(sceneObject.sceneName.ToString(), LoadSceneMode.Single);
        }
        else
        {
            NetworkManager.SceneManager.LoadScene(sceneObject.sceneName.ToString(), LoadSceneMode.Additive);
        }
    }

}
