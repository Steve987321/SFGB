using System.Collections;
using System.Collections.Generic;
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
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 100), (1 / Time.smoothDeltaTime).ToString());
    }

}
