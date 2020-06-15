using UnityEngine;
using UnityEngine.UI;

public class ChipDisplay : MonoBehaviour
{
    public Chip chip;
    [HideInInspector]
    public float percentageValue;

    private void Start()
    {
        if (chip != null)
        {
            percentageValue = chip.percentageValue;

            GetComponent<Image>().sprite = chip.image;
        }
    }

    public void SetChip(Chip _chip)
    {
        chip = _chip;

        percentageValue = chip.percentageValue;

        GetComponent<Image>().sprite = chip.image;
    }
}
