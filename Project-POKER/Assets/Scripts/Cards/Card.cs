using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards")]
public class Card : ScriptableObject
{
    public CardEnum card;
    public CardTypeEnum type;
    public CardValueEnum value;

    public Sprite background;

    public bool isCoeur()
    {
        if (type.Equals(CardTypeEnum.COEUR))
            return true;
        return false;
    }
    public bool isTrefle()
    {
        if (type.Equals(CardTypeEnum.TREFLE))
            return true;
        return false;
    }
    public bool isPique()
    {
        if (type.Equals(CardTypeEnum.PIQUE))
            return true;
        return false;
    }
    public bool isCarreau()
    {
        if (type.Equals(CardTypeEnum.CARREAU))
            return true;
        return false;
    }

    public bool isStronger(Card _card)
    {
        if (value > _card.value)
        {
            return true;
        }

        return false;
    }
}
