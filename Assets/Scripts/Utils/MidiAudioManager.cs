using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class MidiAudioManager : MonoBehaviour
{
    [Header("Conexiones")]
    public UDPReceiver udpReceiver;
    
    [Header("Ajustes de Sonido")]
    [Range(1f, 20f)] public float volumeBoost = 10.0f; 
    [Range(1f, 4f)] public float velocityCurve = 2.2f; 
    public int poolSize = 40;

    private Dictionary<int, AudioClip> pianoSamples = new Dictionary<int, AudioClip>();
    private List<int> availableMidiNotes = new List<int>();
    private List<AudioSource> audioPool = new List<AudioSource>();
    private Dictionary<int, AudioSource> activeNotes = new Dictionary<int, AudioSource>();
    private HashSet<int> sustainedNotes = new HashSet<int>();
    private bool isPedalDown = false;

    private Dictionary<string, int> noteOffsets = new Dictionary<string, int>() {
        {"c", 0}, {"c#", 1}, {"d", 2}, {"d#", 3}, {"e", 4}, {"f", 5},
        {"f#", 6}, {"g", 7}, {"g#", 8}, {"a", 9}, {"a#", 10}, {"b", 11}
    };

    void Start()
    {
        // 1. Carga Inteligente de Samples (c2, c#2, etc.)
        AudioClip[] loadedClips = Resources.LoadAll<AudioClip>("notes");

        foreach (var clip in loadedClips)
        {
            string name = clip.name.ToLower().Trim();
            
            char octaveChar = name[name.Length - 1];
            if (char.IsDigit(octaveChar))
            {
                int octave = (int)char.GetNumericValue(octaveChar);
                // La nota es todo lo que está antes del número (ej: 'c#')
                string noteName = name.Substring(0, name.Length - 1);

                if (noteOffsets.ContainsKey(noteName))
                {
                    // Fórmula MIDI: (Octava + 1) * 12 + Offset de la nota
                    int midiNum = (octave + 1) * 12 + noteOffsets[noteName];
                    pianoSamples[midiNum] = clip;
                    availableMidiNotes.Add(midiNum);
                    Debug.Log($"<color=green>[ETHERIA]</color> Mapeado: {name} -> MIDI {midiNum}");
                }
            }
        }
        availableMidiNotes.Sort();

        // 2. Pool de voces
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource s = gameObject.AddComponent<AudioSource>();
            s.playOnAwake = false;
            s.spatialBlend = 0; 
            audioPool.Add(s);
        }
    }

    void Update()
    {
        while (udpReceiver.messageQueue.TryDequeue(out string msg))
        {
            ProcessMidi(msg);
        }
    }

    void ProcessMidi(string raw)
    {
        string[] p = raw.Split('|');
        if (p.Length < 6) return;

        if (p[0] == "note")
        {
            int note = int.Parse(p[2]);
            int vel = int.Parse(p[3]);
            if (p[1] == "note_on" && vel > 0) PlayNote(note, vel);
            else StopNote(note);
        }
        else if (p[0] == "cc" && p[1] == "64")
        {
            isPedalDown = int.Parse(p[2]) >= 64;
            if (!isPedalDown) ReleaseSustain();
        }
    }

    void PlayNote(int targetNote, int vel)
    {
        if (availableMidiNotes.Count == 0) return;

        int bestBaseNote = availableMidiNotes[0];
        float minDiff = float.MaxValue;
        foreach (int n in availableMidiNotes)
        {
            float diff = Mathf.Abs(targetNote - n);
            if (diff < minDiff) { minDiff = diff; bestBaseNote = n; }
        }

        // Si es la nota exacta, el pitch será 1.0. Si no, se ajustará solo.
        float semitoneOffset = targetNote - bestBaseNote;
        float pitch = Mathf.Pow(2.0f, semitoneOffset / 12.0f);

        // Dinámica de volumen
        float normalizedVel = vel / 127f;
        float curvedVolume = Mathf.Pow(normalizedVel, velocityCurve) * volumeBoost;

        if (activeNotes.ContainsKey(targetNote)) activeNotes[targetNote].Stop();

        foreach (var s in audioPool)
        {
            if (!s.isPlaying)
            {
                s.clip = pianoSamples[bestBaseNote];
                s.pitch = pitch;
                s.volume = Mathf.Clamp01(curvedVolume); 
                s.Play();
                activeNotes[targetNote] = s;
                sustainedNotes.Remove(targetNote);
                return;
            }
        }
    }

    void StopNote(int note)
    {
        if (isPedalDown) sustainedNotes.Add(note);
        else if (activeNotes.ContainsKey(note))
        {
            activeNotes[note].Stop();
            activeNotes.Remove(note);
        }
    }

    void ReleaseSustain()
    {
        foreach (int n in sustainedNotes)
        {
            if (activeNotes.ContainsKey(n))
            {
                activeNotes[n].Stop();
                activeNotes.Remove(n);
            }
        }
        sustainedNotes.Clear();
    }
}