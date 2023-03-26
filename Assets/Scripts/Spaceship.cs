using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public partial class Spaceship : MonoBehaviour
{
    [Serializable]
    public struct Stats
    {
        public float Health;
        public float Armor;
        public float Recovery;
    }

    [SerializeField] private Stats baseStats;
    [SerializeField] private SimpleButton[] weaponSlots;
    [SerializeField] private SimpleButton[] moduleSlots;

    private Stats startStats;
    private Stats currentStats;
    private Weapon[] weapons;
    private Module[] modules;

    private SlotSelector slotSelector;
    private Spaceship opponent;
    private bool canChangeModules;
    private IEnumerator recoveringRoutine;

    [HideInInspector] public UnityEvent Died;
    [HideInInspector] public UnityEvent<string> ConfigurationUpdated;

    public void Initialize(
        SlotSelector slotSelector,
        Spaceship opponent,
        UnityEvent<Gamestate> stateChanged)
    {
        this.slotSelector = slotSelector;
        this.opponent = opponent;
        stateChanged.AddListener(OnStateChanged);
    }

    public bool IsAlive() => currentStats.Health > 0;

    public void Hit(float damage)
    {
        var armorLost = Mathf.Min(damage, currentStats.Armor);
        currentStats.Armor -= armorLost;
        var healthLost = Mathf.Min(damage - armorLost, currentStats.Health);
        currentStats.Health -= healthLost;
        if (!IsAlive()) Died?.Invoke();
    }

    private void Awake()
    {
        weapons = new Weapon[weaponSlots.Length];
        modules = new Module[moduleSlots.Length];
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            var slotIndex = i;
            weaponSlots[slotIndex].Clicked.AddListener(() => OnWeaponSlotClicked(slotIndex));
        }

        for (int i = 0; i < moduleSlots.Length; i++)
        {
            var slotIndex = i;
            moduleSlots[slotIndex].Clicked.AddListener(() => OnModuleSlotClicked(slotIndex));
        }

        ResetStats();
    }

    private void OnStateChanged(Gamestate state)
    {
        canChangeModules = state == Gamestate.Setup;
        switch (state)
        {
            case Gamestate.Battle:
                ResetStats();
                if (weapons is not null)
                    foreach (var weapon in weapons)
                        weapon?.StartShooting(opponent);
                StartRecovering();
                StartIdle();
                break;
            case Gamestate.Finished:
                if (weapons is not null)
                    foreach (var weapon in weapons)
                        weapon?.CancelShooting();
                CancelRecovering();
                break;
            case Gamestate.Setup:
                CancelIdle();
                break;
        }
    }

    private void OnWeaponSlotClicked(int slotIndex)
    {
        if (!canChangeModules) return;

        if (IsWeaponSlotEmpty(slotIndex))
            slotSelector.SelectWeapon(weapon => AddWeapon(slotIndex, weapon));
        else
            RemoveWeapon(slotIndex);
    }

    private void OnModuleSlotClicked(int slotIndex)
    {
        if (!canChangeModules) return;

        if (IsModuleSlotEmpty(slotIndex))
            slotSelector.SelectModule(module => AddModule(slotIndex, module));
        else
            RemoveModule(slotIndex);
    }

    private bool IsWeaponSlotEmpty(int slotIndex) => weapons[slotIndex] == null;
    private bool IsModuleSlotEmpty(int slotIndex) => modules[slotIndex] == null;

    private void AddWeapon(int slotIndex, Weapon weapon)
    {
        var newWeapon = Instantiate(weapon, weaponSlots[slotIndex].transform);
        newWeapon.transform.localPosition = Vector3.zero;
        weapons[slotIndex] = newWeapon;
    }

    private void AddModule(int slotIndex, Module module)
    {
        var newModule = Instantiate(module, moduleSlots[slotIndex].transform);
        newModule.transform.localPosition = Vector3.zero;
        modules[slotIndex] = newModule;
    }

    private void RemoveWeapon(int slotIndex)
    {
        Destroy(weapons[slotIndex].gameObject);
        weapons[slotIndex] = null;
        weaponSlots[slotIndex].gameObject.SetActive(true);
    }

    private void RemoveModule(int slotIndex)
    {
        Destroy(modules[slotIndex].gameObject);
        modules[slotIndex] = null;
        moduleSlots[slotIndex].gameObject.SetActive(true);
    }

    private void ResetStats()
    {
        currentStats = baseStats;
        var increaseRecoverySum = 0;
        var decreaseCooldownSum = 0;
        foreach (var module in modules)
        {
            if (module is null) continue;
            switch (module.ModuleType)
            {
                case Module.Type.Health:
                    currentStats.Health += module.Value;
                    break;
                case Module.Type.Armor:
                    currentStats.Armor += module.Value;
                    break;
                case Module.Type.Recovery:
                    increaseRecoverySum += module.Value;
                    break;
                case Module.Type.Cooldown:
                    decreaseCooldownSum += module.Value;
                    break;
            }
        }

        currentStats.Recovery = Percents.Add(baseStats.Recovery, increaseRecoverySum);
        foreach (var weapon in weapons)
            if (weapon is not null)
                weapon.SetDecreasedCooldown(decreaseCooldownSum);
        startStats = currentStats;
        ConfigurationUpdated?.Invoke(GetConfigurationInfo());
    }

    private void StartRecovering()
    {
        CancelRecovering();
        recoveringRoutine = RecoveringLoop();
        StartCoroutine(recoveringRoutine);
    }

    private void CancelRecovering()
    {
        if (recoveringRoutine is not null)
            StopCoroutine(recoveringRoutine);
    }

    private IEnumerator RecoveringLoop()
    {
        var oneSec = new WaitForSeconds(1f);
        while (IsAlive())
        {
            yield return oneSec;
            currentStats.Armor = Mathf.Min(currentStats.Armor + currentStats.Recovery, startStats.Armor);
        }
    }
    
    private string GetConfigurationInfo()
    {
        var info = $"{name}\n" +
            $"Health: {startStats.Health}\n" + 
            $"Armor: {startStats.Armor} (+{startStats.Recovery})";
        for (int i = 0; i < weapons.Length; i++)
            info += $"\nWeapon slot {i + 1}: {GetWeaponInfo(i)}";
        for (int i = 0; i < modules.Length; i++)
            info += $"\nModule slot {i + 1}: {GetModuleInfo(i)}";
        return info;
    }

    private string GetWeaponInfo(int index) => weapons[index]?.GetInfo() ?? "<empty>";
    
    private string GetModuleInfo(int index) => modules[index]?.GetInfo() ?? "<empty>";
}
