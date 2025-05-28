using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(MassScaler))]
public class MassScalerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //reference au scripy cible
        MassScaler scaler= (MassScaler)target;

        //Champ rigidbody
        scaler.rb=(Rigidbody)EditorGUILayout.ObjectField("Rigidbody",scaler.rb,typeof(Rigidbody),true);

        //Slider dynamique base sur masseMin et masseMax

        EditorGUILayout.LabelField("Parametres de configuration",EditorStyles.boldLabel);
        scaler.masseMin = EditorGUILayout.FloatField("Masse Min", scaler.masseMin);
        scaler.masseMax = EditorGUILayout.FloatField("Masse Max", scaler.masseMax);

        scaler.masse = EditorGUILayout.Slider("Masse", scaler.masse, scaler.masseMin, scaler.masseMax);

        scaler.facteurEchelle = EditorGUILayout.FloatField("Facteur Echelle", scaler.facteurEchelle);

        //appliquer les changements
        if (!Application.isPlaying)
        {
            scaler.masse=Mathf.Clamp(scaler.masse,scaler.masseMin,scaler.masseMax);
            float echelle = Mathf.Max(0.1f, scaler.masse * scaler.facteurEchelle);
            scaler.transform.localScale=new Vector3(echelle,echelle,echelle);

        }

        //marquer l'objet comme modifie
        if (GUI.changed)
        {
            EditorUtility.SetDirty(scaler);
        }
    }
}
