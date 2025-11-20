using UnityEngine;
using UnityEditor;
using System.Collections;

namespace TunnelGame
{
	[CustomEditor(typeof(GameManager))]
	public class GameManagerEditor : Editor
	{
		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("General Settings:");
			EditorGUI.indentLevel++;
			DrawGeneralSettings();
			EditorGUI.indentLevel--;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Player Settings:");
			EditorGUI.indentLevel++;
			DrawPlayerSettings();
			EditorGUI.indentLevel--;
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Drop Settings:");
			EditorGUI.indentLevel++;
			DrawDropSettings();
			EditorGUI.indentLevel--;
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Background Settings:");
			EditorGUI.indentLevel++;
			DrawBackgroundSettings();
			EditorGUI.indentLevel--;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Tunnel Settings:");
			EditorGUI.indentLevel++;
			DrawTunnelSettings();
			EditorGUI.indentLevel--;

			EditorGUILayout.Space();

			serializedObject.ApplyModifiedProperties();
		}

		private void DrawGeneralSettings()
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("gameCamera"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("startUI"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("playerSelectUI"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("gameUI"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("overUI"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("showCollisionBoxesInScene"));

			if (GUILayout.Button("Clear Saved Data"))
			{
				PlayerPrefs.DeleteAll();
			}
		}

		private void DrawPlayerSettings()
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("playerInfos"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("scoreMultiplier"));
		}
		
		private void DrawDropSettings()
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("dropChance"), new GUIContent("Drop Chance %"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("dropCollectAmount"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("dropEdgePadding"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("dropPrefab"));
		}

		private void DrawBackgroundSettings()
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("bkgMaterial"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("bkgMoveSpeed"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("bkgRepeatSize"));
		}

		private void DrawTunnelSettings()
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tunnelMaterial"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("textureFillsTunnel"));
			
			SerializedProperty alignTexture = serializedObject.FindProperty("alignTextureWithTunnel");
			SerializedProperty unitSize		= serializedObject.FindProperty("unitSize");
			SerializedProperty textureSize	= serializedObject.FindProperty("textureSize");
			
			EditorGUILayout.PropertyField(alignTexture);
			
			if (alignTexture.boolValue)
			{
				EditorGUILayout.PropertyField(unitSize);
				textureSize.floatValue = unitSize.floatValue;
			}
			else
			{
				EditorGUILayout.PropertyField(textureSize);
				unitSize.floatValue = 1f;
			}
			
			EditorGUILayout.PropertyField(serializedObject.FindProperty("minTunnelSizeInUnits"), new GUIContent(alignTexture.boolValue ? "Min Tunnel Size In Units" : "Min Tunnel Size"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxTunnelSizeInUnits"), new GUIContent(alignTexture.boolValue ? "Max Tunnel Size In Units" : "Max Tunnel Size"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("minTunnelLengthInUnits"), new GUIContent(alignTexture.boolValue ? "Min Tunnel Length In Units" : "Min Tunnel Length"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxTunnelLengthInUnits"), new GUIContent(alignTexture.boolValue ? "Max Tunnel Length In Units" : "Max Tunnel Length"));
		}
	}
}
