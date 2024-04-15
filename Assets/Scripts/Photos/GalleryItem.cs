using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class GalleryItem : MonoBehaviour
{
    [SerializeField] RawImage photo;
    [SerializeField] TMP_Text dateText;

    public Photo PhotoInfo;

    public void Initialize(Photo info)
    {
        PhotoInfo = info;
        dateText.text = info.TimeTaken.ToString("dd/MM/yyyy") + "\n" + info.TimeTaken.ToString("T");

        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false, true);
        byte[] bytes = File.ReadAllBytes(Application.dataPath + PhotoInfo.TextureSaveName);
        texture.LoadImage(bytes);
        photo.texture = texture;
    }
}
