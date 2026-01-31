using UnityEngine;
using TMPro;

public class SongItem : MonoBehaviour
{
    [Header("Los 3 Renglones de la UI")]
    public TMP_Text txtTituloArtista;   // Renglón 1: Título - Artista
    public TMP_Text txtTonalidadTempo;  // Renglón 2: Tonalidad | Tempo
    public TMP_Text txtDetalles;        // Renglón 3: Género | Duración | Modo

    public void Setup(SongListarResponse data)
    {
        // 1. Título - Artista
        if (txtTituloArtista != null)
            txtTituloArtista.text = $"{data.title} - {data.artist_name}";

        // 2. Tonalidad: C#m | Tempo: 54 BPM
        if (txtTonalidadTempo != null)
            txtTonalidadTempo.text = $"Tonalidad: {data.musical_key} | Tempo: {data.tempo} BPM";

        // 3. Género: Clásico | Duración: 3.5 min | Modo: Piano
        if (txtDetalles != null)
        {
            // Convertimos segundos a minutos con 1 decimal
            // La fórmula es: $minutos = \frac{segundos}{60}$
            float duracionMinutos = data.duration / 60f; 
            
            txtDetalles.text = $"Género: {data.musical_genre} | " +
                               $"Duración: {duracionMinutos:F1} min | " +
                               $"Modo: {data.mode}";
        }
    }
}