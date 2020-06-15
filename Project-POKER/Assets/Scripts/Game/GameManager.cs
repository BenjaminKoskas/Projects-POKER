using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<Card> deck = new List<Card>();

    public GameStateEnum gameState;

    private PhotonView photonView;

    private List<Player> players = new List<Player>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        gameState = GameStateEnum.WaitForPlayers;
        foreach (Card card in CardsManager.Instance.cards.Values)
        {
            deck.Add(card);
            Debug.LogError(card.card);
        }
    }

    private void Update()
    {
        if (PlayerNetwork.Instance == null)
            return;

        bool allPlayersInGame = PlayerNetwork.Instance.PlayersInGame == PhotonNetwork.playerList.Length;

        if (players.Count < PhotonNetwork.playerList.Length)
        {
            foreach (Player p in FindObjectsOfType<Player>())
            {
                if(!players.Contains(p))
                    players.Add(p);
                Debug.Log(p.gameObject.name);
            }
        }

        bool HasAllPlayerObj = players.Count == PhotonNetwork.playerList.Length;

        if (allPlayersInGame && HasAllPlayerObj)
        {
            if (gameState.Equals(GameStateEnum.WaitForPlayers))
            {
                gameState = GameStateEnum.ChooseRole;

                if (PhotonNetwork.isMasterClient)
                {
                    photonView.RPC("RPC_GameStartDefineRole", PhotonTargets.All);
                }
            } 
            else if (gameState.Equals(GameStateEnum.PlayerCardDraw))
            {
                gameState = GameStateEnum.PlayersPlay;
                if (PhotonNetwork.isMasterClient)
                {
                    photonView.RPC("RPC_DrawPlayerCard", PhotonTargets.All);
                }
            }
        }
    }

    [PunRPC]
    private void RPC_GameStartDefineRole()
    {
        foreach (Player p in players)
        {
            if (p.index == 1)
                p.SetRole(PlayerRole.Dealer);

            if (p.index == 2 && PhotonNetwork.playerList.Length > 2)
                p.SetRole(PlayerRole.SB);
            else if (p.index == 2 && PhotonNetwork.playerList.Length == 2)
                p.SetRole(PlayerRole.BB);

            if (p.index == 3)
                p.SetRole(PlayerRole.BB);
        }

        gameState = GameStateEnum.PlayerCardDraw;
    }

    [PunRPC]
    private void RPC_DrawPlayerCard()
    {
        foreach (Player p in players)
        {
            p.DrawCard();
        }
    }
}
