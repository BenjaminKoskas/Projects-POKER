using System.Collections.Generic;
using UnityEngine;

public class ChipsManager : MonoBehaviour
{
    public Dictionary<string, Chip> chips = new Dictionary<string, Chip>();

    public static ChipsManager Instance;

    public GameObject chipPrefab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        Chip[] sc_chips = Utils.GetAllInstances<Chip>("Chips");
        foreach (Chip chip in sc_chips)
        {
            if(chip.isDealer)
                chips.Add("Dealer", chip);
            else if (chip.isBB)
                chips.Add("BB", chip);
            else if (chip.isSB)
                chips.Add("SB", chip);
            else
                chips.Add(chip.percentageValue * 100 + "%", chip);
        }
    }
}
