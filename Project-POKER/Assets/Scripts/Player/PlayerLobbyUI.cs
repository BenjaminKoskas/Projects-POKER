using UnityEngine;
using TMPro;

public class PlayerLobbyUI : MonoBehaviour
{
    public TMP_Text usernameField;
    public TMP_Text cashField;

    private void Start()
    {
        if (DBManager.LoggedIn)
        {
            usernameField.text = DBManager.username;
            cashField.text = DBManager.cash + "$";
        }
    }
}
