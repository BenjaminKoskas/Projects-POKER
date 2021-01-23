using System.Collections.Generic;
using UnityEngine;

public enum GameStateEnum
{
    WaitForPlayers, Distribution, Blinds, Preflop, Flop, Turn, River
}

public class GameManager : MonoBehaviour, IPunObservable
{
    public static GameManager Instance;
    public List<Card> deck = new List<Card>();

    public GameStateEnum gameState;

    public bool playerPlayed = true;
    public bool playerDrawed = true;

    public int numberOfPlayerDraw = 0;
    public int playIndex = 0;
    public int rebet;

    public int mainStack;
    public int minBet;

    public List<Player> players = new List<Player>();
    public List<Player> ordererdPlayers = new List<Player>();

    private PhotonView photonView;

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
        if (!PhotonNetwork.isMasterClient) { return; }
        if (PlayerNetwork.Instance == null)
            return;

        bool allPlayersInGame = PlayerNetwork.Instance.PlayersInGame == PhotonNetwork.playerList.Length;

        if (players.Count < PhotonNetwork.playerList.Length)
        {
            foreach (Player p in FindObjectsOfType<Player>())
            {
                if (!players.Contains(p))
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
                    photonView.RPC("RPC_Distribution", PhotonTargets.All);
                }
                break;
            case GameStateEnum.Blinds:
                if (PhotonNetwork.isMasterClient)
                {
                    photonView.RPC("RPC_Blinds", PhotonTargets.MasterClient);
                }
                break;
            case GameStateEnum.Preflop:
                if (PhotonNetwork.isMasterClient)
                {
                    photonView.RPC("RPC_Preflop", PhotonTargets.MasterClient);
                }
                break;
            case GameStateEnum.Flop:
                break;
            case GameStateEnum.Turn:
                break;
            case GameStateEnum.River:
                break;
        }
    }

    [PunRPC]
    private void RPC_Distribution()
    {
        foreach (Player p in players)
        {
            switch (p.index)
            {
                case 1:
                    p.SetRole(PlayerRole.Dealer);
                    break;
                case 2:
                    p.SetRole(PhotonNetwork.playerList.Length == 2 ? PlayerRole.BB : PlayerRole.SB);
                    break;
                case 3:
                    p.SetRole(PlayerRole.BB);
                    break;
                case 4:
                    if(PhotonNetwork.room.PlayerCount == 4)
                        p.SetRole(PlayerRole.CutOff);
                    else if (PhotonNetwork.room.PlayerCount == 5)
                        p.SetRole(PlayerRole.HiJack);
                    else if (PhotonNetwork.room.PlayerCount > 5)
                        p.SetRole(PlayerRole.UTG);
                    break;
                case 5:
                    if (PhotonNetwork.room.PlayerCount == 5)
                        p.SetRole(PlayerRole.CutOff);
                    else if (PhotonNetwork.room.PlayerCount > 5)
                        p.SetRole(PlayerRole.UTG1);
                    break;
                case 6:
                    p.SetRole(PlayerRole.UTG2);
                    break;
                case 7:
                    p.SetRole(PlayerRole.HiJack);
                    break;
                case 8:
                    p.SetRole(PlayerRole.CutOff);
                    break;
            }
        }

        if(FindPlayerByRole(PlayerRole.UTG) != null)
            ordererdPlayers.Add(FindPlayerByRole(PlayerRole.UTG));
        if (FindPlayerByRole(PlayerRole.UTG1) != null)
            ordererdPlayers.Add(FindPlayerByRole(PlayerRole.UTG1));
        if (FindPlayerByRole(PlayerRole.UTG2) != null)
            ordererdPlayers.Add(FindPlayerByRole(PlayerRole.UTG2));
        if (FindPlayerByRole(PlayerRole.HiJack) != null)
            ordererdPlayers.Add(FindPlayerByRole(PlayerRole.HiJack));
        if (FindPlayerByRole(PlayerRole.CutOff) != null)
            ordererdPlayers.Add(FindPlayerByRole(PlayerRole.CutOff));
        if (FindPlayerByRole(PlayerRole.Dealer) != null)
            ordererdPlayers.Add(FindPlayerByRole(PlayerRole.Dealer));
        if (FindPlayerByRole(PlayerRole.SB) != null)
            ordererdPlayers.Add(FindPlayerByRole(PlayerRole.SB));
        if (FindPlayerByRole(PlayerRole.BB) != null)
            ordererdPlayers.Add(FindPlayerByRole(PlayerRole.BB));

        gameState = GameStateEnum.Blinds;
    }

    [PunRPC]
    private void RPC_Blinds()
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
    private void RPC_Preflop()
    {
        if (numberOfPlayerDraw < PhotonNetwork.room.PlayerCount)
        {
            if (playerDrawed)
            {
                playerDrawed = false;
                ordererdPlayers[numberOfPlayerDraw].DrawCard();
            }
        }

        numberOfPlayerDraw = Mathf.Clamp(numberOfPlayerDraw, 0, PhotonNetwork.room.PlayerCount);

        if (numberOfPlayerDraw == PhotonNetwork.room.PlayerCount)
        {
            minBet = (int)PhotonNetwork.room.CustomProperties["BB"];
            PreFlopGameLoop();
        }
    }

    public void PreFlopGameLoop()
    {
        bool canNextState = false;
        if (playIndex == PhotonNetwork.room.PlayerCount)
            playIndex = 0;
        if (playIndex != PhotonNetwork.room.PlayerCount)
        {
            if (playerPlayed)
            {
                int x= 0;
                foreach (Player p in ordererdPlayers)
                {
                    if (p.betStack == minBet)
                        x++;
                }

                if (x == ordererdPlayers.Count)
                {
                    canNextState = true;
                }
                else
                {
                    playerPlayed = false;
                    ordererdPlayers[playIndex].SetStatus(PlayerStatus.Turn);
                }
            }
        }

        if(canNextState)
        {
            playIndex = 0;
            playerPlayed = true;

            gameState = GameStateEnum.Flop;
        }
    }

    public List<Player> FindPlayersWhoChecked()
    {
        List<Player> ps = new List<Player>();

        foreach (Player p in ordererdPlayers)
        {
            if (p.hasCheck)
            {
                ps.Add(p);
            }
        }

        return ps;
    }

    public Player FindPlayerByRole(PlayerRole role)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].role == role)
                return players[i];
        }

        return null;
    }

    public Player FindPlayerBefore(Player player)
    {
        int indexBefore;
        if (ordererdPlayers.FindIndex(x => x == player) == 0)
            indexBefore = ordererdPlayers.Count - 1;
        else
            indexBefore = ordererdPlayers.FindIndex(x => x == player) - 1;

        foreach (Player p in ordererdPlayers)
        {
            if (p != player)
            {
                if (ordererdPlayers.FindIndex(x => x == p) == indexBefore)
                    return p;
            }
        }

        return null;
    }

    public Player FindFirstPlayerToPlay()
    {
        return ordererdPlayers[0];
    }

    public Player FindLastPlayerToPlay()
    {
        return ordererdPlayers[ordererdPlayers.Count - 1];
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext((int)gameState);
            stream.SendNext(rebet);
            stream.SendNext(minBet);
            stream.SendNext(numberOfPlayerDraw);
            stream.SendNext(playerDrawed);
            stream.SendNext(mainStack);
            stream.SendNext(playIndex);
            stream.SendNext(playerPlayed);
        } 
        else if (stream.isReading)
        {
            gameState = (GameStateEnum)(int) stream.ReceiveNext();
            rebet = (int) stream.ReceiveNext();
            minBet = (int) stream.ReceiveNext();
            numberOfPlayerDraw = (int) stream.ReceiveNext();
            playerDrawed = (bool) stream.ReceiveNext();
            mainStack = (int) stream.ReceiveNext();
            playIndex = (int) stream.ReceiveNext();
            playerPlayed = (bool) stream.ReceiveNext();
        }
    }
}
