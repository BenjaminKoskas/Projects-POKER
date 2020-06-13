using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Card card;

    private void Start()
    {
        GetComponent<Image>().sprite = card.background;

        TMP_Text[] text = GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text tmpText in text)
        {
            string s = card.value.GetDescription();
            tmpText.text = s;
        }
    }
}
