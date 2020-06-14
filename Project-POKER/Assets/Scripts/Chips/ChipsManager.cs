using System.Collections.Generic;
using UnityEngine;

public class ChipsManager : MonoBehaviour
{
    public Dictionary<string, Chip> chips = new Dictionary<string, Chip>();

    public static ChipsManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        Chip[] sc_chips = Utils.GetAllInstances<Chip>();
        foreach (Chip chip in sc_chips)
        {
            chips.Add(chip.percentageValue * 100 + "%", chip);
            Debug.Log(chip.percentageValue * 100 + "%");
        }
    }
}
