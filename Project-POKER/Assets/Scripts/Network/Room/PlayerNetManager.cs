using UnityEngine;
using Photon;
using TMPro;

public class PlayerNetManager : PunBehaviour, IPunObservable
{
    public GameObject Username;
    public GameObject Cash;
    public GameObject Action;

    private PhotonView PhotonView;

    private Vector3 realPosition;
    private Vector3 realScale;
    private Transform realParent;

    private TMP_Text UsernameText;
    private TMP_Text CashText;
    private TMP_Text ActionText;

    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();

        UsernameText = Username.GetComponent<TMP_Text>();
        CashText = Cash.GetComponent<TMP_Text>();
        ActionText = Action.GetComponent<TMP_Text>();
    }

    private void Start()
    {
        PhotonPlayer player = GetComponent<PhotonView>().owner;

        transform.SetParent(GameObject.Find("Table").transform);

        int index = (int)player.CustomProperties["Index"];
        int stack = (int) player.CustomProperties["Stack"];

        transform.localScale = new Vector3(1, 1, 1);
        transform.localPosition = new Vector3(PlayerNetwork.Instance.playersPosition[index - 1].x, PlayerNetwork.Instance.playersPosition[index - 1].y);

        UsernameText.text = player.NickName;
        CashText.text = stack.ToString();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.localPosition);
            stream.SendNext(transform.localScale);
            stream.SendNext(transform.parent);
        }
        else
        {
            realPosition = (Vector3) stream.ReceiveNext();
            realScale = (Vector3) stream.ReceiveNext();
            realParent = (Transform) stream.ReceiveNext();
        }
    }
}
