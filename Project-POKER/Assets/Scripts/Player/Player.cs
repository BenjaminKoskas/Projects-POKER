using ExitGames.Client.Photon;
using TMPro;
using UnityEngine;
using Random = System.Random;

public enum PlayerRole {NotDefined, Dealer, BB, SB}

public enum PhotonEventCodes {DrawCard = 0}

public class Player : MonoBehaviour, IPunObservable
{
    public PlayerRole role;

    public int index;

    public TMP_Text stackText;
    public TMP_Text betStackText;

    public GameObject[] cards;
    public GameObject[] hiddenCards;

    private int betStack;
    private GameObject betStackParent;

    private PhotonView photonView;
    private PhotonPlayer photonPlayer;

    private GameObject chipsPosition;
    private GameObject rolePosition;

    private CardDisplay[] cardsDisplays;

    private void OnEnable()
    {
        PhotonNetwork.OnEventCall += OnPhotonEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.OnEventCall -= OnPhotonEvent;
    }

    private void OnPhotonEvent(byte eventCode, object content, int senderId)
    {
        PhotonEventCodes code = (PhotonEventCodes) eventCode;
        if (code == PhotonEventCodes.DrawCard)
        {
            object[] datas = content as object[];

            if (datas.Length == 3)
            {
                if ((int) datas[0] == photonView.viewID)
                {
                    Card card1 = new Card();
                    Card card2 = new Card();

                    CardEnum card1Enum = (CardEnum)(int)datas[1];
                    CardEnum card2Enum = (CardEnum)(int)datas[2];

                    foreach (Card card in CardsManager.Instance.cards.Values)
                    {
                        if (card.card == card1Enum)
                            card1 = card;
                        else if (card.card == card2Enum)
                            card2 = card;
                    }

                    if (GameManager.Instance.deck.Contains(card1) && GameManager.Instance.deck.Contains(card2))
                    {
                        GameManager.Instance.deck.Remove(card1);
                        GameManager.Instance.deck.Remove(card2);
                    }

                    cardsDisplays[0].SetCard(card1);
                    cardsDisplays[1].SetCard(card2);
                }

                if (photonView.isMine)
                {
                    cards[0].SetActive(true);
                    cards[1].SetActive(true);
                }
                else
                {
                    hiddenCards[0].SetActive(true);
                    hiddenCards[1].SetActive(true);
                }
                
            }
        }
    }

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        photonPlayer = photonView.owner;

        index = (int) photonPlayer.CustomProperties["Index"];

        chipsPosition = GameObject.Find("ChipPlayer" + index);
        rolePosition = GameObject.Find("PlayerPos" + index);

        cardsDisplays = new[] {cards[0].GetComponent<CardDisplay>(), cards[1].GetComponent<CardDisplay>()};

        betStackParent = GameObject.Find("Stack_BG" + index);
        betStackText = GameObject.Find("Stack" + index).GetComponent<TMP_Text>();

        betStackParent.SetActive(false);
    }

    public void SetRole(PlayerRole _role)
    {
        role = _role;

        GameObject chip = Instantiate(ChipsManager.Instance.chipPrefab, rolePosition.transform);
        ChipDisplay display = chip.GetComponent<ChipDisplay>();

        switch (_role)
        {
            case PlayerRole.Dealer:
                display.SetChip(ChipsManager.Instance.chips["Dealer"]);
                break;
            case PlayerRole.BB:
                display.SetChip(ChipsManager.Instance.chips["BB"]);
                break;
            case PlayerRole.SB:
                display.SetChip(ChipsManager.Instance.chips["SB"]);
                break;
        }

        display.gameObject.transform.localScale = new Vector3
        (
            display.gameObject.transform.localScale.x * 2,
            display.gameObject.transform.localScale.y * 2,
            display.gameObject.transform.localScale.z
        );
    }

    public void DrawCard()
    {
        if (!photonView.owner.Equals(PhotonNetwork.player)) { return; }

        Random r = new Random();

        if (GameManager.Instance.deck.Count > 0)
        {
            Card card1 = GameManager.Instance.deck[r.Next(0, GameManager.Instance.deck.Count - 1)];
            GameManager.Instance.deck.Remove(card1);

            Card card2 = GameManager.Instance.deck[r.Next(0, GameManager.Instance.deck.Count - 1)];
            GameManager.Instance.deck.Remove(card1);

            object[] datas = { photonView.viewID, (int)card1.card, (int)card2.card };

            RaiseEventOptions options = new RaiseEventOptions()
            {
                CachingOption = EventCaching.DoNotCache,
                Receivers = ReceiverGroup.All
            };

            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.DrawCard, datas, false, options);
        }
        else
        {
            Debug.LogError("No card found in the deck");
        }
    }

    public void PayBlinds()
    {
        int value = role == PlayerRole.BB ? (int)PhotonNetwork.room.CustomProperties["BB"] : (int)PhotonNetwork.room.CustomProperties["SB"];

        GameObject chip = Instantiate(ChipsManager.Instance.chipPrefab, chipsPosition.transform);
        ChipDisplay display = chip.GetComponent<ChipDisplay>();

        display.SetChip(role == PlayerRole.BB ? ChipsManager.Instance.chips["1%"] : ChipsManager.Instance.chips["0.5%"]);

        if(!betStackParent.activeSelf)
            betStackParent.SetActive(true);

        betStack += value;
        betStackText.text = betStack + "$";

        if (photonView.isMine)
        {
            int stack = (int) PhotonNetwork.player.CustomProperties["Stack"];

            ExitGames.Client.Photon.Hashtable hash = new Hashtable
            {
                {"Stack", stack - value}
            };

            PhotonNetwork.player.SetCustomProperties(hash);

            stackText.text = stack - value + "$";
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(stackText.text);
            stream.SendNext(betStackText.text);
        }
        else if (stream.isReading)
        {
            stackText.text = (string) stream.ReceiveNext();
            betStackText.text = (string) stream.ReceiveNext();
        }
    }
}
