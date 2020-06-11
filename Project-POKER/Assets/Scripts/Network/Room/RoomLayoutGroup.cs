using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;

public class RoomLayoutGroup : PunBehaviour
{
    [SerializeField] 
    private GameObject _roomPrefab;
    private GameObject RoomPrefab => _roomPrefab;

    private List<RoomListing> _roomListingsButtons = new List<RoomListing>();
    private List<RoomListing> RoomListingsButtons => _roomListingsButtons;

    public override void OnReceivedRoomListUpdate()
    {
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();

        foreach (RoomInfo room in rooms)
        {
            RoomReceived(room);
        }

        RemoveOldRooms();
    }

    private void RoomReceived(RoomInfo room)
    {
        int index = RoomListingsButtons.FindIndex(x => x.RoomName == room.Name);

        if (index == -1)
        {
            if (room.IsVisible && room.PlayerCount < room.MaxPlayers)
            {
                GameObject roomListingObj = Instantiate(RoomPrefab);
                roomListingObj.transform.SetParent(transform, false);

                RoomListing roomListing = roomListingObj.GetComponent<RoomListing>();
                RoomListingsButtons.Add(roomListing);

                index = (RoomListingsButtons.Count - 1);
                if (index % 2 != 0)
                {
                    roomListingObj.GetComponent<Image>().color = new Color(75, 75, 75, 255);
                }
            }
        }

        if (index != -1)
        {
            RoomListing roomListing = RoomListingsButtons[index];
            roomListing.SetRoomNameText(room.Name);
            roomListing.Updated = true;
        }
    }

    private void RemoveOldRooms()
    {
        List<RoomListing> removeRooms = new List<RoomListing>();

        foreach (RoomListing roomListing in RoomListingsButtons)
        {
            if (!roomListing.Updated)
                removeRooms.Add(roomListing);
            else
                roomListing.Updated = false;
        }

        foreach (RoomListing roomListing in removeRooms)
        {
            GameObject roomListingObj = roomListing.gameObject;
            RoomListingsButtons.Remove(roomListing);
            Destroy(roomListingObj);
        }
    }
}
