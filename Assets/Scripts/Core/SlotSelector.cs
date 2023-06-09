﻿using System;
using UnityEngine;

public class SlotSelector : MonoBehaviour
{
    [SerializeField] private GameObject weaponContainer;
    [SerializeField] private GameObject moduleContainer;
    [SerializeField] private Weapon[] weapons;
    [SerializeField] private Module[] modules;

    private Action<Weapon> weaponSelected;
    private Action<Module> moduleSelected;

    private Vector2 weaponButtonSize = new (2.5f, 0.5f);
    private Vector2 moduleButtonSize = new (0.5f, 0.5f);
    
    private void Awake()
    {
        foreach (var weapon in weapons)
        {
            var button = CreateButton(weapon.transform, weaponContainer.transform, weaponButtonSize);
            button.Clicked.AddListener(() =>
            {
                weaponSelected?.Invoke(weapon);
                Hide();
            });
        }

        foreach (var module in modules)
        {
            var button = CreateButton(module.transform, moduleContainer.transform, moduleButtonSize);
            button.Clicked.AddListener(() =>
            {
                moduleSelected?.Invoke(module);
                Hide();;
            });
        }
            
        Hide();
    }

    public void SelectWeapon(Action<Weapon> onWeaponSelected)
    {
        weaponSelected = onWeaponSelected;
        weaponContainer.SetActive(true);
        moduleContainer.SetActive(false);
    }
    
    public void SelectModule(Action<Module> onModuleSelected)
    {
        moduleSelected = onModuleSelected;
        moduleContainer.SetActive(true);
        weaponContainer.SetActive(false);
    }

    public void Hide()
    {
        weaponContainer.SetActive(false);
        moduleContainer.SetActive(false);
        weaponSelected = null;
        moduleSelected = null;
    }

    private SimpleButton CreateButton(Transform parent, Transform container, Vector2 buttonSize)
    {
        var buttonGO = new GameObject();
        buttonGO.name = $"{parent.gameObject.name} button";
        buttonGO.transform.parent = container;
        buttonGO.transform.position = parent.transform.position;
        buttonGO.AddComponent<BoxCollider2D>().size = buttonSize;
        return buttonGO.AddComponent<SimpleButton>();
    }
}