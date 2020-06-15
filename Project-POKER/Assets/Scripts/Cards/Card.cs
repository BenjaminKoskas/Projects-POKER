using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards")]
public class Card : ScriptableObject
{
    public CardEnum card;
    public CardTypeEnum type;
    public CardValueEnum value;

    public Sprite background;

    public bool isHearts()
    {
        if (type.Equals(CardTypeEnum.HEARTS))
            return true;
        return false;
    }
    public bool isClubs()
    {
        if (type.Equals(CardTypeEnum.CLUBS))
            return true;
        return false;
    }
    public bool isSpades()
    {
        if (type.Equals(CardTypeEnum.SPADES))
            return true;
        return false;
    }
    public bool isDiamonds()
    {
        if (type.Equals(CardTypeEnum.DIAMONDS))
            return true;
        return false;
    }

    public bool isStrongerThan(Card _card)
    {
        if (value > _card.value)
        {
            return true;
        }

        return false;
    }

}
