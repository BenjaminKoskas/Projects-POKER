using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetwork : MonoBehaviour
{
    public static PlayerNetwork Instance;

    public List<Vector2> playersPosition = new List<Vector2>();
    public List<GameObject> playersChipPosition = new List<GameObject>();
    public List<GameObject> playersPosPos = new List<GameObject>();

    public string PlayerName { get; private set; }
    
    public int PlayersInGame = 0;

    private PhotonView PhotonView;

    private void Awake()
    {
        Instance = this;

        PhotonView = GetComponent<PhotonView>();

        PlayerName = DBManager.username;

        playersPosition.Add(new Vector2(0, 513));
        playersPosition.Add(new Vector2(620, 450));
        playersPosition.Add(new Vector2(830, 188));
        playersPosition.Add(new Vector2(620, -150));
        playersPosition.Add(new Vector2(0, -250));
        playersPosition.Add(new Vector2(-629, -150));
        playersPosition.Add(new Vector2(-841, 188));
        playersPosition.Add(new Vector2(-612, 450));

        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Room")
        {
            if(PhotonNetwork.isMasterClient)
                MasterLoadedGame();
            else
                NonMasterLoadedGame();

            for (int i = 0; i < 8; i++)
            {
                playersChipPosition.Add(GameObject.Find("ChipPlayer" + (i + 1)));
                playersPosPos.Add(GameObject.Find("PlayerPos" + (i + 1)));
            }
        }
    }

    private void MasterLoadedGame()
    {
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient);
        PhotonView.RPC("RPC_LoadGameOthers", PhotonTargets.Others);
    }

    private void NonMasterLoadedGame()
    {
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient);
    }

    [PunRPC]
    private void RPC_LoadGameOthers()
    {
        PhotonNetwork.LoadLevel(4);
    }

    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        PlayersInGame++;
        if (PlayersInGame == PhotonNetwork.playerList.Length)
        {
            print("All players are in the game scene");
            PhotonView.RPC("RPC_CreatePlayer", PhotonTargets.AllBuffered);
        }
    }

    [PunRPC]
    private void RPC_CreatePlayer()
    {
        PhotonNetwork.Instantiate
        (
            "PlayerInRoom",
            new Vector3(0f, 0f, 0f),
            Quaternion.identity,
            0
        );
    }
}
