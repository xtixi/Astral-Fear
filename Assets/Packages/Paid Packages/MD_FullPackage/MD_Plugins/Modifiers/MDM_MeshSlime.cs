using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MD_Plugin
{
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Modifiers/Mesh Slime")]
    public class MDM_MeshSlime : MonoBehaviour
    {
        //-----------------------DESCRIPTION------------------------------------------
        //----------------------------------------------------------------------------
        //---MD (Mesh Deformation Modifier): Soft Mesh = Component for objects with Mesh Filter
        //---Precious mesh-slime-body modifier with various settings & mobile support
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public bool exp_UseControls = true;
        public KeyCode exp_Input = KeyCode.Mouse0;
        public bool exp_MobilePlatform = false;
        public Camera exp_MainCamera;
        public enum exp_AxisType { Y_Only, TowardsObjectsPivot, CrossProduct, NormalsDirection };
        public exp_AxisType exp_Axis = exp_AxisType.Y_Only;
        [Space]
        public bool exp_Repair = false;
        public float exp_RepairSpeed = 1f;
        [Space]
        public float exp_Radius = 0.1f;
        public float exp_Falloff = 1.0f;
        public float exp_Intensity = 0.1f;
        [Space]
        public bool exp_ReversedDrag = false;
        public float exp_Drag = 0.16f;
        public float exp_DragFalloff = 0.8f;
        public float exp_MaxDragSpeed = 0.5f;

        private MeshFilter meshf;
        private Vector3[] verts;
        private Vector3[] originVerts;

        private void Awake()
        {
            if (!MD_MeshProEditor.MD_INTERNAL_TECH_CheckModifiers(this.gameObject, this.GetType().Name))
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    EditorUtility.DisplayDialog("Error", "The modifier cannot be applied to this object, because the object already contains modifiers. Please, remove exists modifier to access to the selected modifier...", "OK");
#endif
                DestroyImmediate(this);
                return;
            }

            if (!GetComponent<MeshFilter>())
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    EditorUtility.DisplayDialog("Error", "The object doesn't contain Mesh Filter which is very required component...", "OK");
                DestroyImmediate(this);
#endif
                return;
            }

            if (!exp_MainCamera)
                Debug.LogError("Camera is missing!");

            meshf = GetComponent<MeshFilter>();

            Mesh m = Instantiate(meshf.mesh);
            meshf.mesh = m;
            meshf.mesh.MarkDynamic();
            verts = meshf.mesh.vertices;
            originVerts = meshf.mesh.vertices;
        }

        private Vector3 oldhPos;
        private Vector3 currhPos;

        private void Update()
        {
            if (exp_Repair)
            {
                for (int i = 0; i < verts.Length; i++)
                    verts[i] = Vector3.Lerp(verts[i], originVerts[i], exp_RepairSpeed * Time.deltaTime);
                meshf.mesh.vertices = verts;
                meshf.mesh.RecalculateNormals();
                meshf.mesh.RecalculateBounds();
            }

            if (!exp_UseControls)
                return;

            Ray r = new Ray();
            RaycastHit h;

            if (!exp_MobilePlatform)
                r = exp_MainCamera.ScreenPointToRay(Input.mousePosition);
            else if (Input.touchCount > 0)
                r = exp_MainCamera.ScreenPointToRay(Input.GetTouch(0).position);

            bool c;
            if (!exp_MobilePlatform)
                c = Input.GetKey(exp_Input);
            else
                c = Input.touchCount > 0;


            if (!c) { oldhPos = Vector3.zero; return; }

            if (Physics.Raycast(r, out h))
            {
                if (h.collider)
                {
                    currhPos = ((!exp_ReversedDrag) ? (h.point - oldhPos) : (oldhPos - h.point));
                    if (currhPos.magnitude > exp_MaxDragSpeed)
                        oldhPos = Vector3.zero;
                    ModifyMesh(h.point);
                    oldhPos = h.point;
                }
            }
        }

        private void ModifyMesh(Vector3 p)
        {
            currhPos.y = 0;
            p = transform.InverseTransformPoint(p);
            for (int i = 0; i < verts.Length; i++)
            {
                Vector3 vv = verts[i];
                if (Vector3.Distance(vv, p) > exp_Radius)
                    continue;
                float mult = exp_Falloff * (exp_Radius - (Vector3.Distance(p, vv)));
                Vector3 dir = Vector3.zero;
                switch (exp_Axis)
                {
                    case exp_AxisType.Y_Only:
                        dir = Vector3.up;
                        break;
                    case exp_AxisType.TowardsObjectsPivot:
                        dir = -(transform.position - transform.TransformPoint(vv));
                        break;
                    case exp_AxisType.CrossProduct:
                        dir = Vector3.Cross(vv, p);
                        break;
                    case exp_AxisType.NormalsDirection:
                        dir = meshf.mesh.normals[i];
                        break;
                }
                vv -= (((dir * exp_Intensity) * mult) + ((oldhPos == Vector3.zero) ? Vector3.zero : (currhPos * ((exp_ReversedDrag) ? (exp_Drag / 2f) * exp_DragFalloff : exp_Drag * exp_DragFalloff))));
                verts[i] = vv;
            }
            meshf.mesh.vertices = verts;
            meshf.mesh.RecalculateNormals();
            meshf.mesh.RecalculateBounds();
        }

        public void ModifyMeshExternally(MDM_RaycastEvent entry)
        {
            if (entry.hits.Length > 0 && entry.hits[0].collider.gameObject != this.gameObject)
                return;
            foreach (RaycastHit hit in entry.hits)
                ModifyMesh(hit.point);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MDM_MeshSlime))]
    public class MDM_MeshSlime_Editor : Editor
    {
        MDM_MeshSlime ms;
       
        private void OnEnable()
        {
            ms = (MDM_MeshSlime)target;
        }

        public override void OnInspectorGUI()
        {
            S();
            GUILayout.BeginVertical("Box");
            DrawProperty("exp_UseControls", "Use Contnrols", "If enabled, you will be able to drag the slime mesh with mouse/finger");
            DrawProperty("exp_MobilePlatform", "Mobile Platform", "If enabled, the slime mesh will be ready for Mobile devices");
            if (ms.exp_UseControls && !ms.exp_MobilePlatform)
                DrawProperty("exp_Input", "Input");
            DrawProperty("exp_MainCamera", "Main Camera", "Main Cam target");
            GUILayout.EndVertical();
            S();
            GUILayout.BeginVertical("Box");
            DrawProperty("exp_Axis", "Slime Axis Type", "Select proper axis type, each axis type is specific and unique");
            GUILayout.EndVertical();
            S();
            GUILayout.BeginVertical("Box");
            DrawProperty("exp_Repair", "Repair Slime", "If enabled, the mesh will be 'repaired' to the starting shape");
            if(ms.exp_Repair)
            DrawProperty("exp_RepairSpeed", "Repair Speed");
            GUILayout.EndVertical();
            S();
            GUILayout.BeginVertical("Box");
            DrawProperty("exp_Radius", "Drag Radius");
            DrawProperty("exp_Falloff", "Drag Falloff Radius");
            DrawProperty("exp_Intensity", "Drag Intensity");
            GUILayout.EndVertical();
            S();
            GUILayout.BeginVertical("Box");
            DrawProperty("exp_ReversedDrag", "Reversed Drag","If enabled, the dragged vertices will move towards the cursor move position [if disabled - vice versa]");
            DrawProperty("exp_Drag", "Drag Density","Force in which the drag is proceed");
            DrawProperty("exp_DragFalloff", "Drag Density Falloff");
            DrawProperty("exp_MaxDragSpeed", "Max Drag Speed");
            GUILayout.EndVertical();

            serializedObject.Update();
        }

        private void DrawProperty(string p, string Text, string Tooltip = "", bool childs = false)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(p), new GUIContent(Text, Tooltip), childs);
            serializedObject.ApplyModifiedProperties();
        }

        private void S(float s = 5)
        {
            GUILayout.Space(s);
        }
    }
#endif
}