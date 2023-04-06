using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button CreateGameBtn, JoinGameBtn, LeaveGameBtn;
    [SerializeField] private TextMeshProUGUI joinGameInputField;

    void Awake()
    {
        CreateGameBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.StartGame();
        });
        JoinGameBtn.onClick.AddListener(() =>
        {
            if (joinGameInputField.text.Length -1 != 6)
            {
                Debug.LogError("Code is invalid");
                return;
            }
            GameManager.Instance.JoinGame(joinGameInputField.text);
        });

        LeaveGameBtn.onClick.AddListener(Application.Quit);
    }
}
