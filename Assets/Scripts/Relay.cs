using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using UnityEngine;

public class Relay : MonoBehaviour
{

    async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => { print("signed in"); };
        AuthenticationService.Instance.SignInFailed += t=> { Debug.LogWarning("SignIn Failed: " + t.ErrorCode); };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public static async Task CreateRelay()
    {
        try
        {
            var alloc = await RelayService.Instance.CreateAllocationAsync(1);

            GameManager.Instance.GameCodeText.text = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);
            GameManager.Instance.GameCodeText.gameObject.SetActive(true);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData
            (
                alloc.RelayServer.IpV4,
                (ushort)alloc.RelayServer.Port,
                alloc.AllocationIdBytes,
                alloc.Key,
                alloc.ConnectionData
            );
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e.Reason);
            GameManager.Instance.GameCodeText.text = "";
        }
    }

    public static async Task JoinRelay(string code)
    {
        try
        {
            var alloc = await RelayService.Instance.JoinAllocationAsync(code);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData
            (
                alloc.RelayServer.IpV4,
                (ushort)alloc.RelayServer.Port,
                alloc.AllocationIdBytes,
                alloc.Key,
                alloc.ConnectionData,
                alloc.HostConnectionData
            );
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e.Reason);
        }
    }
}
