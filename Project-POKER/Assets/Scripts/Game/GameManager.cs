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

    public bool playerPlayed = true;
    public bool hasDrawCard = false;

    public int numberOfPlayerPlayed = 0;

    private PhotonView photonView;

    private List<Player> players = new List<Player>();
    private List<Player> ordererdPlayers = new List<Player>();

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
            switch (p.index)
            {
                case 1:
                    p.SetRole(PlayerRole.Dealer);
                    ordererdPlayers.Add(p);
                    break;
                case 2:
                    p.SetRole(PhotonNetwork.room.PlayerCount == 2 ? PlayerRole.BB : PlayerRole.SB);
                    ordererdPlayers.Add(p);
                    break;
                case 3:
                    p.SetRole(PlayerRole.SB);
                    ordererdPlayers.Add(p);
                    break;
                case 4:
                    if(PhotonNetwork.room.PlayerCount == 4)
                        p.SetRole(PlayerRole.CutOff);
                    else if (PhotonNetwork.room.PlayerCount == 5)
                        p.SetRole(PlayerRole.HiJack);
                    else if (PhotonNetwork.room.PlayerCount > 5)
                        p.SetRole(PlayerRole.UTG);

                    ordererdPlayers.Add(p);
                    break;
                case 5:
                    if (PhotonNetwork.room.PlayerCount == 5)
                        p.SetRole(PlayerRole.CutOff);
                    else if (PhotonNetwork.room.PlayerCount > 5)
                        p.SetRole(PlayerRole.UTG1);

                    ordererdPlayers.Add(p);
                    break;
                case 6:
                    p.SetRole(PlayerRole.UTG2);
                    ordererdPlayers.Add(p);
                    break;
                case 7:
                    p.SetRole(PlayerRole.HiJack);
                    ordererdPlayers.Add(p);
                    break;
                case 8:
                    p.SetRole(PlayerRole.CutOff);
                    ordererdPlayers.Add(p);
                    break;
            }
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
        if (!hasDrawCard)
        {
            foreach (Player p in players)
            {
                p.DrawCard();
            }
        }

        hasDrawCard = true;

        if (numberOfPlayerPlayed != PhotonNetwork.room.PlayerCount)
        {
            if (playerPlayed)
            {
                playerPlayed = false;
                if (ordererdPlayers[numberOfPlayerPlayed].photonView.isMine)
                    ordererdPlayers[numberOfPlayerPlayed].SetStatus(PlayerStatus.Turn);
            }
        }
        else
        {
            numberOfPlayerPlayed = 0;
            playerPlayed = true;

            gameState = GameStateEnum.Flop;
        }
    }

    private Player FindPlayerByRole(PlayerRole role)
    {
        foreach (Player p in players)
        {
            if (p.role == role)
            {
                return p;
            }
        }

        return null;
    }
}
