using UnityEngine;
using UnityEngine.UI;

public class ChartEditorUI : MonoBehaviour
{
    public ChartEditor chartEditor;    // R¨¦f¨¦rence vers le ChartEditor
    public AudioSource musicSource;    // Source audio utilis¨¦e pour la musique
    public Button startButton;         // Bouton "Start Recording"
    public Button saveButton;          // Bouton "Save Chart"

    public string chartFileName = "mon_chart"; // Nom du fichier JSON

    void Start()
    {
        startButton.onClick.AddListener(StartRecording);
        saveButton.onClick.AddListener(SaveChart);
    }

    void StartRecording()
    {
        chartEditor.currentChart = new ChartData();  // R¨¦initialiser les anciennes donn¨¦es
        musicSource.Play();                          // Lancer la musique
        Debug.Log("Enregistrement commenc¨¦");
    }

    void SaveChart()
    {
        ChartIO.SaveChart(chartEditor.currentChart, chartFileName);
        Debug.Log("Chart sauvegard¨¦ : " + chartFileName);
    }
}
