using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [System.Serializable]
    public class UpgradeOption
    {
        public string id;       // e.g., "FR", "HP", "MG"
        public string name;     // Friendly name for UI
        public bool isEpic;     // Epic vs Common weighting
        public bool isOffensive; // Attack vs Defense categories
    }

    [Header("Pool Settings")]
    public List<UpgradeOption> allUpgrades = new List<UpgradeOption>();
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Main logic: Generates 3 unique options based on technical specs
    public List<UpgradeOption> GetUpgradeChoices()
    {
        List<UpgradeOption> pool = new List<UpgradeOption>();
        List<UpgradeOption> selection = new List<UpgradeOption>();

        foreach (var up in allUpgrades)
        {
            // 1. Filter out "Consumed" components (Logic for Phase 2)
            // 2. Filter out Max Level items (Level 5)
            if (IsMaxLevel(up.id)) continue;

            // 3. Calculate Weighting
            float weight = up.isEpic ? 20f : 100f; //

            // 4. Apply Sticky Bias (2.0x multiplier if owned)
            if (GetLevel(up.id) > 0) weight *= 2.0f;

            // Add to weighted pool
            for (int i = 0; i < (int)weight; i++) pool.Add(up);
        }

        // Pick 3 unique items
        while (selection.Count < 3 && pool.Count > 0)
        {
            UpgradeOption choice = pool[Random.Range(0, pool.Count)];
            if (!selection.Contains(choice)) selection.Add(choice);
        }

        return selection;
    }

    private int GetLevel(string id)
    {
        // Check Offensive (AutoShooter) or Defensive (PlayerController)
        if (id == "FR") return GameManager.Instance.gunScript.currentWeaponLevel;
        if (PlayerController.Instance.defenseLevels.ContainsKey(id))
            return PlayerController.Instance.defenseLevels[id];
        return 0;
    }

    private bool IsMaxLevel(string id)
    {
        return GetLevel(id) >= 5; //
    }
}