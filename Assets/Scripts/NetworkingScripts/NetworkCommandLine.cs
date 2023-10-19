using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkCommandLine : MonoBehaviour
{
   private NetworkManager netManager;

   void Start()
   {
       if (Application.isEditor) return;
       netManager = GetComponentInParent<NetworkManager>();


       var args = GetCommandlineArgs();

       if (!args.TryGetValue("-mode", out string mode)) return;
       
       switch (mode)
       {
           case "server":
               netManager.StartServer();
               break;
           case "host":
               netManager.StartHost();
               break;
           case "client":
               netManager.StartClient();
               break;
       }
   }
   void OnGUI()
   {
       GUILayout.BeginArea(new Rect(10, 10, 300, 300));
       if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
       {
           StartButtons();
       }
       else
       {
           StatusLabels();

           SubmitNewPosition();
       }

       GUILayout.EndArea();
   }

   static void StartButtons()
   {
       if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
       if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
       if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
   }

   static void StatusLabels()
   {
       var mode = NetworkManager.Singleton.IsHost ?
           "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

       GUILayout.Label("Transport: " +
                       NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
       GUILayout.Label("Mode: " + mode);
   }

   static void SubmitNewPosition()
   {
       if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change"))
       {
           if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient )
           {
               foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
                   NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<PlayerMover>().Move();
           }
           else
           {
               var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
               var player = playerObject.GetComponent<PlayerMover>();
               player.Move();
           }
       }
   }

   private Dictionary<string, string> GetCommandlineArgs()
   {
       Dictionary<string, string> argDictionary = new Dictionary<string, string>();

       var args = System.Environment.GetCommandLineArgs();

       for (int i = 0; i < args.Length; ++i)
       {
           var arg = args[i].ToLower();
           if (!arg.StartsWith('-')) continue;
           var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
           value = (value?.StartsWith('-') ?? false) ? null : value;
           argDictionary.Add(arg, value);
       }
       return argDictionary;
   }
}