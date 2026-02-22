using UnityEngine;

public class SelectedSongManager : MonoBehaviour
{
    public static SelectedSongManager Instance { get; private set; }

    [Header("Datos de la Canción Seleccionada")]
    public SongListarResponse selectedSong;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}