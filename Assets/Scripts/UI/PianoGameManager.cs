using UnityEngine;

public class PianoGameManager : MonoBehaviour
{
    void Start()
    {
        if (SelectedSongManager.Instance != null && SelectedSongManager.Instance.selectedSong != null)
        {
            var song = SelectedSongManager.Instance.selectedSong;

            // Aquí tienes acceso a TODO
            Debug.Log($"<color=green>[JUEGO]</color> Cargando canción: {song.title}");
            Debug.Log($"Detalles: {song.artist_name} | {song.musical_genre} | {song.tempo} BPM");
            Debug.Log($"Ruta del archivo: {song.file_path}");
        }
        else
        {
            Debug.LogError("No se encontró ninguna canción seleccionada.");
        }
    }
}