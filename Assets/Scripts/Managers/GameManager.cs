using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // how many players alive in current scene
    private int _playersAlive;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;
    }

    void LateUpdate()
    {
        // last man standing 
        if (_playersAlive == 1)
        {

        }

        if (Input.GetKeyDown(KeyCode.Insert))
        {
            Cursor.visible = !Cursor.visible;
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(50, 300, 100, 50), "Start Client"))
            NetworkManager.Singleton.StartClient();
        if (GUI.Button(new Rect(50, 360, 100, 50), "Start Host"))
            NetworkManager.Singleton.StartHost();
    }

}
