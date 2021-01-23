using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = System.Random;

public enum PlayerRole {Dealer, BB, SB, UTG, UTG1, UTG2, HiJack, CutOff}

public enum PlayerStatus {Fold, NotTurn, Turn}

public enum PhotonEventCodes {DrawCard = 0, EndTurn = 1, RoleUpdate = 2, BetUpdate = 3}

public class Player : MonoBehaviour, IPunObservable
{
    public PlayerRole role;
    public PlayerStatus status;

    public bool hasCheck = false;
    public bool hasPlay = false;

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

    private void Update()
    {
        if (photonView.isMine)
            Debug.Log(role);
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
        else if (code == PhotonEventCodes.EndTurn)
        {
            object[] datas = content as object[];

            if (datas.Length == 1)
            {
                if((int)datas[0] == photonView.viewID)
                    GameManager.Instance.ordererdPlayers.Remove(this);
            }
        }
        else if (code == PhotonEventCodes.RoleUpdate)
        {
            object[] datas = content as object[];

            if (datas.Length == 4)
            {
                if ((int) datas[0] != photonView.viewID) { return;}
                Chip _chip = new Chip();

                if ((bool)datas[1])
                    _chip = ChipsManager.Instance.BB;
                else if ((bool)datas[2])
                    _chip = ChipsManager.Instance.SB;
                else if ((bool)datas[3])
                    _chip = ChipsManager.Instance.dealer;

                GameObject chip = Instantiate(ChipsManager.Instance.chipPrefab, rolePosition.transform);
                ChipDisplay display = chip.GetComponent<ChipDisplay>();

                display.SetChip(_chip);

                display.gameObject.transform.localScale = new Vector3
                (
                    display.gameObject.transform.localScale.x * 2,
                    display.gameObject.transform.localScale.y * 2,
                    display.gameObject.transform.localScale.z
                );
            }
        }
        else if (code == PhotonEventCodes.BetUpdate)
        {
            object[] datas = content as object[];

            if (datas.Length == 3)
            {
                if ((int)datas[0] != photonView.viewID) { return; }
                int[] chipsValue = (int[]) datas[1];

                List<Chip> chips = new List<Chip>();

                foreach (int value in chipsValue)
                {
                    foreach (Chip c in ChipsManager.Instance.chips.Values)
                    {
                        if(c.value == value)
                            chips.Add(c);
                    }
                }

                foreach (Chip c in chips)
                {
                    GameObject chip = Instantiate(ChipsManager.Instance.chipPrefab, chipsPosition.transform);
                    ChipDisplay display = chip.GetComponent<ChipDisplay>();

                    display.SetChip(c);
                }

                if (!betStackParent.activeSelf)
                    betStackParent.SetActive(true);

                betStackText.text = (int)datas[2] + "$";
                stackText.text = stack + "$";
            }
        }
    }

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        photonPlayer = photonView.owner;

        SetStatus(PlayerStatus.NotTurn);

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

        switch (_role)
        {
            case PlayerRole.Dealer:
                SetRoleChip(ChipsManager.Instance.dealer);
                break;
            case PlayerRole.BB:
                SetRoleChip(ChipsManager.Instance.BB);
                break;
            case PlayerRole.SB:
                SetRoleChip(ChipsManager.Instance.SB);
                break;
        }
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
        if (hasCheck)
            SetCheck(false);
        hasPlay = true;

        GameManager.Instance.playerPlayed = true;

        object[] datas = { photonView.viewID };

        RaiseEventOptions options = new RaiseEventOptions()
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.EndTurn, datas, false, options);
    }

    public void Check()
    {
        SetStatus(PlayerStatus.NotTurn);
        SetCheck(true);
        GameManager.Instance.playIndex++;
        GameManager.Instance.playerPlayed = true;
        hasPlay = true;
    }

    public void SetCheck(bool _check)
    {
        hasCheck = _check;
    }

    public void Call()
    {
        SetStatus(PlayerStatus.NotTurn);
        if (hasCheck)
            SetCheck(false);
        hasPlay = true;

        GameManager.Instance.playIndex++;
        GameManager.Instance.playerPlayed = true;
    }

    public void Bet()
    {
        string betT = playerUI.betField.text.Replace('$', ' ');
        int bet = int.Parse(betT);
        if (GameManager.Instance.FindPlayerBefore(this).hasCheck ||
            GameManager.Instance.FindFirstPlayerToPlay() == this)
        {
            if (bet < (int) PhotonNetwork.room.CustomProperties["BB"])
                return;
        }
        else
        {
            if (bet < GameManager.Instance.FindPlayerBefore(this).betStack)
                return;
        }

        if (bet > GameManager.Instance.FindPlayerBefore(this).betStack)
        {
            if (GameManager.Instance.rebet != 3)
            {
                GameManager.Instance.rebet++;
                GameManager.Instance.minBet = bet;
            }
            else
                return;
        }

        AddBet(bet);
        SetStatus(PlayerStatus.NotTurn);
        if (hasCheck)
            SetCheck(false);
        hasPlay = true;

        GameManager.Instance.playIndex++;
        GameManager.Instance.playerPlayed = true;
    }

    public void AddBet(int bet)
    {
        GameManager.Instance.mainStack += bet;
        betStack += bet;
        Debug.LogError(betStack);
        stack -= bet;

        int[] chipsValue = new int[ChipsManager.Instance.FindChipsByValue(bet).Count];
        foreach (Chip c in ChipsManager.Instance.FindChipsByValue(bet))
        {
            chipsValue.Append(c.value);
        }

        object[] datas = { photonView.viewID, chipsValue, bet };

        RaiseEventOptions options = new RaiseEventOptions()
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.BetUpdate, datas, false, options);
    }

    public void SetRoleChip(Chip _chip)
    {
        object[] datas = { photonView.viewID, _chip.isBB, _chip.isSB, _chip.isDealer };

        RaiseEventOptions options = new RaiseEventOptions()
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.RoleUpdate, datas, false, options);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(betStack);
            stream.SendNext(stack);
            stream.SendNext((int)status);
            stream.SendNext(hasCheck);
            stream.SendNext(hasPlay);
        }
        else if (stream.isReading)
        {
            betStack = (int)stream.ReceiveNext();
            stack = (int)stream.ReceiveNext();
            status = (PlayerStatus)(int)stream.ReceiveNext();
            hasCheck = (bool)stream.ReceiveNext();
            hasPlay = (bool)stream.ReceiveNext();
        }
    }
}
