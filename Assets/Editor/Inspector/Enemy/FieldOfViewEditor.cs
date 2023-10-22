using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[HideScriptField]
[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : HiddenScriptEditor
{
    private void OnSceneGUI()
    {
        
        FieldOfView fov = (FieldOfView) target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.radius);
        
        
        Vector3 viewAngleA = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.angle / 2);
        Vector3 viewAngleB = DirectionFromAngle(fov.transform.eulerAngles.y, fov.angle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.radius);
        
        Handles.color = Color.red;
        
        foreach (var visibleTarget in fov.visibleTargets)
        {
            Handles.DrawLine(fov.transform.position, visibleTarget.position);
        }
    }

    private Vector3 DirectionFromAngle(float eurlerY, float angleInDegrees)
    {
        angleInDegrees += eurlerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}

[CustomEditor(typeof(CameraController))] public class CameraControllerEditor : HiddenScriptEditor { }
public class HiddenScriptEditor : UnityEditor.Editor
{
    private static readonly string[] k_HiddenFields = { "m_Script" };

    /// <summary>
    /// Draws inspector properties without the script field.
    /// </summary>
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        serializedObject.UpdateIfRequiredOrScript();
        DrawPropertiesExcluding(serializedObject, k_HiddenFields);
        serializedObject.ApplyModifiedProperties();
        EditorGUI.EndChangeCheck();
    }
}