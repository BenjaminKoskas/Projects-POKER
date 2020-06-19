using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private Player player;

    private GameObject playerUI;

    private Button foldButton;
    private Button checkButton;
    private Button callButton;
    private Button BetButton;

    private Button minBet;
    private Button halfBet;
    private Button threeQuartBet;
    private Button potBet;
    private Button allInBet;

    public TMP_InputField betField { get; private set; }

    private bool initialized = false;

    private void Awake()
    {
        player = GetComponent<Player>();
        if (!player.photonView.isMine) { return; }

        playerUI = GameObject.Find("PlayerUI");

        foldButton = GameObject.Find("Fold").GetComponent<Button>();
        checkButton = GameObject.Find("Check").GetComponent<Button>();
        callButton = GameObject.Find("Call").GetComponent<Button>();
        BetButton = GameObject.Find("Bet").GetComponent<Button>();

        minBet = GameObject.Find("MinBet").GetComponent<Button>();
        halfBet = GameObject.Find("1/2Bet").GetComponent<Button>();
        threeQuartBet = GameObject.Find("3/4Bet").GetComponent<Button>();
        potBet = GameObject.Find("PotBet").GetComponent<Button>();
        allInBet = GameObject.Find("AllInBet").GetComponent<Button>();

        betField = GameObject.Find("BetInput").GetComponent<TMP_InputField>();

        foldButton.onClick.AddListener(player.Fold);
        checkButton.onClick.AddListener(player.Check);
        callButton.onClick.AddListener(player.Call);
        BetButton.onClick.AddListener(player.Bet);

        minBet.onClick.AddListener(MinBet);
        halfBet.onClick.AddListener(HalfBet);
        threeQuartBet.onClick.AddListener(ThreeQuartBet);
        potBet.onClick.AddListener(PotBet);
        allInBet.onClick.AddListener(AllInBet);

        initialized = true;
    }

    private void Update()
    {
        if (!initialized) { return; }

        if (player.status.Equals(PlayerStatus.Fold) || player.status.Equals(PlayerStatus.NotTurn) && playerUI.activeSelf)
            playerUI.SetActive(false);
        else if (player.status.Equals(PlayerStatus.Turn) && !playerUI.activeSelf && !player.hasCheck && GameManager.Instance.mainStack == 0)
        {
            playerUI.SetActive(true);
            checkButton.gameObject.SetActive(true);
        }
        else if (player.status.Equals(PlayerStatus.Turn) && !playerUI.activeSelf && player.hasCheck)
        {
            playerUI.SetActive(true);
            checkButton.gameObject.SetActive(false);
        } 
        else if (player.status.Equals(PlayerStatus.Turn) && !playerUI.activeSelf && !player.hasCheck &&
                 GameManager.Instance.mainStack > 0)
        {
            if(GameManager.Instance.gameState.Equals(GameStateEnum.Preflop))
                if (player.role.Equals(PlayerRole.BB) && !(GameManager.Instance.FindPlayerBefore(player).betStack > (int)PhotonNetwork.room.CustomProperties["BB"]))
                {
                    playerUI.SetActive(true);
                    checkButton.gameObject.SetActive(true);
                    return;
                }

            playerUI.SetActive(true);
            checkButton.gameObject.SetActive(false);
        }

        if (playerUI.activeSelf)
        {
            if (GameManager.Instance.mainStack == 0)
            {
                minBet.gameObject.SetActive(false);
                halfBet.gameObject.SetActive(false);
                threeQuartBet.gameObject.SetActive(false);
                potBet.gameObject.SetActive(false);
            } 
            else if(GameManager.Instance.mainStack == (int)PhotonNetwork.room.CustomProperties["BB"])
            {
                halfBet.gameObject.SetActive(false);
                threeQuartBet.gameObject.SetActive(false);
            } 
            else if (GameManager.Instance.mainStack / 2 < (int) PhotonNetwork.room.CustomProperties["BB"])
            {
                halfBet.gameObject.SetActive(false);
            } 
            else if (GameManager.Instance.mainStack * 0.75 < (int) PhotonNetwork.room.CustomProperties["BB"])
            {
                threeQuartBet.gameObject.SetActive(false);
            }
            else
            {
                minBet.gameObject.SetActive(true);
                halfBet.gameObject.SetActive(true);
                threeQuartBet.gameObject.SetActive(true);
                potBet.gameObject.SetActive(true);
            }
        }
    }

    private void MinBet()
    {
        betField.text = (float)PhotonNetwork.room.CustomProperties["BB"] + "$";
    }

    private void HalfBet()
    {
        betField.text = GameManager.Instance.mainStack / 2 + "$";
    }

    private void ThreeQuartBet()
    {
        betField.text = GameManager.Instance.mainStack * 0.75 + "$";
    }

    private void PotBet()
    {
        betField.text = GameManager.Instance.mainStack + "$";
    }

    private void AllInBet()
    {
        betField.text = player.stack.ToString() + '$';
    }
}
