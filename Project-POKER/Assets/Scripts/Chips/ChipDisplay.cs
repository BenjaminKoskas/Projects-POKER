using UnityEngine;

public class ChipDisplay : MonoBehaviour
{
    public Chip chip;
    [HideInInspector]
    public float percentageValue;

    private void Start()
    {
        percentageValue = chip.percentageValue;
    } 
}
