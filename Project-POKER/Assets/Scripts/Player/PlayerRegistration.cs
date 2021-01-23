using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class PlayerRegistration : MonoBehaviour
{
    public TMP_InputField nameField;

    public Button submitButton;

    private void Start()
    {
        /*
        if(PlayerSave.SaveExist())
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        */
    }

    private void Update()
    {
        VerifyInputs();
    }

    public void CallRegister()
    {
        Random r = new Random();
        PlayerSave.SavePlayer(nameField.text, 10000, "User" + r.Next(0, 10000));

        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void VerifyInputs()
    {
        submitButton.interactable = (nameField.text.Length >= 6);
    }
}
