using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public enum PlayerRole {NotDefined, Dealer, BB, SB}

public enum PhotonEventCodes {DrawCard = 0}

public class Player : MonoBehaviour
{
    public PlayerRole role;

    public int index;

    public GameObject[] cards;
    public GameObject[] hiddenCards;

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

                    GameManager.Instance.deck.Remove(card1);
                    GameManager.Instance.deck.Remove(card2);

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
            Debug.LogError("Not card found in the deck");
        }
    }
}
