using UnityEngine;

public enum PlayerRole {NotDefined, Dealer, BB, SB}

public class Player : MonoBehaviour
{
    public PlayerRole role;

    public int index;

    private PhotonView photonView;
    private PhotonPlayer photonPlayer;

    private GameObject chipsPosition;
    private GameObject rolePosition;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        photonPlayer = photonView.owner;

        index = (int) photonPlayer.CustomProperties["Index"];

        chipsPosition = GameObject.Find("ChipPlayer" + index);
        rolePosition = GameObject.Find("PlayerPos" + index);
    }

    public void SetRole(PlayerRole _role)
    {
        role = _role;

        GameObject obj = ChipsManager.Instance.chipPrefab;

        switch (_role)
        {
            case PlayerRole.Dealer:
                obj.GetComponent<ChipDisplay>().chip = ChipsManager.Instance.chips["Dealer"];
                break;
            case PlayerRole.BB:
                obj.GetComponent<ChipDisplay>().chip = ChipsManager.Instance.chips["BB"];
                break;
            case PlayerRole.SB:
                obj.GetComponent<ChipDisplay>().chip = ChipsManager.Instance.chips["SB"];
                break;
        }

        Instantiate(obj, rolePosition.transform);
    }
}
