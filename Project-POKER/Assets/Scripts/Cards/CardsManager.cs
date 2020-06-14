using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour
{
    public Dictionary<string, Card> cards = new Dictionary<string, Card>();

    public static CardsManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        Card[] sc_cards = Utils.GetAllInstances<Card>();
        foreach (Card card in sc_cards)
        {
            cards.Add(card.type.GetDescription() + card.value.GetDescription(), card);
        }
    }
}
