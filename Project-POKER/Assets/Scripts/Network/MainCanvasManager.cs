using UnityEngine;

public class MainCanvasManager : MonoBehaviour
{
    public static MainCanvasManager Instance;

    [SerializeField] 
    private LobbyCanvas _lobbyCanvas;
    public LobbyCanvas LobbyCanvas => _lobbyCanvas;

    private void Awake()
    {
        Instance = this;
    }
}
