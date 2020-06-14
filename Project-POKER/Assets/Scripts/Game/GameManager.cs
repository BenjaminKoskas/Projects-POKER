using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
    }

    private void Update()
    {
        if (PlayerNetwork.Instance == null)
            return;

        if (players.Count < PhotonNetwork.playerList.Length)
        {
            foreach (Player p in FindObjectsOfType<Player>())
            {
                if(!players.Contains(p))
                    players.Add(p);
                Debug.Log(p.gameObject.name);
            }
        }

        if (PlayerNetwork.Instance.PlayersInGame == PhotonNetwork.playerList.Length && players.Count == PhotonNetwork.playerList.Length && gameState.Equals(GameStateEnum.WaitForPlayers))
        {
            gameState = GameStateEnum.ChooseRole;
            
            if (PhotonNetwork.isMasterClient)
            {
                photonView.RPC("RPC_GameStartDefineRole", PhotonTargets.All);
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
}
