using UnityEngine;
using UnityEngine.UI;

public class CurrentRoomCanvas : MonoBehaviour
{
    public Slider buyInSlider;

    public void OnClickStartSync()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable()
        {
            {"Stack", (int)buyInSlider.value}
        };

        PhotonNetwork.player.SetCustomProperties(hash);

        PhotonNetwork.LoadLevel(4);
    }

    public void OnClickStartDelayed()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        PhotonNetwork.room.IsOpen = false;
        PhotonNetwork.room.IsVisible = false;
        PhotonNetwork.LoadLevel(4);
    }
}
