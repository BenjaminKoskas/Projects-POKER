﻿using System;
using TMPro;
using UnityEngine;
using Photon;

public class CreateRoom : PunBehaviour
{
    [SerializeField] 
    private TMP_Text _roomName;
    private TMP_Text RoomName => _roomName;

    [SerializeField]
    private TMP_InputField _BuyIn;
    private TMP_InputField BuyIn => _BuyIn;

    [SerializeField]
    private TMP_Dropdown _type;
    private TMP_Dropdown Type => _type;

    [SerializeField]
    private TMP_InputField _maxPlayers;
    private TMP_InputField MaxPlayers => _maxPlayers;

    public void OnClick_CreateRoom()
    {
        if (RoomName.text == "" || BuyIn.text == "" ||
            MaxPlayers.text == "")
        {
            Debug.Log("Some room properties are not set");
            return;
        }

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable
        {
            {"BB", (int)Math.Round((float.Parse(BuyIn.text) / 100))}, 
            {"SB", (int)Math.Round((float.Parse(BuyIn.text) / 100) / 2)}, 
            {"Type", Type.captionText.text}, 
            {"MaxPlayers", int.Parse(MaxPlayers.text)}, 
            {"MaxBuyIn", int.Parse(BuyIn.text)},
            {"MinBuyIn", (int)Math.Round((float.Parse(BuyIn.text) / 2))}
        };

        string[] ss = {"BB", "SB", "Type", "MaxPlayers"};

        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = Convert.ToByte((int)hash["MaxPlayers"]), CustomRoomProperties = hash, CustomRoomPropertiesForLobby = ss };
        
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
