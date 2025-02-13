﻿using System;
using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
	#if UNITY_2021_2_OR_NEWER
		using UnityEditor.SceneManagement;
	#else
		using UnityEditor.Experimental.SceneManagement;
	#endif
#endif

namespace Unity.DemoTeam.DigitalHuman
{
	[ExecuteAlways, DisallowMultipleComponent]
	public abstract class MeshInstanceBehaviour : MonoBehaviour
	{
		public const string meshInstanceSuffix = "(MeshInstance)";

		[HideInInspector]
		public Mesh meshAsset;
		[NonSerialized]
		public Mesh meshInstance;

		protected virtual void OnMeshInstanceCreated() { }
		protected virtual void OnMeshInstanceDeleted() { }

		static bool IsMeshInstance(Mesh mesh)
		{
#if UNITY_EDITOR
			return (EditorUtility.IsPersistent(mesh) == false);
#else
			return (mesh.GetInstanceID() < 0);
#endif
		}

		public void EnsureMeshInstance()
		{
			if(TryGetComponent<SkinnedMeshRenderer>(out var smr))
			{
				if (smr.sharedMesh == null || (IsMeshInstance(smr.sharedMesh) && smr.sharedMesh != meshInstance))
					smr.sharedMesh = meshAsset;

				if (smr.sharedMesh != null && !IsMeshInstance(smr.sharedMesh))
					smr.sharedMesh = EnsureMeshInstanceAux(smr.sharedMesh);

				return;
			}

			if(TryGetComponent<MeshFilter>(out var mf))
			{
				if (mf.sharedMesh == null || (IsMeshInstance(mf.sharedMesh) && mf.sharedMesh != meshInstance))
					mf.sharedMesh = meshAsset;

				if (mf.sharedMesh != null && !IsMeshInstance(mf.sharedMesh))
					mf.sharedMesh = EnsureMeshInstanceAux(mf.sharedMesh);

				return;
			}
		}

		Mesh EnsureMeshInstanceAux(Mesh mesh)
		{
			if (meshInstance != null)
			{
				Mesh.DestroyImmediate(meshInstance);
				meshInstance = null;
			}

			meshAsset = mesh;
			meshInstance = Mesh.Instantiate(meshAsset);
			meshInstance.name = meshAsset.name + meshInstanceSuffix;
			meshInstance.hideFlags = HideFlags.HideAndDontSave & ~HideFlags.DontUnloadUnusedAsset;
			//meshInstance.MarkDynamic(); //enabling this makes it impossible to write to the vertex buffers from gpu

			//Debug.Log("ensureMeshInstance " + meshAsset.name + " -> " + meshInstance.name);

			OnMeshInstanceCreated();

			return meshInstance;
		}

		public void RemoveMeshInstance()
		{
			if(TryGetComponent<SkinnedMeshRenderer>(out var smr))
			{
				if (smr.sharedMesh == null || IsMeshInstance(smr.sharedMesh))
					smr.sharedMesh = meshAsset;

				RemoveMeshInstanceAux();
				return;
			}

			if(TryGetComponent<MeshFilter>(out var mf))
			{
				if (mf.sharedMesh == null || IsMeshInstance(mf.sharedMesh))
					mf.sharedMesh = meshAsset;

				RemoveMeshInstanceAux();
				return;
			}
		}

		void RemoveMeshInstanceAux()
		{
			if (meshInstance != null)
			{
				Mesh.DestroyImmediate(meshInstance);
				meshInstance = null;
			}

			OnMeshInstanceDeleted();
		}

#if UNITY_EDITOR
		static MeshInstanceBehaviour()
		{
			PrefabStage.prefabSaving += (GameObject prefab) =>
			{
				var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
				if (prefabStage != null)
				{
					var meshInstanceBehaviours = prefab.GetComponentsInChildren<MeshInstanceBehaviour>(includeInactive: true);
					foreach (var meshInstanceBehaviour in meshInstanceBehaviours)
					{
						meshInstanceBehaviour.RemoveMeshInstance();
						//Debug.Log("reverted mesh instance for " + meshInstanceBehaviour.ToString());
					}
				}
			};

			PrefabStage.prefabSaved += (GameObject prefab) =>
			{
				var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
				if (prefabStage != null)
				{
					var meshInstanceBehaviours = prefab.GetComponentsInChildren<MeshInstanceBehaviour>(includeInactive: false);
					foreach (var meshInstanceBehaviour in meshInstanceBehaviours)
					{
						meshInstanceBehaviour.EnsureMeshInstance();
						//Debug.Log("reinstated mesh instance for " + meshInstanceBehaviour.ToString());
					}
				}
			};
		}
#endif
	}
}
