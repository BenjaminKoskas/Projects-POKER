using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Card card;

    private void Start()
    {
        if (card != null)
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

    public void SetCard(Card _card)
    {
        card = _card;

        GetComponent<Image>().sprite = card.background;

        TMP_Text[] text = GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text tmpText in text)
        {
            string s = card.value.GetDescription();
            tmpText.text = s;
        }
    }
}
