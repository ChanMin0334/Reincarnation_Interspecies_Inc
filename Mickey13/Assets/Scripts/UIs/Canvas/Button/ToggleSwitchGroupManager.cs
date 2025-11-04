using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSwitchGroupManager : MonoBehaviour
{
    [Header("Start value")]
    [SerializeField] private ToggleSwitch initToggleSwitch;

    [Header("Toggle Options")]
    [SerializeField] private bool allCanBeToggledOff; // 모든 토글 버튼 ON/OFF

    private List<ToggleSwitch> _toggleSwitches = new List<ToggleSwitch>(); // 한번에 관리할 토글 버튼 리스트

    private void Awake()
    {
        ToggleSwitch[] toggleSwitches = GetComponentsInChildren<ToggleSwitch>(); // 자식 요소의 모든 토글 버튼 등록
        foreach(var toggleSwitch in toggleSwitches)
        {
            RegisterToggleButtonToGroup(toggleSwitch);
        }
    }

    private void RegisterToggleButtonToGroup(ToggleSwitch toggleSwitch)
    {
        if (_toggleSwitches.Contains(toggleSwitch))
            return;

        _toggleSwitches.Add(toggleSwitch);

        toggleSwitch.SetupForManager(this);
    }

    private void Start()
    {
        bool areAllToggleOff = true;
        foreach(var button in _toggleSwitches)
        {
            if (!button.CurrentValue)
                continue;

            areAllToggleOff = false;
            break;
        }

        if (!areAllToggleOff || allCanBeToggledOff)
            return;

        if (initToggleSwitch != null)
            initToggleSwitch.ToggleByGroupManager(true);
        else
            _toggleSwitches[0].ToggleByGroupManager(true);
    }

    public void ToggleGroup(ToggleSwitch toggleSwitch)
    {
        if (_toggleSwitches.Count <= 1) 
            return;

        if(allCanBeToggledOff && toggleSwitch.CurrentValue)
        {
            foreach(var button in _toggleSwitches)
            {
                if (button == null)
                    continue;

                button.ToggleByGroupManager(false);
            }
        }
        else
        {
            foreach(var button in _toggleSwitches)
            {
                if(button == null)
                    continue;
                if(button == toggleSwitch)
                    button.ToggleByGroupManager(true);
                else
                    button.ToggleByGroupManager(false);
            }
        }
    }
}