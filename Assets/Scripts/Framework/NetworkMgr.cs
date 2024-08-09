using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Transporting.Tugboat;
using GamePlay.Room;
using UnityEngine;

public class NetworkMgr : MonoBehaviour
{
    public Tugboat tugboat;
    public NetworkManager networkManager;
    public LocalConnectionState ClientState { get; private set; }
    public LocalConnectionState ServerState { get; private set; }
    public static NetworkMgr Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        networkManager.ClientManager.OnClientConnectionState += OnClientConnection;
        networkManager.ServerManager.OnServerConnectionState += OnServerConnection;
        networkManager.ServerManager.OnRemoteConnectionState += (connection, obj) =>
        {
            if (obj.ConnectionState == RemoteConnectionState.Started)
            {
            }
            else if (obj.ConnectionState == RemoteConnectionState.Stopped)
            {
            }
        };
    }

    public bool CreateRoom(RoomType roomType, string roomName)
    {
        if (networkManager == null)
            return false;
        RoomMgr.Instance.SetRoomConfig(roomType, roomName);
        var flag = networkManager.ServerManager.StartConnection();
        return flag;
    }

    public bool CloseRoom()
    {
        if (networkManager == null)
            return false;
        return networkManager.ServerManager.StopConnection(true);
    }

    public bool JoinRoom()
    {
        if (networkManager == null)
            return false;
        var flag = networkManager.ClientManager.StartConnection();
        return flag;
    }

    public bool ExitRoom()
    {
        if (networkManager == null)
            return false;

        var flag = networkManager.ClientManager.StopConnection();
        return flag;
    }

    private void OnServerConnection(ServerConnectionStateArgs obj)
    {
        ServerState = obj.ConnectionState;
    }

    private void OnClientConnection(ClientConnectionStateArgs obj)
    {
        ClientState = obj.ConnectionState;
    }
}