using Mirror;
using UnityEngine;

public class NetMan : NetworkManager 
{
    private bool _playerSpawned;
    private bool _playerConnected;

    public void OnCreateCharacter(NetworkConnectionToClient conn, PosMessage message) 
    {
        GameObject go = Instantiate(playerPrefab, message.vector2, Quaternion.identity);

        NetworkServer.AddPlayerForConnection(conn, go);
    }

    public override void OnStartServer() 
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<PosMessage>(OnCreateCharacter);
    }

    public void ActivatePlayerSpawn() 
    {
        Vector3 pos = Input.mousePosition;
        pos.z = 10f;
        pos = Camera.main.ScreenToWorldPoint(pos);

        PosMessage m = new PosMessage() 
        {
            vector2 = pos
        };


        NetworkClient.Send(m);
        _playerSpawned = true;
    }

    public override void OnClientConnect() 
    {
        base.OnClientConnect();
        _playerConnected = true;
    }

    public override void Update() {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Mouse0) && !_playerSpawned && _playerConnected) 
        {
            ActivatePlayerSpawn();
        }
    }
}

public struct PosMessage : NetworkMessage 
{
    public Vector2 vector2;
}