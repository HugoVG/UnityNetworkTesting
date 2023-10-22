using System;
using System.Threading.Tasks;
#if UNITY_EDITOR
using ParrelSync;
#endif
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyMaker : MonoBehaviour
{
    
    [SerializeField] TMP_Text joinCodeText;
    [SerializeField] TMP_InputField joinCodeInputField;
    [SerializeField] GameObject buttons;
    [SerializeField] GameObject LobbyUI;
    
    private UnityTransport transportProvider;
    private const int maxPlayers = 3;
    
    private async void Awake()
    {
        //Get the transport provider
        transportProvider = FindObjectOfType<UnityTransport>();
        buttons.SetActive(false);
        LobbyUI.SetActive(false);
        await Authenticate();
        
        buttons.SetActive(true);
    }

    private static async Task Authenticate()
    {
        var options = new InitializationOptions();
        
#if UNITY_EDITOR
        if (ClonesManager.IsClone())
        {
            options.SetProfile(ClonesManager.GetArgument());
        }
#endif
        
        await UnityServices.InitializeAsync(options);

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in:" + AuthenticationService.Instance.IsSignedIn);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateGame()
    {
        buttons.SetActive(false);
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            joinCodeText.text = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            transportProvider.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);
            NetworkManager.Singleton.StartHost();
            LobbyUI.SetActive(true);
        }
        catch(Exception e)
        {
            Debug.Log(e);
            buttons.SetActive(true);
        }
    }

    public async void JoinGame()
    {
        buttons.SetActive(false);
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCodeInputField.text);
        transportProvider.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);
        Debug.Log($"client: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"host: {allocation.HostConnectionData[0]} {allocation.HostConnectionData[1]}");
        Debug.Log($"client: {allocation.AllocationId}");
        NetworkManager.Singleton.StartClient();
        LobbyUI.SetActive(true);
    }
    
    public void StartGame()
    {
        if(!NetworkManager.Singleton.IsHost) return;
        Debug.Log("StartingGame");
        NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
