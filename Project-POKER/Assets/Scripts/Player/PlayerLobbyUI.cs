using UnityEngine;
using TMPro;

public class PlayerLobbyUI : MonoBehaviour
{
    public TMP_Text usernameField;
    public TMP_Text cashField;

    private void Start()
    {
        PlayerData data = PlayerSave.LoadPlayer();
        usernameField.text = data.username;
        cashField.text = data.cash + "$";
    }
}
