using TMPro;
using UnityEngine;
using Photon;

public class CreateRoom : PunBehaviour
{
    [SerializeField] 
    private TMP_Text _roomName;
    private TMP_Text RoomName => _roomName;

    public void OnClick_CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 6 };

        if (PhotonNetwork.CreateRoom(RoomName.text, roomOptions, TypedLobby.Default))
        {
            print("Create room successfully sent.");
        }
        else
        {
            print("Create room failed to send.");
        }
    }

    public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        print("Create room failed: " + codeAndMsg[1]);
    }

    public override void OnCreatedRoom()
    {
        print("Room created successfully.");
    }
}
