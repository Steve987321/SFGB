using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
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

    public static async Task<string> CreateRelay()
    {
        string res;
        try
        {
            var alloc = await RelayService.Instance.CreateAllocationAsync(1);

            res = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

            var relayServerData = new RelayServerData(alloc, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        }
        catch (RelayServiceException e)
        {
            Debug.LogException(e);
            res = "";
        }

        return res;
    }

    public static async Task JoinRelay(string code)
    {
        try
        {
            var alloc = await RelayService.Instance.JoinAllocationAsync(code);

            var relayServerData = new RelayServerData(alloc, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        }
        catch (RelayServiceException e)
        {
            Debug.LogException(e);
        }
    }
}
