using UnityEngine;
using System.Collections.Generic;

public class ShowRepertorio : MonoBehaviour
{
    [Header("Configuración UI")]
    [SerializeField] private GameObject songBoxPrefab; // Tu molde café
    [SerializeField] private Transform songBoxContainer; // El 'Content' del ScrollView

    [Header("Servicios")]
    [SerializeField] private AuthService authService;

    private void Start()
    {
        CargarDatos();
    }

    private void CargarDatos()
    {
        // Limpiamos basura previa
        foreach (Transform child in songBoxContainer) Destroy(child.gameObject);

        StartCoroutine(authService.GetSongs(
            onSuccess: (json) => {
                SongListWrapper wrapper = JsonUtility.FromJson<SongListWrapper>(json);
                
                foreach (var song in wrapper.songs)
                {
                    // Creamos la cajita dinámicamente
                    GameObject newBox = Instantiate(songBoxPrefab, songBoxContainer);
                    
                    // Le pasamos los datos
                    SongItem item = newBox.GetComponent<SongItem>();
                    if (item != null) item.Setup(song);
                }
            },
            onError: (err) => Debug.LogError("Error al cargar canciones: " + err)
        ));
    }
}