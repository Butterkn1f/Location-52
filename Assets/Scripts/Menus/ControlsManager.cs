using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ControlsManager : MonoBehaviour
{
    [SerializeField] List<ControlTexts> controlTexts;

    public void SetControlActive(ControlsType type, bool active)
    {
        var text = controlTexts.FirstOrDefault(x => x.Type == type);
        if (text.Text)
            text.Text.SetActive(active);
    }
}

public enum ControlsType
{
    Grid,
    CameraUnAds,
    CameraAds,
    CameraGallery,
    Anomalyser,
    GridBag,
    Flashlight,
    Noisemaker,
    Trap
}

[System.Serializable]
public struct ControlTexts
{
    public ControlsType Type;
    public GameObject Text;
}
