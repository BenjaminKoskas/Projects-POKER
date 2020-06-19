using UnityEngine;

[CreateAssetMenu(fileName = "New Chip", menuName = "Chips")]
public class Chip : ScriptableObject
{
    public int value;

    public bool isDealer;
    public bool isBB;
    public bool isSB;

    public Sprite image;
}
