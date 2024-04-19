using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : Common.DesignPatterns.Singleton<PlayerUIManager>
{
    [SerializeField] GameObject hudObject;
    [SerializeField] GameObject controlsObject;

    public ControlsManager ControlsManager;
    public bool bIsHUDHidden { get; private set; } = false;

    public void SetHideHUD(bool shouldHide, bool keepShowingControls = true)
    {
        bIsHUDHidden = shouldHide;
        hudObject.SetActive(!shouldHide);
        controlsObject.SetActive(!shouldHide || keepShowingControls);
    }
}
