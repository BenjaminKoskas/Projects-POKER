using System.Collections.Generic;
using UnityEngine;

public enum GameStateEnum
{
    WaitForPlayers, Distribution, Blinds, Preflop, Flop, Turn, River
}


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
        }

        for (int i = PhotonNetwork.room.PlayerCount; i < 8; i++)
        {
            GameObject.Find("Stack_BG"+(i+1)).SetActive(false);
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
            }
        }

        bool HasAllPlayerObj = players.Count == PhotonNetwork.playerList.Length;

        switch (gameState)
        {
            case GameStateEnum.WaitForPlayers:
                if (allPlayersInGame && HasAllPlayerObj)
                {
                    gameState = GameStateEnum.Distribution;
                }
                break;
            case GameStateEnum.Distribution:
                if (PhotonNetwork.isMasterClient)
                {
                    photonView.RPC("RPC_GameStartDefineRole", PhotonTargets.All);
                }
                break;
            case GameStateEnum.Blinds:
                if (PhotonNetwork.isMasterClient)
                {
                    photonView.RPC("RPC_BlindsPay", PhotonTargets.All);
                }
                break;
            case GameStateEnum.Preflop:
                if (PhotonNetwork.isMasterClient)
                {
                    photonView.RPC("RPC_DrawPlayerCard", PhotonTargets.All);
                }
                break;
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

        gameState = GameStateEnum.Blinds;
    }

    [PunRPC]
    private void RPC_BlindsPay()
    {
        foreach (Player p in players)
        {
            if (p.role == PlayerRole.BB || p.role == PlayerRole.SB)
            {
                p.PayBlinds();
            }
        }
        gameState = GameStateEnum.Preflop;
    }

    [PunRPC]
    private void RPC_DrawPlayerCard()
    {
        foreach (Player p in players)
        {
            p.DrawCard();
        }

        gameState = GameStateEnum.Flop;
    }
}
