using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    private enum GameState
    {
        Lobby,
        FightingCountDown,
        Playing,
    }

    public enum Scene
    {
        Scene0, 
        Street,
        ContainerShip,
        HighWay
    }

    // will return next fight scene map
    public static Scene NextFightScene(Scene currScene)
    {
        return currScene switch
        {
            Scene.Scene0 => Scene.Street,
            Scene.Street => Scene.ContainerShip,
            Scene.ContainerShip => Scene.HighWay,
            Scene.HighWay => Scene.Street,
            _ => Scene.ContainerShip
        };
    }

    public Scene CurrentScene = Scene.Scene0;

    private GameState _state;

    [SerializeField] private GameObject _playerPrefab;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer) {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    // only use this
    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest arg1, NetworkManager.ConnectionApprovalResponse arg2)
    {
        if (CurrentScene != Scene.Scene0)
        {
            arg2.Approved = false;
            arg2.Reason = "Already in fight";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= 2)
        {
            arg2.Approved = false;
            arg2.Reason = "max players reached";
            return;
        }

        arg2.Approved = true;
    }

    public void LoadScene(Scene scene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
        CurrentScene = scene;
    }

    public Scene GetRandomFightScene()
    {
        int f = UnityEngine.Random.Range(1, 4);
        switch (f)
        {
            case 1: return Scene.ContainerShip;
            case 2: return Scene.HighWay;
            case 3: return Scene.Street;
        }
        
        return CurrentScene;
    }
    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (var id in NetworkManager.Singleton.ConnectedClientsIds)
        {
            var player = Instantiate(_playerPrefab);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(id, true);
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;
    }

    void LateUpdate()
    {
        if (!IsServer) return;

        var f = FindObjectsOfType<Player>();

        // last man standing 
        if (f.Any(p => p.Health.Value <= 0))
        {
            LoadScene(NextFightScene(CurrentScene));
        }

        if (CurrentScene == Scene.Scene0)
        {
            if (NetworkManager.Singleton.ConnectedClientsIds.Count == 2)
            {
                if (f[0].IsReady.Value && f[1].IsReady.Value)
                {
                    LoadScene(GetRandomFightScene());
                }
            }
        }
    }


    void OnGUI()
    {
        if (GUI.Button(new Rect(50, 300, 100, 50), "Start Client"))
            NetworkManager.Singleton.StartClient();
        if (GUI.Button(new Rect(50, 360, 100, 50), "Start Host"))
            StartHost();
    }

}
