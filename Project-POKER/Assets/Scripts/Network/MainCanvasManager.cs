using UnityEngine;

public class MainCanvasManager : MonoBehaviour
{
    public static MainCanvasManager Instance;

    [SerializeField] 
    private LobbyCanvas _lobbyCanvas;
    public LobbyCanvas LobbyCanvas => _lobbyCanvas;

    [SerializeField] 
    private CurrentRoomCanvas _currentRoomCanvas;
    public CurrentRoomCanvas CurrentRoomCanvas => _currentRoomCanvas;

    [SerializeField]
    private GameObject _createRoomUI;
    public GameObject CreateRoomUI => _createRoomUI;

    private void Awake()
    {
        Instance = this;
    }
}
