using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColoredSquare))]
[CanEditMultipleObjects]
public class ColoredSquareEditor : Editor
{
    
    SerializedProperty color;
    
    Dictionary<SquareColor, Material> materialMap = new Dictionary<SquareColor, Material>();

    private void OnEnable() {
        materialMap.Add(SquareColor.red, (Material) EditorGUIUtility.Load("Materials/Red.mat"));
        materialMap.Add(SquareColor.blue, (Material) EditorGUIUtility.Load("Materials/Blue.mat"));
        materialMap.Add(SquareColor.white, (Material) EditorGUIUtility.Load("Materials/White.mat"));
        materialMap.Add(SquareColor.green, (Material) EditorGUIUtility.Load("Materials/Green.mat"));
        materialMap.Add(SquareColor.orange, (Material) EditorGUIUtility.Load("Materials/Orange.mat"));
        materialMap.Add(SquareColor.yellow, (Material) EditorGUIUtility.Load("Materials/Yellow.mat"));

        color = serializedObject.FindProperty("Color");
    }

    
     public override void OnInspectorGUI(){
         
        EditorGUILayout.PropertyField (color);

        SquareColor cubeletColor = (SquareColor) color.enumValueIndex;

        foreach(ColoredSquare target in targets) target.SetColor(materialMap[cubeletColor]);

        serializedObject.ApplyModifiedProperties();
    }
}
