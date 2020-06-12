using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour
{
    [SerializeField] 
    private TMP_Text _roomNameText;
    private TMP_Text RoomNameText => _roomNameText;

    [SerializeField]
    private TMP_Text _betText;
    private TMP_Text BetText => _betText;

    [SerializeField]
    private TMP_Text _gameText;
    private TMP_Text GameText => _gameText;

    [SerializeField]
    private TMP_Text _typeText;
    private TMP_Text TypeText => _typeText;

    [SerializeField]
    private TMP_Text _playerText;
    private TMP_Text PlayerText => _playerText;

    public string RoomName { get; private set; }
    public string Bet { get; private set; }
    public string Game { get; private set; }
    public string Type { get; private set; }
    public string Players { get; private set; }
    public bool Updated { get; set; }

    private void Start()
    {
        GameObject lobbyCanvasObj = MainCanvasManager.Instance.LobbyCanvas.gameObject;
        if (lobbyCanvasObj == null) { return; }

        LobbyCanvas lobbyCanvas = lobbyCanvasObj.GetComponent<LobbyCanvas>();

        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => lobbyCanvas.OnClickJoinRoom(RoomNameText.text));
    }

    private void OnDestroy()
    {
        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
    }

    public void SetRoomNameText(string text)
    {
        RoomName = text;
        RoomNameText.text = RoomName;
    }

    public void SetBetText(string text)
    {
        Bet = text;
        BetText.text = Bet;
    }

    public void SetGameText(string text)
    {
        Game = text;
        GameText.text = Game;
    }

    public void SetTypeText(string text)
    {
        Type = text;
        TypeText.text = Type;
    }

    public void SetPlayerText(string text)
    {
        Players = text;
        PlayerText.text = Players;
    }
}
