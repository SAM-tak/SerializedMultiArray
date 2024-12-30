using UnityEditor;
using UnityEngine;

namespace SerializedArray
{
    [CustomPropertyDrawer(typeof(BoolPlane))]
    public class BoolPlaneDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            // 子のフィールドをインデントしない 
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            // 矩形を計算
            var labelRect = new Rect(position.x, position.y, position.width * 0.4f, position.height);
            var sizeRect = new Rect(position.x + position.width * 0.4f, position.y, position.width * 0.4f, position.height);
            var buttonRect = new Rect(position.x + position.width * 0.8f + 10, position.y, position.width * 0.2f - 10, position.height);

            var size = property.FindPropertyRelative("_size").vector2IntValue;

            // ラベル表示
            EditorGUI.PrefixLabel(labelRect, GUIUtility.GetControlID(FocusType.Passive), label);

            // サイズ表示
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.Vector2IntField(sizeRect, GUIContent.none, size);
            EditorGUI.EndDisabledGroup();

            // 編集ウィンドウを表示する
            if(GUI.Button(buttonRect, "Edit")) {
                BoolPlaneWindow.ShowWindow(property, label.text);
            }

            // インデントを元通りに戻す
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        class BoolPlaneWindow : EditorWindow
        {
            // 値の編集・更新
            SerializedProperty property;
            BoolPlane box;

            // 編集対象
            bool[,] value;

            // EditorGUI用変数
            Vector2 scroll;
            Vector2Int planeSize;
            Color line = new(0.8f, 0.8f, 0.8f);

            /// <summary>
            /// ウィンドウを表示する
            /// </summary>
            public static void ShowWindow(SerializedProperty property, string label)
            {
                var window = GetWindow<BoolPlaneWindow>(label);

                // 値の設定
                window.property = property;
                var box = property.boxedValue as BoolPlane;
                window.box = box;
                window.value = box.Value;
                window.planeSize = new Vector2Int(box.Width, box.Height);

                window.ShowUtility();
            }

            private void OnGUI()
            {
                if(property == null) {
                    Close();
                    return;
                }

                bool dirty = false;

                // 配列のサイズを設定する
                EditorGUILayout.LabelField("Set array size:");
                EditorGUILayout.BeginHorizontal();
                planeSize = EditorGUILayout.Vector2IntField(GUIContent.none, planeSize);
                if(GUILayout.Button("Set")) {
                    // 配列のサイズ変更
                    ResizeArray();
                    // 変更した配列をBoolPlaneに適用
                    box.Value = value;
                    dirty = true;
                }
                EditorGUILayout.EndHorizontal();

                // 値を表示するエリア
                if(value == null) {
                    return;
                }

                scroll = EditorGUILayout.BeginScrollView(scroll);

                for(int y = 0; y < value.GetLength(1); y++) {
                    EditorGUILayout.BeginHorizontal();
                    for(int x = 0; x < value.GetLength(0); x++) {
                        // 5マスごとに色を変更する
                        bool colored = x % 5 == 4 || y % 5 == 4;
                        if(colored) {
                            GUI.color = line;
                        }
                        var bit = EditorGUILayout.Toggle(value[x, y], GUILayout.Width(15));
                        if(value[x, y] != bit) {
                            value[x, y] = bit;
                            dirty = true;
                        }
                        if(colored) {
                            GUI.color = Color.white;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();

                if(dirty) {
                    // 保存処理
                    property.boxedValue = box;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            /// <summary>
            /// 配列をリサイズする
            /// </summary>
            private void ResizeArray()
            {
                var newValue = new bool[planeSize.x, planeSize.y];
                for(int x = 0; x < planeSize.x && x < value.GetLength(0); x++)
                    for(int y = 0; y < planeSize.y && y < value.GetLength(1); y++) {
                        newValue[x, y] = value[x, y];
                    }
                value = newValue;
            }
        }
    }
}

/*Docs
 *https://kazupon.org/unity-no-edit-param-view-inspector/
 *https://docs.unity3d.com/ja/2021.3/Manual/editor-PropertyDrawers.html
 *https://docs.unity3d.com/ja/2021.1/ScriptReference/EditorGUILayout.html
 *https://docs.unity3d.com/ja/current/ScriptReference/GUI.html
 */
