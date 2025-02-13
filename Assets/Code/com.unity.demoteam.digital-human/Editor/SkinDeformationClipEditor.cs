﻿#pragma warning disable 0219

//#define SOLVE_FULL_LAPLACIAN

using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.DemoTeam.DigitalHuman
{
	[CustomEditor(typeof(SkinDeformationClip)), CanEditMultipleObjects]
	public class SkinDeformationClipEditor : Editor
	{
		static GUIStyle miniLabelAlignLeft;
		static GUIStyle miniLabelAlignRight;

		static bool framesFoldout = false;
		static Vector2 framesScroll = Vector2.zero;

		void FittedIndices_SetFromTargetMesh(object userData)
		{
			var clip = userData as SkinDeformationClip;
			var iarr = SkinDeformationFitting.GetBlendShapeIndices(clip.settings.transferTarget);

			clip.settings.fittedIndices = String.Join(",", iarr.Select(i => i.ToString()).ToArray());
		}

		void FittedIndices_SetPrecomputed(object userData)
		{
			var clip = userData as SkinDeformationClip;
			var iarr = new int[]
			{
				// all indices from snappers head
				//    0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,132,133,134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,152,153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,173,174,175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,210,211,212,213,214,215,216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,237,238,239,240,241,242,243,244,245,246,247,248,249,250,251,252,253,254,255,256,257,258,259,260,261,262,263,264,265,266,267,268,269,270,271,272,273,274,275,276,277,278,279,280,281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299,300,301,302,303,304,305,306,307,308,309,310,311,312,313,314,315,316,317
				// subtract linearly dependent indices
				//    23,106,107,108,109,176,177,178,179,296,297,298,299
				// =>
				0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,110,111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,132,133,134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,152,153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,173,174,175,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,210,211,212,213,214,215,216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,237,238,239,240,241,242,243,244,245,246,247,248,249,250,251,252,253,254,255,256,257,258,259,260,261,262,263,264,265,266,267,268,269,270,271,272,273,274,275,276,277,278,279,280,281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,300,301,302,303,304,305,306,307,308,309,310,311,312,313,314,315,316,317
			};

			clip.settings.fittedIndices = String.Join(",", iarr.Select(i => i.ToString()).ToArray());
		}

		void FittedIndices_SetPrecomputedWrinkles(object userData)
		{
			var clip = userData as SkinDeformationClip;
			var iarr = new int[]
			{
				// all indices from snappers head that contribute to wrinkle maps
				// =>
				1,14,15,44,48,51,54,59,62,67,72,80,85,87,89,91,93,95,97,110,113,116,118,119,121,123,126,129,132,135,138,141,144,147,150,158,163,181,183,184,187,191,194,197,200,207,209,219,221,227,229,230,231,232,233,235,237,240,241,250,251,252,253,254,255,256,257,258,259,264,265,290,291,292,293,294,295
			};

			clip.settings.fittedIndices = String.Join(",", iarr.Select(i => i.ToString()).ToArray());
		}

		void FittedIndices_ApplyFormattingAndSortAscending(object userData)
		{
			var clip = userData as SkinDeformationClip;
			var istr = clip.settings.fittedIndices;
			var iarr = Array.ConvertAll<string, int>(istr.Split(','), int.Parse);

			Array.Sort<int>(iarr);

			clip.settings.fittedIndices = String.Join(",", iarr.Select(i => i.ToString()).ToArray());
		}

		void FittedIndices_ApplyFilterToEnsureLinearlyIndependent(object userData)
		{
			var clip = userData as SkinDeformationClip;
			var istr = clip.settings.fittedIndices;
			var iarr = Array.ConvertAll<string, int>(istr.Split(','), int.Parse);

			Array.Sort<int>(iarr);
			iarr = SkinDeformationFitting.ComputeLinearlyIndependentBlendShapeIndices(clip.settings.transferTarget, iarr);

			if (iarr == null)
			{
				Debug.LogError("linearly independent filter did not complete");
				return;
			}

			clip.settings.fittedIndices = String.Join(",", iarr.Select(i => i.ToString()).ToArray());
		}

		void OnDisable()
		{
			SkinDeformationClipRegions.Disable();
		}

		public override void OnInspectorGUI()
		{
			if (miniLabelAlignLeft == null)
			{
				miniLabelAlignLeft = new GUIStyle(EditorStyles.miniLabel);
				miniLabelAlignLeft.alignment = TextAnchor.UpperLeft;
			}

			if (miniLabelAlignRight == null)
			{
				miniLabelAlignRight = new GUIStyle(EditorStyles.miniLabel);
				miniLabelAlignRight.alignment = TextAnchor.UpperRight;
			}

			if (targets.Length == 1)
			{
				SkinDeformationClip clip = (SkinDeformationClip)target;

				if (clip.settings.solveRegionPreview)
					SkinDeformationClipRegions.Enable(clip);
				else
					SkinDeformationClipRegions.Disable();

				EditorGUILayout.HelpBox(GetInfoString(clip), MessageType.Info, true);
				if (GUILayout.Button("Import"))
				{
					ImportClip(clip);
				}

				EditorGUILayout.Separator();
				base.OnInspectorGUI();

				EditorGUILayout.Separator();
				if (GUILayout.Button("Configure Fitted Indices ..."))
				{
					GUI.FocusControl(null);
					var menu = new GenericMenu();
					menu.AddItem(new GUIContent("Set from target mesh"), false, FittedIndices_SetFromTargetMesh, clip);
					menu.AddItem(new GUIContent("Set precomputed linearly independent"), false, FittedIndices_SetPrecomputed, clip);
					menu.AddItem(new GUIContent("Set precomputed linearly independent (wrinkles only)"), false, FittedIndices_SetPrecomputedWrinkles, clip);
					menu.AddItem(new GUIContent("Apply formatting and sort ascending"), false, FittedIndices_ApplyFormattingAndSortAscending, clip);
					menu.AddItem(new GUIContent("Apply filter to ensure linearly independent"), false, FittedIndices_ApplyFilterToEnsureLinearlyIndependent, clip);
					menu.ShowAsContext();
				}

				EditorGUILayout.Separator();
				framesFoldout = EditorGUILayout.Foldout(framesFoldout, "Frame intervals", EditorStyles.foldout);
				if (framesFoldout)
				{
					framesScroll = EditorGUILayout.BeginScrollView(framesScroll, false, true);
					for (int i = 0; i != clip.subframeCount; i++)
					{
						Rect rectGroup = EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.PrefixLabel("interval " + i, EditorStyles.miniLabel);
							{
								Rect rectLabel = GUILayoutUtility.GetLastRect();
								Rect rectFrame = new Rect(rectLabel);

								rectFrame.xMin = rectLabel.xMax;
								rectFrame.xMax = rectGroup.xMax;
								EditorGUI.DrawRect(rectFrame, Color.black);
								rectFrame.xMin += 1.0f;
								rectFrame.xMax -= 1.0f;
								rectFrame.yMin += 1.0f;
								rectFrame.yMax -= 1.0f;
								EditorGUI.DrawRect(rectFrame, Color.Lerp(Color.black, Color.green, 0.05f));

								float x0 = rectFrame.xMin + rectFrame.width * clip.subframes[i].fractionLo;
								float x1 = rectFrame.xMin + rectFrame.width * clip.subframes[i].fractionHi;

								float y0 = clip.subframes[i].fractionLo;
								float y1 = clip.subframes[i].fractionHi;

								rectFrame.xMin = x0;
								rectFrame.xMax = x1;
								//EditorGUI.DrawRect(rectFrame, Color.Lerp(Color.black, Color.green, 0.4f));

								Handles.color = Color.Lerp(Color.black, Color.green, 0.4f);
								Handles.DrawAAConvexPolygon(
									new Vector3(rectFrame.xMin, rectFrame.yMax, 0.0f),
									new Vector3(rectFrame.xMax, rectFrame.yMax, 0.0f),
									new Vector3(rectFrame.xMax, rectFrame.yMin + rectFrame.height * y1, 0.0f),
									new Vector3(rectFrame.xMin, rectFrame.yMin + rectFrame.height * y0, 0.0f)
								);

								Handles.color = Color.Lerp(Color.black, Color.magenta, 0.4f);
								Handles.DrawAAConvexPolygon(
									new Vector3(rectFrame.xMin, rectFrame.yMax - rectFrame.height * (1.0f - y0), 0.0f),
									new Vector3(rectFrame.xMax, rectFrame.yMax - rectFrame.height * (1.0f - y1), 0.0f),
									new Vector3(rectFrame.xMax, rectFrame.yMin, 0.0f),
									new Vector3(rectFrame.xMin, rectFrame.yMin, 0.0f)
								);
							}
							EditorGUILayout.TextField("keyframe " + clip.subframes[i].frameIndexLo, miniLabelAlignLeft);
							EditorGUILayout.TextField("keyframe " + clip.subframes[i].frameIndexHi, miniLabelAlignRight);
						}
						EditorGUILayout.EndHorizontal();
					}
					EditorGUILayout.EndScrollView();
				}
				else
				{
					framesScroll = Vector2.zero;
				}
			}
			else
			{
				SkinDeformationClipRegions.Disable();

				EditorGUILayout.HelpBox("Skin Deformation Clip (multiple)", MessageType.Info, true);
				if (GUILayout.Button("Import"))
				{
					foreach (var target in targets)
					{
						SkinDeformationClip clip = (SkinDeformationClip)target;
						ImportClip(clip);
					}
				}

				EditorGUILayout.Separator();
				base.OnInspectorGUI();
			}
		}

		static T[] GetAssetsAtPath<T>(string path, string name) where T : UnityEngine.Object
		{
			if (Directory.Exists(path) == false)
				return new T[0];

			var guids = AssetDatabase.FindAssets(name + " t:" + typeof(T).Name, new string[] { path });
			var paths = guids.Select(g => AssetDatabase.GUIDToAssetPath(g)).OrderBy(s => s);
			var array = paths.Select(p => AssetDatabase.LoadAssetAtPath<T>(p)).ToArray();

			return array;
		}

		static string[] GetFilesAtPath(string path, string pattern)
		{
			if (Directory.Exists(path) == false)
				return new string[0];

			var result = Directory.GetFiles(path, pattern);
			for (int i = 0; i != result.Length; i++)
			{
				result[i] = result[i].Replace('\\', '/');
			}
			Array.Sort(result);
			return result;
		}

		static string GetInfoString(bool flag)
		{
			if (flag)
				return "YES";
			else
				return "no";
		}

		static string GetInfoString(SkinDeformationClip clip)
		{
			string s = string.Empty;
			s += "Skin Deformation Clip";
			s += "\n -- # frames: " + (clip.subframeCount + 1);
			s += "\n -- # keyframes: " + clip.frameCount;
			s += "\n -- # vertices: " + clip.frameVertexCount;
			s += "\n -- contains deltas: " + GetInfoString(clip.framesContainDeltas);
			s += "\n -- contains albedo: " + GetInfoString(clip.framesContainAlbedo);
			s += "\n -- contains fitted weights: " + GetInfoString(clip.framesContainFittedWeights);
			s += "\n -- (rev " + clip.version + ")";
			return s;
		}

		public static int[] ResolveIndexArrayFromVertexSelection(TextAsset vertexSelection, MeshAdjacency weldedAdjacency = null)
		{
			if (vertexSelection == null)
				return new int[0];

			var indicesCommaSep = vertexSelection.text.Trim('[', ']');
			if (indicesCommaSep.Length > 0)
			{
				var parsed = Array.ConvertAll<string, int>(indicesCommaSep.Split(','), int.Parse);
				if (weldedAdjacency != null)
					return parsed.Select(i => weldedAdjacency.vertexResolve[i]).Distinct().ToArray();
				else
					return parsed;
			}
			else
			{
				return new int[0];
			}
		}

		public static int[] ResolveIndexArrayFromVertexSelectionArray(TextAsset[] vertexSelections, MeshAdjacency weldedAdjacency = null)
		{
			var indices = new int[0];
			foreach (var vertexSelection in vertexSelections)
			{
				indices = indices.Union(ResolveIndexArrayFromVertexSelection(vertexSelection, weldedAdjacency)).ToArray();
			}
			return indices;
		}

		static void AssertClipSources(SkinDeformationClip clip)
		{
			//TODO validation
		}


		static SharedJobData sharedJobData;
		struct SharedJobData
		{
			public string[] paths;

			public NativeMeshObjLoader.VertexAttribs vertexAttribs;
			public NativeMeshObjLoader.VertexOrder vertexOrder;

			public NativeMeshSOA[] result;
			public bool[] resultTouched;
		}

		struct ObjLoaderJob : IJobParallelFor
		{
			public void Execute(int i)
			{
				sharedJobData.resultTouched[i] = true;
				using (var sourceObj_i = NativeMeshObjLoader.Parse(sharedJobData.paths[i], Allocator.Temp, sharedJobData.vertexAttribs, sharedJobData.vertexOrder))
				{
					sharedJobData.result[i].CopyFrom(sourceObj_i);
				}
			}
		}

		static void ImportClip(SkinDeformationClip clip)
		{
			var sourceObjsPreloaded = null as NativeMeshSOA[];

			try
			{
				var progressTitle = "Importing '" + clip.name + "'";
				var progressIndex = 0;
				var progressCount = 4.0f;

				EditorUtility.DisplayProgressBar(progressTitle, "Loading assets", progressIndex++ / progressCount);

				var sourceObjPaths = null as string[];
				var sourceMeshAssets = null as Mesh[];
				var sourceAlbedoAssets = null as Texture2D[];

				var referenceObjPath = clip.settings.referenceObjPath;
				var referenceMeshAsset = clip.settings.referenceMeshAsset;
				var referenceIsFirstFrame = clip.settings.referenceIsFirstFrame;

				int frameCount = 0;
				int frameVertexCount = 0;
				int frameFaceIndicesCount = 0;

				var useExternalLoader = (clip.settings.sourceFrom == SkinDeformationClip.SourceType.ExternalObj);
				if (useExternalLoader)
				{
					sourceObjPaths = GetFilesAtPath(clip.settings.externalObjPath, clip.settings.externalObjPattern);
					Debug.Assert(sourceObjPaths.Length > 0, "source obj count == 0 (check import settings)");

					if (referenceIsFirstFrame)
					{
						referenceObjPath = sourceObjPaths[0];
						sourceObjPaths = sourceObjPaths.Skip(1).ToArray();
					}

					using (var nativeMesh = NativeMeshObjLoader.Parse(referenceObjPath))
					{
						frameCount = sourceObjPaths.Length;
						frameVertexCount = nativeMesh.vertexCount;
						frameFaceIndicesCount = nativeMesh.faceIndicesCount;
					}

					if (clip.settings.externalObjPreloadThreaded)
					{
						sharedJobData.paths = sourceObjPaths;
						sharedJobData.vertexAttribs = NativeMeshObjLoader.VertexAttribs.Position;// ignored for now
						sharedJobData.vertexOrder = NativeMeshObjLoader.VertexOrder.ByDefinition;
						sharedJobData.result = new NativeMeshSOA[sourceObjPaths.Length];
						sharedJobData.resultTouched = new bool[sourceObjPaths.Length];

						for (int i = 0; i != sharedJobData.result.Length; i++)
						{
							sharedJobData.result[i].Allocate(frameVertexCount, frameFaceIndicesCount, Allocator.Persistent);
						}

						unsafe
						{
							fixed (bool* resultTouched = sharedJobData.resultTouched)
							{
								var jobDef = new ObjLoaderJob();
								var job = jobDef.Schedule(sourceObjPaths.Length, 1);

								JobHandle.ScheduleBatchedJobs();

								while (true)
								{
									int numTotal = sourceObjPaths.Length;
									int numTouched = 0;

									for (int i = 0; i != numTotal; i++)
									{
										if (resultTouched[i])
										{
											numTouched++;
										}
									}

									EditorUtility.DisplayProgressBar(progressTitle, "Loading assets (" + numTouched + " / " + numTotal + ")", (progressIndex - 1 + ((float)numTouched / numTotal)) / progressCount);

									if (job.IsCompleted)
										break;
								}

								job.Complete();
							}
						}

						sourceObjsPreloaded = sharedJobData.result;
					}
				}
				else
				{
					sourceMeshAssets = GetAssetsAtPath<Mesh>(clip.settings.meshAssetFolder, clip.settings.meshAssetPrefix);
					Debug.Assert(sourceMeshAssets.Length > 0, "source mesh count == 0 (check import settings)");

					sourceAlbedoAssets = GetAssetsAtPath<Texture2D>(clip.settings.albedoAssetFolder, clip.settings.albedoAssetPrefix);
					if (sourceAlbedoAssets.Length != sourceMeshAssets.Length)
					{
						sourceAlbedoAssets = null;
						Debug.LogWarning("source albedo count != mesh count (SKIPPING albedo assets)");
					}

					if (referenceIsFirstFrame)
					{
						referenceMeshAsset = sourceMeshAssets[0];
						sourceMeshAssets = sourceMeshAssets.Skip(1).ToArray();

						if (sourceAlbedoAssets != null)
							sourceAlbedoAssets = sourceAlbedoAssets.Skip(1).ToArray();
					}

					Debug.Assert(referenceMeshAsset != null);

					frameCount = sourceMeshAssets.Length;
					frameVertexCount = referenceMeshAsset.vertexCount;
				}

				Debug.Log("frameCount: " + frameCount);
				Debug.Log("frameVertexCount: " + frameVertexCount);

				int frameFittedWeightsCount = 0;// modified later
				var frames = new SkinDeformation[frameCount];

				int subframeCount = frameCount - 1;
				var subframes = new SkinDeformationClip.Subframe[subframeCount];

				MeshBuffers meshBuffers = new MeshBuffers(frameVertexCount);
				MeshBuffers meshBuffersReference = new MeshBuffers(frameVertexCount);

				MeshAdjacency weldedAdjacency = new MeshAdjacency(meshBuffersReference, clip.settings.solveWelded);

				EditorUtility.DisplayProgressBar(progressTitle, "Importing frames", progressIndex++ / progressCount);
				{
					var sourceRotation = Quaternion.Euler(clip.settings.applyRotation);
					var sourceScale = clip.settings.applyScaling;

					if (useExternalLoader)
					{
						using (var referenceObj = NativeMeshObjLoader.Parse(referenceObjPath))
						{
							meshBuffersReference.LoadFrom(referenceObj);
							meshBuffersReference.ApplyRotation(sourceRotation);
							meshBuffersReference.ApplyScale(sourceScale);
						}
					}
					else
					{
						meshBuffersReference.LoadFrom(referenceMeshAsset);
						meshBuffersReference.ApplyRotation(sourceRotation);
						meshBuffersReference.ApplyScale(sourceScale);
					}

					var denoiseIndices = ResolveIndexArrayFromVertexSelectionArray(clip.settings.denoiseRegions, weldedAdjacency);
					var denoiseFactor = clip.settings.denoiseStrength;
					if (denoiseFactor < float.Epsilon)
						denoiseIndices = new int[0];

					var transplantIndices = ResolveIndexArrayFromVertexSelectionArray(clip.settings.transplantRegions, weldedAdjacency);
					var transplantFactor = clip.settings.transplantStrength;
					if (transplantFactor < float.Epsilon)
						transplantIndices = new int[0];

#if SOLVE_FULL_LAPLACIAN
					var laplacianConstraintCount = frameVertexCount;
					var laplacianConstraintIndices = null as int[];

					unsafe
					{

						using (var laplacianFreeVertexMap = new UnsafeArrayBool(frameVertexCount))
						{
							laplacianFreeVertexMap.Clear(false);

							for (int k = 0; k != denoiseIndices.Length; k++)
							{
								if (laplacianFreeVertexMap.val[denoiseIndices[k]] == false)
								{
									laplacianFreeVertexMap.val[denoiseIndices[k]] = true;
									laplacianConstraintCount--;
								}
							}

							for (int k = 0; k != transplantIndices.Length; k++)
							{
								if (laplacianFreeVertexMap.val[transplantIndices[k]] == false)
								{
									laplacianFreeVertexMap.val[transplantIndices[k]] = true;
									laplacianConstraintCount--;
								}
							}

							laplacianConstraintIndices = new int[laplacianConstraintCount];

							for (int i = 0, k = 0; i != frameVertexCount; i++)
							{
								if (laplacianFreeVertexMap.val[i] == false)
									laplacianConstraintIndices[k++] = i;
							}
						}
					}
#else
					var laplacianROIIndices = denoiseIndices.Union(transplantIndices).ToArray();
					var laplacianConstraintCount = frameVertexCount - laplacianROIIndices.Length;
#endif

#if SOLVE_FULL_LAPLACIAN
					var laplacianTransform = null as MeshLaplacianTransform;
#else
					var laplacianTransform = null as MeshLaplacianTransformROI;
#endif
					var meshLaplacian = new MeshLaplacian();
					var meshLaplacianDenoised = new MeshLaplacian();
					var meshLaplacianReference = new MeshLaplacian();

					var laplacianResolve = (laplacianConstraintCount < frameVertexCount);
					if (laplacianResolve)
					{
#if SOLVE_FULL_LAPLACIAN
						laplacianTransform = new MeshLaplacianTransform(weldedAdjacency, laplacianConstraintIndices);
#else
						laplacianTransform = new MeshLaplacianTransformROI(weldedAdjacency, laplacianROIIndices, 0);
						{
							for (int i = 0; i != denoiseIndices.Length; i++)
								denoiseIndices[i] = laplacianTransform.internalFromExternal[denoiseIndices[i]];
							for (int i = 0; i != transplantIndices.Length; i++)
								transplantIndices[i] = laplacianTransform.internalFromExternal[transplantIndices[i]];
						}
#endif
						laplacianTransform.ComputeMeshLaplacian(meshLaplacianDenoised, meshBuffersReference);
						laplacianTransform.ComputeMeshLaplacian(meshLaplacianReference, meshBuffersReference);
					}

					for (int i = 0; i != frameCount; i++)
					{
						EditorUtility.DisplayProgressBar(progressTitle, "Importing frames (" + (i + 1) + " / " + frameCount + ")", (progressIndex - 1 + ((float)i / frameCount)) / progressCount);

						if (useExternalLoader)
						{
							if (clip.settings.externalObjPreloadThreaded)
							{
								meshBuffers.LoadFrom(sourceObjsPreloaded[i]);
							}
							else using (var sourceObj_i = NativeMeshObjLoader.Parse(sourceObjPaths[i]))
							{
								meshBuffers.LoadFrom(sourceObj_i);
							}

							meshBuffers.ApplyRotation(sourceRotation);
							meshBuffers.ApplyScale(sourceScale);
						}
						else
						{
							meshBuffers.LoadFrom(sourceMeshAssets[i]);
							meshBuffers.ApplyRotation(sourceRotation);
							meshBuffers.ApplyScale(sourceScale);
						}

						if (meshBuffers.vertexCount != frameVertexCount)
						{
							Debug.LogWarning("frame " + i + " has " + meshBuffers.vertexCount + " vertices (expected " + frameVertexCount + ")");
						}

						if (laplacianResolve)
						{
							laplacianTransform.ComputeMeshLaplacian(meshLaplacian, meshBuffers);

							double historyFactor = denoiseFactor;
							foreach (int j in denoiseIndices)
							{
								double dx = denoiseFactor * meshLaplacianDenoised.vertexDifferentialX[j] + (1.0 - denoiseFactor) * meshLaplacian.vertexDifferentialX[j];
								double dy = denoiseFactor * meshLaplacianDenoised.vertexDifferentialY[j] + (1.0 - denoiseFactor) * meshLaplacian.vertexDifferentialY[j];
								double dz = denoiseFactor * meshLaplacianDenoised.vertexDifferentialZ[j] + (1.0 - denoiseFactor) * meshLaplacian.vertexDifferentialZ[j];
								meshLaplacian.vertexDifferentialX[j] = dx;
								meshLaplacian.vertexDifferentialY[j] = dy;
								meshLaplacian.vertexDifferentialZ[j] = dz;
								meshLaplacianDenoised.vertexDifferentialX[j] = dx;
								meshLaplacianDenoised.vertexDifferentialY[j] = dy;
								meshLaplacianDenoised.vertexDifferentialZ[j] = dz;
							}

							foreach (int j in transplantIndices)
							{
								meshLaplacian.vertexDifferentialX[j] = transplantFactor * meshLaplacianReference.vertexDifferentialX[j] + (1.0 - transplantFactor) * meshLaplacian.vertexDifferentialX[j];
								meshLaplacian.vertexDifferentialY[j] = transplantFactor * meshLaplacianReference.vertexDifferentialY[j] + (1.0 - transplantFactor) * meshLaplacian.vertexDifferentialY[j];
								meshLaplacian.vertexDifferentialZ[j] = transplantFactor * meshLaplacianReference.vertexDifferentialZ[j] + (1.0 - transplantFactor) * meshLaplacian.vertexDifferentialZ[j];
							}

							laplacianTransform.ResolveMeshBuffers(meshBuffers, meshLaplacian);

							meshBuffers.RecalculateNormals(weldedAdjacency);
							meshBuffers.ApplyWeldedChanges(weldedAdjacency);
						}

						frames[i].SetAlbedo(sourceAlbedoAssets != null ? sourceAlbedoAssets[i] : null);
						frames[i].SetDeltas(meshBuffersReference, meshBuffers);
					}

					for (int i = 0; i != subframeCount; i++)
					{
						subframes[i].frameIndexLo = i;
						subframes[i].frameIndexHi = i + 1;
						subframes[i].fractionLo = 0.0f;
						subframes[i].fractionHi = 1.0f;
					}

					if (clip.settings.keyframes)
					{
						ImportFrameIntervalsFromCSV(clip.settings.keyframesCSV, frameCount - 1, ref subframeCount, ref subframes);
					}
				}

				EditorUtility.DisplayProgressBar(progressTitle, "Transferring frames", progressIndex++ / progressCount);
				if (clip.settings.transferTarget != null)
				{
					var targetBuffers = new MeshBuffers(clip.settings.transferTarget);
					var targetVertexCount = targetBuffers.vertexCount;
					var targetVertexResolve = new int[targetVertexCount];

					Debug.LogFormat("transferMode: {0}", clip.settings.transferMode);
					Debug.LogFormat("targetVertexCount: {0}", targetVertexCount);

					switch (clip.settings.transferMode)
					{
						case SkinDeformationClip.ImportSettings.TransferMode.ByVertexIndex:
							{
								Debug.Assert(frameVertexCount == targetVertexCount, "target vertex count does not match reference vertex count");
								for (int i = 0; i != targetVertexCount; i++)
								{
									targetVertexResolve[i] = i;
								}
							}
							break;

						case SkinDeformationClip.ImportSettings.TransferMode.ByVertexPosition:
							{
								var referenceBSP = new KdTree3(meshBuffersReference.vertexPositions, meshBuffersReference.vertexCount);
								var referenceDelta2Max = 0.0f;
								var referenceDelta2Count = 0;
								var referenceDelta2Threshold = 1e-10f;

								for (int i = 0; i != targetVertexCount; i++)
								{
									targetVertexResolve[i] = referenceBSP.FindNearest(ref targetBuffers.vertexPositions[i]);
								}

								for (int i = 0; i != targetVertexCount; i++)
								{
									Vector3 posTarget = targetBuffers.vertexPositions[i];
									Vector3 posReference = meshBuffersReference.vertexPositions[targetVertexResolve[i]];

									var delta2 = Vector3.SqrMagnitude(posTarget - posReference);
									if (delta2 > referenceDelta2Threshold)
									{
										referenceDelta2Max = Mathf.Max(delta2, referenceDelta2Max);
										referenceDelta2Count++;
									}
								}

								if (referenceDelta2Count > 0)
								{
									Debug.LogWarning("some (" + referenceDelta2Count + ") target vertices were far from reference (max delta: " + referenceDelta2Max + ")");
								}
							}
							break;
					}

					var targetDeltaPositions = new Vector3[targetVertexCount];
					var targetDeltaNormals = new Vector3[targetVertexCount];

					for (int frameIndex = 0; frameIndex != frameCount; frameIndex++)
					{
						EditorUtility.DisplayProgressBar(progressTitle, "Transferring frames (" + (frameIndex + 1) + " / " + frameCount + ")", (progressIndex - 1 + ((float)frameIndex / frameCount)) / progressCount);

						for (int i = 0; i != targetVertexCount; i++)
						{
							int j = targetVertexResolve[i];// reference index

							targetDeltaPositions[i] = frames[frameIndex].deltaPositions[j];
							targetDeltaNormals[i] = frames[frameIndex].deltaNormals[j];
						}

						ArrayUtils.ResizeChecked(ref frames[frameIndex].deltaPositions, targetVertexCount);
						ArrayUtils.ResizeChecked(ref frames[frameIndex].deltaNormals, targetVertexCount);

						ArrayUtils.CopyChecked(targetDeltaPositions, ref frames[frameIndex].deltaPositions, targetVertexCount);
						ArrayUtils.CopyChecked(targetDeltaNormals, ref frames[frameIndex].deltaNormals, targetVertexCount);
					}

					frameVertexCount = targetVertexCount;

					/*
					switch (clip.importSettings.transferMode)
					{
						case SkinDeformationClip.TransferMode.PassThrough:
							{
								// clean copy
							}
							break;

						case SkinDeformationClip.TransferMode.PassThroughWithFirstFrameDelta:
							{
								buffersFrameX.LoadFrom(clip.importSettings.transferTarget);

								Vector3 anchorOrigin = buffersFrame0.CalcMeshCenter();
								Vector3 anchorTarget = buffersFrameX.CalcMeshCenter();
								Vector3 offsetTarget = anchorOrigin - anchorTarget;
								for (int j = 0; j != frameVertexCount; j++)
								{
									buffersFrameX.vertexPositions[j] += offsetTarget;
								}

								SkinDeformation firstFrameDelta;
								firstFrameDelta = new SkinDeformation();
								firstFrameDelta.SetDeltas(buffersFrameX, buffersFrame0);

								for (int i = 0; i != frameCount; i++)
								{
									EditorUtility.DisplayProgressBar(progressTitle, "Retargeting frames", (progressIndex - 1 + ((float)i / frameCount)) / progressCount);

									for (int j = 0; j != frameVertexCount; j++)
									{
										frames[i].deltaPositions[j] += firstFrameDelta.deltaPositions[j];
										frames[i].deltaNormals[j] += firstFrameDelta.deltaNormals[j];
									}
								}
							}
							break;
					}
					*/
				}

				EditorUtility.DisplayProgressBar(progressTitle, "Fitting frames to blend shapes", progressIndex++ / progressCount);
				if (clip.settings.transferTarget != null)
				{
					if (clip.settings.fitToBlendShapes)
						frameFittedWeightsCount = clip.settings.transferTarget.blendShapeCount;
					else
						frameFittedWeightsCount = 0;

					for (int i = 0; i != frameCount; i++)
						frames[i].fittedWeights = new float[frameFittedWeightsCount];

					if (frameFittedWeightsCount > 0)
					{
						var blendShapeIndicesCommaSep = clip.settings.fittedIndices;
						var blendShapeIndices = Array.ConvertAll<string, int>(blendShapeIndicesCommaSep.Split(','), int.Parse);

						SkinDeformationFitting.FitFramesToBlendShapes(frames, clip.settings.transferTarget, blendShapeIndices, clip.settings.fittingMethod, clip.settings.fittingParam);
					}
				}
				else
				{
					for (int i = 0; i != frameCount; i++)
						frames[i].fittedWeights = new float[0];
				}

				EditorUtility.DisplayProgressBar(progressTitle, "Saving binary", progressIndex++ / progressCount);
				{
					clip.settingsLastImported = clip.settings.Clone();
					clip.frameCount = frameCount;
					clip.frameVertexCount = frameVertexCount;
					clip.frameFittedWeightsCount = frameFittedWeightsCount;
					clip.frames = frames;
					clip.framesContainAlbedo = (frames[0].albedo != null);
					clip.framesContainDeltas = (frames[0].deltaPositions.Length > 0);
					clip.framesContainFittedWeights = (frames[0].fittedWeights.Length > 0);
					clip.subframeCount = subframeCount;
					clip.subframes = subframes;
					clip.version++;

					EditorUtility.SetDirty(clip);
					AssetDatabase.SaveAssets();

					clip.SaveFrameData();
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex);
			}
			finally
			{
				if (sourceObjsPreloaded != null)
				{
					for (int i = 0; i != sourceObjsPreloaded.Length; i++)
					{
						sourceObjsPreloaded[i].Dispose();
					}

					sourceObjsPreloaded = null;
				}

				EditorUtility.ClearProgressBar();
			}
		}

		static void ImportFrameIntervalsFromCSV(TextAsset textAsset, int maxFrameIndex, ref int subframeCount, ref SkinDeformationClip.Subframe[] subframes)
		{
			if (textAsset == null)
				return;

			// skip 1st column of every row
			const int SKIP_COL = 1;
			const int NO_VALUE = -1;

			// rows[0] == comma sep. original frame #
			// rows[1] == comma sep. wrap frame #
			// rows[2] == comma sep. frame progress 0-100
			const int ROW_ORIG = 0;
			const int ROW_KEYS = 1;
			const int ROW_TIME = 2;
			const int NUM_ROWS = 3;

			var vals = new int[NUM_ROWS][];
			var valCount = NO_VALUE;

			using (StringReader reader = new StringReader(textAsset.text))
			{
				for (int row = 0; row != NUM_ROWS; row++)
				{
					var line = reader.ReadLine();
					if (line == null)
						return;// bad input, missing rows

					var svals = line.Split(',');
					var svalCount = svals.Length;
					if (svalCount != valCount + SKIP_COL && valCount != NO_VALUE)
						return;// bad input, rows not equal length

					valCount = svalCount - SKIP_COL;
					vals[row] = new int[valCount];
					for (int j = 0; j != valCount; j++)
					{
						if (!int.TryParse(svals[j + SKIP_COL], out vals[row][j]))
							vals[row][j] = NO_VALUE;
					}
				}
			}

			int keyIndex0 = NO_VALUE;
			int keyIndexN = NO_VALUE;

			for (int i = 0; i != valCount; i++)
			{
				int frameIndex = vals[ROW_KEYS][i];
				if (frameIndex != NO_VALUE)
				{
					if (frameIndex > maxFrameIndex)
						break;// ignore rest of sequence

					keyIndexN = i;
					if (keyIndex0 == NO_VALUE)
						keyIndex0 = i;
				}
			}

			if (keyIndex0 == keyIndexN)
				return;// bad input, need at least two keyframes

			subframeCount = keyIndexN - keyIndex0;
			subframes = new SkinDeformationClip.Subframe[subframeCount];

			int keyIndexLo = keyIndex0;
			int keyIndexHi = keyIndex0 + 1;

			while (keyIndexLo < keyIndexN)
			{
				while (vals[ROW_KEYS][keyIndexHi] == NO_VALUE)
					keyIndexHi++;

				int keyLo = vals[ROW_KEYS][keyIndexLo];
				int keyHi = vals[ROW_KEYS][keyIndexHi];

				float n = keyIndexHi - keyIndexLo;
				for (int i = keyIndexLo; i != keyIndexHi; i++)
				{
					subframes[i].frameIndexLo = keyLo;
					subframes[i].frameIndexHi = keyHi;
					subframes[i].fractionLo = (i - keyIndexLo + 0) / (float)n;
					subframes[i].fractionHi = (i - keyIndexLo + 1) / (float)n;
				}

				keyIndexLo = keyIndexHi;
				keyIndexHi = keyIndexHi + 1;
			}
		}
	}
}
