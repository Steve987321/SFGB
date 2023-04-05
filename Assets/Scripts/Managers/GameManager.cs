using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

    public enum Scene : int
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
    public TextMeshProUGUI GameCodeText;

    private GameState _state;

    [SerializeField] private GameObject _playerPrefab;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        print("on network spawn");
        if (IsServer) {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    // only use this
    public async void StartGame()
    {
        //NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCallback;

        await Relay.CreateRelay();
        
        if (!NetworkManager.Singleton.StartHost())
            Debug.LogError("failed to start as host");

        GameManager.Instance.LoadScene(Scene.Scene0);
    }

    public async void JoinGame(string joinCode)
    {
        await Relay.JoinRelay(joinCode);

        NetworkManager.Singleton.StartClient();
    }

    public void LoadScene(Scene scene)
    {
        print("loading scene: " + scene.ToString());
        if (scene != Scene.Scene0)
        {
            GameCodeText.gameObject.SetActive(false);
        }
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
        //foreach (var id in NetworkManager.Singleton.ConnectedClientsIds)
        //{
        //    var player = Instantiate(_playerPrefab);
        //    player.GetComponent<NetworkObject>().SpawnAsPlayerObject(id, true);
        //    print("spawned player prefab");
        //}
        // Destroy existing player prefabs
        var players = FindObjectsOfType<Player>();
        foreach (var player in players)
        {
            if (player.IsLocalPlayer)
            {
                player.GetComponent<NetworkObject>().Despawn(true);
            }
            else
            {
                Destroy(player.gameObject);
            }
        }

        // Instantiate new player prefabs for each connected client
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
        if (!IsHost) return;

        var f = FindObjectsOfType<Player>();

        //if (CurrentScene == Scene.Scene0) {
        //    if (NetworkManager.Singleton.ConnectedClientsIds.Count == 2) {
        //        if (f[0].IsReady.Value && f[1].IsReady.Value) {
        //            LoadScene(GetRandomFightScene());
        //        }
        //    }
        //}
        if (CurrentScene != Scene.Scene0) {
            // last man standing 
            if (f.Any(p => p.Health.Value <= 0)) {
                LoadScene(NextFightScene(CurrentScene));
            }
        }
    }


    void OnGUI()
    {
        if (IsHost)
        {
            if (GUI.Button(new Rect(Screen.width - 150, 50, 150, 40), "Load Random Level"))
                LoadScene(GetRandomFightScene());
            if (GUI.Button(new Rect(Screen.width - 150, 90, 150, 40), "Load Next Level"))
                LoadScene(NextFightScene(CurrentScene));
        }

        //#if UNITY_EDITOR
        //        if (GUI.Button(new Rect(50, 300, 100, 50), "Start Client"))
        //            NetworkManager.Singleton.StartClient();
        //        if (GUI.Button(new Rect(50, 360, 100, 50), "Start Host"))
        //            StartGame();
        //#endif
    }
}
