using UnityEngine;
using UnityEngine.UI;

public class VolumeText : MonoBehaviour
{
    private Text txt;
    [SerializeField] private string volumeTypeName; // sound or music

    private void Awake()
    {
        txt = GetComponent<Text>();
    }

    private void Update()
    {
        UpdateVolume();
    }
    private void UpdateVolume()
    {
        float volumeValue = PlayerPrefs.GetFloat(volumeTypeName + "Volume") * 100;
        txt.text = volumeTypeName.ToUpper() + ": %" + volumeValue.ToString();
    }
}
