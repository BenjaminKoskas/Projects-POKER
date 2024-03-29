﻿using Photon;

public class LobbyNetwork : PunBehaviour
{
    private void Start()
    {
        print("Connecting to server...");
        PhotonNetwork.ConnectUsingSettings("1.0.0");
    }

    public override void OnConnectedToMaster()
    {
        print("Connected to master.");
        PhotonNetwork.automaticallySyncScene = false;
        PhotonNetwork.playerName = PlayerNetwork.Instance.PlayerName;
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnJoinedLobby()
    {
        print("Joined lobby.");

        if (!PhotonNetwork.inRoom)
        {
            MainCanvasManager.Instance.CurrentRoomCanvas.transform.SetAsFirstSibling();
            MainCanvasManager.Instance.CreateRoomUI.SetActive(true);
        }
    }
}
