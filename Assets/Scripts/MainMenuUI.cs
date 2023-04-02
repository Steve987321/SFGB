using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button CreateGameBtn, JoinGameBtn, LeaveGameBtn;
    void Awake()
    {
        CreateGameBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.StartHost();
            GameManager.Instance.LoadScene(GameManager.Scene.Scene0);
        });
        JoinGameBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });

        LeaveGameBtn.onClick.AddListener(Application.Quit);
    }
}
