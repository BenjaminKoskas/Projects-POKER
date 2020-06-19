using System.Collections.Generic;
using ExitGames.Client.Photon;
using TMPro;
using UnityEngine;
using Random = System.Random;

public enum PlayerRole {Dealer, BB, SB, UTG, UTG1, UTG2, HiJack, CutOff}

public enum PlayerStatus {Fold, NotTurn, Turn}

public enum PhotonEventCodes {DrawCard = 0, EndTurn = 1, StackUpdate = 2, CheckUpdate = 3, GameStateUpdate = 4}

public class Player : MonoBehaviour, IPunObservable
{
    public PlayerRole role;
    public PlayerStatus status;

    public bool hasCheck = false;

    public int index;

    public TMP_Text stackText;
    public TMP_Text betStackText;

    public GameObject[] cards;
    public GameObject[] hiddenCards;

    public int stack;
    public int betStack;

    [HideInInspector]
    public PhotonView photonView;
    private PhotonPlayer photonPlayer;

    private PlayerUI playerUI;
    private CardDisplay[] cardsDisplays;

    private GameObject betStackParent;

    private GameObject chipsPosition;
    private GameObject rolePosition;

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

                    GameManager.Instance.numberOfPlayerDraw++;
                    GameManager.Instance.playerDrawed = true;
                }
                
            }
        }
        else if (code == PhotonEventCodes.EndTurn)
        {
            object[] datas = content as object[];

            if (datas.Length == 2)
            {
                if ((int) datas[0] == photonView.viewID && !photonView.isMine)
                {
                    if ((int)datas[1] == (int)PlayerStatus.Fold)
                        GameManager.Instance.ordererdPlayers.Remove(this);
                    else
                        GameManager.Instance.numberOfPlayerPlayed++;

                    GameManager.Instance.playerPlayed = true;

                    SetStatus((PlayerStatus)(int)datas[1]);
                }
            } 
            else if (datas.Length == 3)
            {
                if ((int)datas[0] == photonView.viewID && !photonView.isMine)
                {
                    GameManager.Instance.numberOfPlayerPlayed++;
                    GameManager.Instance.playerPlayed = true;

                    SetStatus((PlayerStatus)(int)datas[1]);
                    hasCheck = (bool) datas[2];
                }
            }
        }
        else if (code == PhotonEventCodes.CheckUpdate)
        {
            object[] datas = content as object[];

            if (datas.Length == 2)
            {
                if ((int)datas[0] == photonView.viewID && !photonView.isMine)
                {
                    hasCheck = (bool)datas[1];
                }
            }
        }
    }

    private void Awake()
    {
        SetStatus(PlayerStatus.NotTurn);

        photonView = GetComponent<PhotonView>();
        photonPlayer = photonView.owner;

        stack = (int)photonPlayer.CustomProperties["Stack"];

        playerUI = GetComponent<PlayerUI>();

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

        if (_role != PlayerRole.Dealer || _role != PlayerRole.SB || _role != PlayerRole.BB) { return;}

        GameObject chip = Instantiate(ChipsManager.Instance.chipPrefab, rolePosition.transform);
        ChipDisplay display = chip.GetComponent<ChipDisplay>();

        switch (_role)
        {
            case PlayerRole.Dealer:
                display.SetChip(ChipsManager.Instance.dealer);
                break;
            case PlayerRole.BB:
                display.SetChip(ChipsManager.Instance.BB);
                break;
            case PlayerRole.SB:
                display.SetChip(ChipsManager.Instance.SB);
                break;
        }

        display.gameObject.transform.localScale = new Vector3
        (
            display.gameObject.transform.localScale.x * 2,
            display.gameObject.transform.localScale.y * 2,
            display.gameObject.transform.localScale.z
        );
    }

    public void SetStatus(PlayerStatus _status)
    {
        status = _status;
    }

    public void DrawCard()
    {
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

            GameManager.Instance.numberOfPlayerDraw++;
            GameManager.Instance.playerDrawed = true;
        }
        else
        {
            Debug.LogError("No card found in the deck");
        }
    }

    public void PayBlinds()
    {
        int value = role == PlayerRole.BB ? (int)PhotonNetwork.room.CustomProperties["BB"] : (int)PhotonNetwork.room.CustomProperties["SB"];

        AddBet(value);
    }

    public void Fold()
    {
        SetStatus(PlayerStatus.Fold);

        object[] datas = { photonView.viewID, (int)PlayerStatus.Fold };

        RaiseEventOptions options = new RaiseEventOptions()
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.EndTurn, datas, false, options);

        if(hasCheck)
            SetCheck(false);

        GameManager.Instance.ordererdPlayers.Remove(this);
        GameManager.Instance.playerPlayed = true;
    }

    public void Check()
    {
        SetStatus(PlayerStatus.NotTurn);
        hasCheck = true;

        object[] datas = { photonView.viewID, (int)PlayerStatus.NotTurn, hasCheck };

        RaiseEventOptions options = new RaiseEventOptions()
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.EndTurn, datas, false, options);

        GameManager.Instance.numberOfPlayerPlayed++;
        GameManager.Instance.playerPlayed = true;
    }

    public void SetCheck(bool _check)
    {
        hasCheck = _check;

        object[] datas = { photonView.viewID, hasCheck };

        RaiseEventOptions options = new RaiseEventOptions()
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.CheckUpdate, datas, false, options);
    }

    public void Call()
    {
        SetStatus(PlayerStatus.NotTurn);

        object[] datas = { photonView.viewID, (int)PlayerStatus.NotTurn };

        RaiseEventOptions options = new RaiseEventOptions()
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.EndTurn, datas, false, options);

        if (hasCheck)
            SetCheck(false);

        GameManager.Instance.numberOfPlayerPlayed++;
        GameManager.Instance.playerPlayed = true;
    }

    public void Bet()
    {
        SetStatus(PlayerStatus.NotTurn);

        object[] datas = { photonView.viewID, (int)PlayerStatus.NotTurn };

        RaiseEventOptions options = new RaiseEventOptions()
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.EndTurn, datas, false, options);

        if (hasCheck)
            SetCheck(false);

        GameManager.Instance.numberOfPlayerPlayed++;
        GameManager.Instance.playerPlayed = true;
    }

    public void AddBet(int bet)
    {
        List<Chip> chips = ChipsManager.Instance.FindChipsByValue(bet);

        foreach (Chip c in chips)
        {
            GameObject chip = Instantiate(ChipsManager.Instance.chipPrefab, chipsPosition.transform);
            ChipDisplay display = chip.GetComponent<ChipDisplay>();

            display.SetChip(c);
        }

        if (!betStackParent.activeSelf)
            betStackParent.SetActive(true);

        betStack += bet;
        betStackText.text = betStack + "$";

        GameManager.Instance.mainStack += bet;

        if (photonView.isMine)
        {
            int stack = (int)PhotonNetwork.player.CustomProperties["Stack"];

            ExitGames.Client.Photon.Hashtable hash = new Hashtable
            {
                {"Stack", stack - bet}
            };

            PhotonNetwork.player.SetCustomProperties(hash);

            stackText.text = stack - bet + "$";
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
