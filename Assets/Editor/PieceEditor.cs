using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Piece))]
[CanEditMultipleObjects]
public class PieceEditor : Editor
{
    SerializedProperty frontColor;
    SerializedProperty backColor;
    SerializedProperty leftColor;
    SerializedProperty rightColor;
    SerializedProperty upColor;
    SerializedProperty downColor;
    
    Dictionary<SquareColor, Material> materialMap = new Dictionary<SquareColor, Material>();

    private void OnEnable() {
        materialMap.Add(SquareColor.red, (Material) EditorGUIUtility.Load("Materials/Red.mat"));
        materialMap.Add(SquareColor.blue, (Material) EditorGUIUtility.Load("Materials/Blue.mat"));
        materialMap.Add(SquareColor.white, (Material) EditorGUIUtility.Load("Materials/White.mat"));
        materialMap.Add(SquareColor.green, (Material) EditorGUIUtility.Load("Materials/Green.mat"));
        materialMap.Add(SquareColor.orange, (Material) EditorGUIUtility.Load("Materials/Orange.mat"));
        materialMap.Add(SquareColor.yellow, (Material) EditorGUIUtility.Load("Materials/Yellow.mat"));
        materialMap.Add(SquareColor.none, (Material) EditorGUIUtility.Load("Materials/None.mat"));

        frontColor = serializedObject.FindProperty("frontColor");
        backColor = serializedObject.FindProperty("backColor");
        leftColor = serializedObject.FindProperty("leftColor");
        rightColor = serializedObject.FindProperty("rightColor");
        upColor = serializedObject.FindProperty("upColor");
        downColor = serializedObject.FindProperty("downColor");
    }

    
     public override void OnInspectorGUI(){
         
        EditorGUILayout.PropertyField (frontColor);
        EditorGUILayout.PropertyField (backColor);
        EditorGUILayout.PropertyField (leftColor);
        EditorGUILayout.PropertyField (rightColor);
        EditorGUILayout.PropertyField (upColor);
        EditorGUILayout.PropertyField (downColor);

        SquareColor selectedFrontColor = (SquareColor) frontColor.enumValueIndex;
        SquareColor selectedBackColor = (SquareColor) backColor.enumValueIndex;
        SquareColor selectedLeftColor = (SquareColor) leftColor.enumValueIndex;
        SquareColor selectedRightColor = (SquareColor) rightColor.enumValueIndex;
        SquareColor selectedUpColor = (SquareColor) upColor.enumValueIndex;
        SquareColor selectedDownColor = (SquareColor) downColor.enumValueIndex;

        //Debug.Log(selectedFrontColor + ", " + selectedBackColor + ", " + selectedLeftColor + ", " + selectedDownColor);

        foreach(Piece target in targets){
            target.Initialize();

            target.SetMaterial(CubeFace.front, materialMap[selectedFrontColor]);
            target.SetMaterial(CubeFace.back, materialMap[selectedBackColor]);
            target.SetMaterial(CubeFace.left, materialMap[selectedLeftColor]);
            target.SetMaterial(CubeFace.right, materialMap[selectedRightColor]);
            target.SetMaterial(CubeFace.up, materialMap[selectedUpColor]);
            target.SetMaterial(CubeFace.down, materialMap[selectedDownColor]);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
