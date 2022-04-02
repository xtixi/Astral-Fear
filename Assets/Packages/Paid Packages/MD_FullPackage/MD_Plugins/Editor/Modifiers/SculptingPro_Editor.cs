using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MD_Plugin;

[CustomEditor(typeof(MDM_SculptingPro))]
public class SculptingPro_Editor : ModifiersEditorController
{
    private MDM_SculptingPro ss;

    private void OnEnable()
    {
        ss = (MDM_SculptingPro)target;

        SculptingPro_Window.B_EditMode = ss.SS_InEditMode;
        SculptingPro_Window.B_Radius = ss.SS_BrushSize;
        SculptingPro_Window.B_Strength = ss.SS_BrushStrength;
    }

    public override void OnInspectorGUI()
    {
        int gSize = GUI.skin.font.fontSize;
        GUILayout.Space(10);
        GUI.skin.label.fontSize = 21;
        GUILayout.Label(new GUIContent("Sculpting Pro v1.4"));
        EditorGUILayout.HelpBox(new GUIContent("v1.4 Change-Log (01/12/2020 [dd/mm/yyyy])\n- Added Smooth feature\n- Added Stylize feature\n- Added distance limitations\n- Updated UI\n- Major optimizations"));
        GUI.skin.label.fontSize = gSize;

        GUILayout.Space(10);

        GUILayout.Label("Essential Settings");
        GUILayout.BeginVertical("Box");
        GUILayout.BeginVertical("Box");
        DrawProperty("SS_AtRuntime", "Sculpting At Runtime", "If enabled, the script will work only at runtime");
        GUILayout.Space(3);
        DrawProperty("SS_MobileSupport", "Sculpting Editor For Mobile", "If enabled, the system will work only for mobile devices");
        GUILayout.EndVertical();
        GUILayout.Space(5);
        DrawProperty("SS_AutoRecalculateNormals", "Auto Recalculate Normals", "If enabled, the system will recalculate mesh normals");
        if (ss.SS_UseInput && !ss.SS_MultithreadingSupported)
            DrawProperty("SS_UpdateColliderAfterRelease", "Update Collider After Release", "If disabled, the collider will be updated every frame if the mouse is down");
        else if (ss.SS_MultithreadingSupported && !ss.SS_UpdateColliderAfterRelease)
            ss.SS_UpdateColliderAfterRelease = true;
        GUILayout.EndVertical();

        if (!ss.SS_AtRuntime)
        {
            GUILayout.BeginVertical("Box");
            DrawProperty("SS_InEditMode", "In Edit Mode", "If enabled, the selection will be locked to the object and you are free to sculpt the mesh");
            EditorGUILayout.HelpBox("LMouse: Raise, LShift+LMouse: Lower, LControl+LMouse: Revert, R: Restart Mesh", MessageType.Info);
            GUILayout.EndVertical();
        }

        GUILayout.Space(10);

        GUILayout.Label("Multithreading Settings");
        GUILayout.BeginVertical("Box");
        DrawProperty("SS_MultithreadingSupported", "Multithreading Supported", "If enabled, the sculpting system will be ready for advanced & complex meshes. See more 'Multithreading' in docs.");
        if(ss.SS_MultithreadingSupported)
           DrawProperty("SS_MultithreadingProcessDelay","Process Delay [millisec]","Multithreading process delay [default: 10] - the bigger number is, the smoother result should be.");
        if(ss.SS_MultithreadingSupported)
        {
            if (GUILayout.Button(new GUIContent("Fix Brush Incorrection","If the multithreading method is enabled, sculpting brush might be in another scale-ratio. This could be fixed after clicking this button. If not, you must adjust brush scale by yourself [Create empty object, group empty object with target brush graphic and assign empty object to the Brush Projection].")))
            {
                if(EditorUtility.DisplayDialog("Info","You can choose between 2 methods\n1. Would you like to reset the mesh matrix transform? This will set the current transform scale to ONE, but it could take more than 1 minute.\n\n2. Adjust child brush scale if it's possible. This will adjust child brush scale to 0.1.","I'll choose 1","I'll choose 2"))
                    ss.SS_Funct_BakeMesh();
                else
                {
                    if(ss.SS_BrushProjection.transform.childCount>0)
                    {
                        foreach (Transform t in ss.SS_BrushProjection.transform)
                            t.transform.localScale = Vector3.one * 0.1f;
                    }
                }
            }
        }
        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.Label("Brush Settings");
        GUILayout.BeginVertical("Box");
        DrawProperty("SS_UseBrushProjection", "Show Brush Projection");
        DrawProperty("SS_BrushProjection", "Brush Projection");
        GUILayout.Space(5);
        GUILayout.BeginVertical("Box");
        DrawProperty("SS_BrushSize", "Brush Size");
        DrawProperty("SS_BrushStrength", "Brush Strength");
        GUILayout.EndVertical();
        GUILayout.Space(5);
        DrawProperty("SS_State", "Brush State","Current brush state. The field is visible in case of keeping the only ONE brush state");

        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.Label("Sculpting Settings");
        GUILayout.BeginVertical("Box");
        DrawProperty("SS_MeshSculptMode", "Sculpt Mode");
        if (ss.SS_MeshSculptMode == MDM_SculptingPro.SS_MeshSculptMode_Internal.CustomDirection)
            DrawProperty("SS_CustomDirection", "Custom Direction", "Choose a custom direction in world space");
        else if (ss.SS_MeshSculptMode == MDM_SculptingPro.SS_MeshSculptMode_Internal.CustomDirectionObject)
        {
            DrawProperty("SS_CustomDirectionObject", "Custom Direction Object");
            DrawProperty("SS_CustomDirObjDirection", "Direction Towards Object", "Choose a direction of the included object above in local space");
        }
        GUILayout.Space(5);
        GUILayout.BeginVertical("Box");
        DrawProperty("SS_EnableHeightLimitations", "Height Limitations","If enabled, you will be able to set vertices Y Limit [height] in world space (great for planar terrains)");
        if(ss.SS_EnableHeightLimitations)
            DrawProperty("SS_HeightLimitations", "Height Limitations", "Minimum[X] and Maximum[Y] height limitation in world space");
        GUILayout.EndVertical();
        GUILayout.BeginVertical("Box");
        DrawProperty("SS_EnableDistanceLimitations", "Distance Limitations", "If enabled, you will be able to limit the vertices sculpting range in both inside depth and outside depth");
        if (ss.SS_EnableDistanceLimitations) DrawProperty("SS_DistanceLimitation", "Distance Limitation", "How far can the vertice go?");
        GUILayout.EndVertical();
        GUILayout.Space(5);
        DrawProperty("SS_SculptingType", "Sculpting Type", "Choose a sculpting type.");
        GUILayout.EndVertical();

        if (ss.SS_AtRuntime)
        {
            GUILayout.Space(10);

            GUILayout.Label("Input & Feature Settings");
            if (!ss.SS_MobileSupport)
            {
                GUILayout.BeginVertical("Box");
                DrawProperty("SS_UseInput", "Use Input", "Use custom sculpt input controls. Otherwise, you can use internal API functions to interact the mesh sculpt.");
                if (ss.SS_UseInput)
                {
                    GUILayout.BeginVertical("Box");
                    DrawProperty("SS_UseRaiseFunct", "Use Raise", "Use Raise sculpting function");
                    DrawProperty("SS_UseLowerFunct", "Use Lower", "Use Lower sculpting function");
                    DrawProperty("SS_UseRevertFunct", "Use Revert", "Use Revert sculpting function");
                    DrawProperty("SS_UseNoiseFunct", "Use Noise", "Use Noise sculpting function");
                    if (ss.SS_UseNoiseFunct)
                    {
                        GUILayout.BeginVertical("Box");
                        DrawProperty("SS_NoiseFunctionDirections", "Noise Direction", "Choose a noise direction in world space");
                        GUILayout.EndVertical();
                    }
                    DrawProperty("SS_UseSmoothFunct", "Use Smooth", "Use Smooth sculpting function");
                    if (ss.SS_UseSmoothFunct)
                    {
                        GUILayout.BeginVertical("Box");
                        DrawProperty("SS_smoothType", "Smoothing Type", "Choose between two smoothing types... HCfilter is less problematic, but takes more time to process. Laplacian is more problematic, but takes much less time. In general, the HCfilter is recommended for planar meshes, the Laplacian for spatial meshes.");
                        if(ss.SS_smoothType == MDM_SculptingPro.SS_SmoothType.LaplacianFilter) DrawProperty("SS_SmoothIntensity", "Smooth Intensity");
                        GUILayout.EndVertical();
                    }
                    DrawProperty("SS_UseStylizeFunct", "Use Stylize", "Use Stylize sculpting function");
                    if (ss.SS_UseStylizeFunct)
                    {
                        GUILayout.BeginVertical("Box");
                        DrawProperty("SS_StylizeIntensity", "Stylize Intensity");
                        GUILayout.EndVertical();
                    }

                    GUILayout.EndVertical();

                    if ((ss.SS_UseSmoothFunct || ss.SS_UseStylizeFunct) && !ss.SS_MultithreadingSupported)
                        EditorGUILayout.HelpBox("The Smooth & Stylize functions are not supported for non-multithreaded modifiers. Please enable Multithreading to make the Smooth or Stylize function work.", MessageType.Warning);

                    GUILayout.Space(2);
                    if (ss.SS_UseRaiseFunct)
                        DrawProperty("SS_SculptingRaiseInput", "Raise Input");
                    if (ss.SS_UseLowerFunct)
                        DrawProperty("SS_SculptingLowerInput", "Lower Input");
                    if (ss.SS_UseRevertFunct)
                        DrawProperty("SS_SculptingRevertInput", "Revert Input");
                    if (ss.SS_UseNoiseFunct)
                        DrawProperty("SS_SculptingNoiseInput", "Noise Input");
                    if (ss.SS_UseSmoothFunct)
                        DrawProperty("SS_SculptingSmoothInput", "Smooth Input");
                    if (ss.SS_UseStylizeFunct)
                        DrawProperty("SS_SculptingStylizeInput", "Stylize Input");

                    GUILayout.Space(5);
                    DrawProperty("SS_SculptFromCursor", "Sculpt Origin Is Cursor", "If enabled, the raycast origin will be cursor");
                    GUILayout.Space(3);
                    if (!ss.SS_SculptFromCursor)
                        DrawProperty("SS_SculptOrigin", "Sculpt Origin Object", "The raycast origin");
                    GUILayout.Space(3);
                    GUILayout.BeginVertical("Box");
                    DrawProperty("SS_MainCamera", "Main Camera Source", "Assign main camera to make the raycast work properly");
                    DrawProperty("SS_SculptingLayerMask", "Sculpting Layer Mask", "Set custom sculpting layer mask");
                    GUILayout.EndVertical();

                }
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.Space(3);
                GUILayout.BeginVertical("Box");
                DrawProperty("SS_MainCamera", "Main Camera Source", "Assign main camera to make the raycast work properly");
                DrawProperty("SS_SculptingLayerMask", "Sculpting Layer Mask", "Set custom sculpting layer mask");
                GUILayout.EndVertical();
            }
        }
        else
        {
            GUILayout.Space(10);
            if (GUILayout.Button("Show Editor Tool Window"))
                SculptingPro_Window.Init();
            GUILayout.Space(10);
        }

        SculptingPro_Window.ssSource = ss;

        if (ss != null)
            serializedObject.Update();

        ppAddMeshColliderRefresher(ss);
        ppBackToMeshEditor(ss);
       
    }
    private void DrawProperty(string p, string Text, string Tooltip = "", bool childs = false)
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty(p), new GUIContent(Text, Tooltip), childs);
        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        if (ss.SS_BrushProjection == null || !ss.SS_InEditMode || ss.SS_AtRuntime)
        {
            if (!Application.isPlaying)
                ss.CheckForThread(false);
            return;
        }

        if (ss.SS_BrushProjection.GetComponent<Collider>())
            DestroyImmediate(ss.SS_BrushProjection.GetComponent<Collider>());

        if (!Application.isPlaying)
        {
            if (!ss.SS_MultithreadingSupported)
                ss.CheckForThread(false);
        }

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        Tools.current = Tool.None;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == ss.transform.GetComponent<Collider>())
            {
                if (!ss.SS_UseBrushProjection)
                {
                    if (ss.SS_BrushProjection.activeInHierarchy)
                        ss.SS_BrushProjection.SetActive(false);
                }
                else
                    ss.SS_BrushProjection.SetActive(true);
                ss.SS_BrushProjection.transform.position = hit.point;
                ss.SS_BrushProjection.transform.rotation = Quaternion.FromToRotation(-Vector3.forward,hit.normal);
                ss.SS_BrushProjection.transform.localScale = new Vector3(ss.SS_BrushSize, ss.SS_BrushSize, ss.SS_BrushSize);

                switch (ss.SS_State)
                {
                    case MDM_SculptingPro.SS_State_Internal.Raise:
                        ss.SS_Funct_DoSculpting(hit.point, ss.SS_BrushProjection.transform.forward, ss.SS_BrushSize, ss.SS_BrushStrength, MDM_SculptingPro.SS_State_Internal.Raise);
                        if (!ss.SS_UpdateColliderAfterRelease)
                            ss.SS_Funct_RefreshMeshCollider();
                        break;
                    case MDM_SculptingPro.SS_State_Internal.Lower:
                        ss.SS_Funct_DoSculpting(hit.point, ss.SS_BrushProjection.transform.forward, ss.SS_BrushSize, ss.SS_BrushStrength, MDM_SculptingPro.SS_State_Internal.Lower);
                        if (!ss.SS_UpdateColliderAfterRelease)
                            ss.SS_Funct_RefreshMeshCollider();
                        break;


                    case MDM_SculptingPro.SS_State_Internal.Revert:
                        ss.SS_Funct_DoSculpting(hit.point, ss.SS_BrushProjection.transform.forward, ss.SS_BrushSize, ss.SS_BrushStrength, MDM_SculptingPro.SS_State_Internal.Revert);
                        if (!ss.SS_UpdateColliderAfterRelease)
                            ss.SS_Funct_RefreshMeshCollider();
                        break;
                }
            }
            else
                ss.SS_BrushProjection.SetActive(false);
        }
        else
            ss.SS_BrushProjection.SetActive(false);



        #region Editor Hotkeys
        if (Application.isPlaying)
            return;
        if (ss.SS_InEditMode)
        {
            //---Mouse
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !Event.current.alt)
            {
                if (!Event.current.control)
                {
                    if (!Event.current.shift)
                        ss.SS_State = MDM_SculptingPro.SS_State_Internal.Raise;
                    else
                        ss.SS_State = MDM_SculptingPro.SS_State_Internal.Lower;
                }
                else
                    ss.SS_State = MDM_SculptingPro.SS_State_Internal.Revert;
            }
            else if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                ss.SS_State = MDM_SculptingPro.SS_State_Internal.None;
                ss.SS_Funct_RefreshMeshCollider();
                if (!Application.isPlaying)
                {
                    if (ss.SS_MultithreadingSupported)
                        ss.CheckForThread();
                }
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                ss.SS_State = MDM_SculptingPro.SS_State_Internal.None;
                ss.SS_Funct_RefreshMeshCollider();
            }

            //---Keys
            if (Event.current.type == EventType.KeyDown)
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.R:
                        ss.SS_Funct_RestoreOriginal();
                        break;
                }
            }
        }

        #endregion
    }
}
