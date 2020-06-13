using TMPro;
using UnityEngine;
using Photon;

public class CreateRoom : PunBehaviour
{
    [SerializeField] 
    private TMP_Text _roomName;
    private TMP_Text RoomName => _roomName;

    [SerializeField]
    private TMP_InputField _BB;
    private TMP_InputField BB => _BB;

    [SerializeField]
    private TMP_InputField _SB;
    private TMP_InputField SB => _SB;

    [SerializeField]
    private TMP_Dropdown _type;
    private TMP_Dropdown Type => _type;

    [SerializeField]
    private TMP_InputField _maxPlayers;
    private TMP_InputField MaxPlayers => _maxPlayers;

    public void OnClick_CreateRoom()
    {
        if (RoomName.text == "" || BB.text == "" || SB.text == "" ||
            MaxPlayers.text == "")
        {
            Debug.Log("Some room properties are not set");
            return;
        }

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable
        {
            {"BB", int.Parse(BB.text)}, {"SB", int.Parse(SB.text)}, {"Type", Type.captionText.text}, {"MaxPlayers", int.Parse(MaxPlayers.text)}
        };

        string[] ss = {"BB", "SB", "Type", "MaxPlayers"};

        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 6, CustomRoomProperties = hash, CustomRoomPropertiesForLobby = ss };
        
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
