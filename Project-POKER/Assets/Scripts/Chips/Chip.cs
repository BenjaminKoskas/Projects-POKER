using UnityEngine;

[CreateAssetMenu(fileName = "New Chip", menuName = "Chips")]
public class Chip : ScriptableObject
{
    public float percentageValue;

    public bool isDealer;
    public bool isBB;
    public bool isSB;

    public Sprite image;
}
