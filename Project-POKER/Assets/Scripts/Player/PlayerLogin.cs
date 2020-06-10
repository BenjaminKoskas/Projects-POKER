using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerLogin : MonoBehaviour
{
    public TMP_InputField nameField;
    public TMP_InputField passwordField;

    public Button submitButton;

    public void CallLogin()
    {
        StartCoroutine(LoginPlayer());
    }

    IEnumerator LoginPlayer()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", nameField.text);
        form.AddField("password", passwordField.text);

        UnityWebRequest request = UnityWebRequest.Post("http://localhost:8888/sqlconnect/login.php", form);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.downloadHandler.text[0] == '0')
        {
            Debug.Log("User Logged Succesfully.");

            DBManager.username = nameField.text;
            DBManager.cash = int.Parse(request.downloadHandler.text.Split('\t')[1]);

            UnityEngine.SceneManagement.SceneManager.LoadScene(3);
        }
        else
        {
            Debug.Log("User login failed. Error #" + request.downloadHandler.text);
        }
    }

    public void VerifyInputs()
    {
        submitButton.interactable = (nameField.text.Length >= 8 && passwordField.text.Length >= 8);
    }
}
