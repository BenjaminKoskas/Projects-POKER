using UnityEngine;
using TMPro;

public class PlayerNetManager : MonoBehaviour
{
    public GameObject Username;
    public GameObject Cash;
    public GameObject Action;

    private TMP_Text UsernameText;
    private TMP_Text CashText;
    private TMP_Text ActionText;

    private void Awake()
    {
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
}
