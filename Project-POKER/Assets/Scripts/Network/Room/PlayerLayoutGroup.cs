using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon;

public class PlayerLayoutGroup : PunBehaviour
{
    [SerializeField] 
    private GameObject _playerListingPrefab;
    private GameObject PlayerListingPrefab => _playerListingPrefab;

    private List<PlayerListing> _playerListings = new List<PlayerListing>();
    private List<PlayerListing> PlayerListings => _playerListings;

    public override void OnJoinedRoom()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        MainCanvasManager.Instance.CurrentRoomCanvas.transform.SetAsLastSibling();
        MainCanvasManager.Instance.CreateRoomUI.SetActive(false);

        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
        for (int i = 0; i < photonPlayers.Length; i++)
        {
            PlayerJoinedRoom(photonPlayers[i]);
        }
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        PlayerJoinedRoom(newPlayer);
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        PlayerLeftRoom(otherPlayer);
    }

    private void PlayerJoinedRoom(PhotonPlayer photonPlayer)
    {
        if (photonPlayer == null) { return; }

        PlayerLeftRoom(photonPlayer);

        ExitGames.Client.Photon.Hashtable hash = new Hashtable
        {
            {"Index", PhotonNetwork.room.PlayerCount} , {"Stack", 0}
        };

        photonPlayer.CustomProperties = hash;

        GameObject playerListingObj = Instantiate(PlayerListingPrefab);
        playerListingObj.transform.SetParent(transform, false);

        PlayerListing playerListing = playerListingObj.GetComponent<PlayerListing>();
        playerListing.ApplyPhotonPlayer(photonPlayer);

        PlayerListings.Add(playerListing);
    }

    private void PlayerLeftRoom(PhotonPlayer photonPlayer)
    {
        int index = PlayerListings.FindIndex(x => x.PhotonPlayer == photonPlayer);
        if (index != -1)
        {
            Destroy(PlayerListings[index].gameObject);
            PlayerListings.RemoveAt(index);
        }
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
