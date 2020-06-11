using UnityEngine;
using Photon;
using TMPro;

public class PlayerListing : PunBehaviour
{
    public PhotonPlayer PhotonPlayer { get; private set; }

    [SerializeField] 
    private TMP_Text _playerName;
    private TMP_Text PlayerName => _playerName;

    public void ApplyPhotonPlayer(PhotonPlayer photonPlayer)
    {
        PhotonPlayer = photonPlayer;
        PlayerName.text = photonPlayer.NickName;
    }
}
