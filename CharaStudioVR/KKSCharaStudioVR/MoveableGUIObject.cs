using System;
using System.Collections.Generic;
using Studio;
using UnityEngine;

namespace KKSCharaStudioVR
{
	public class MoveableGUIObject : MonoBehaviour
	{
		public List<Action<MonoBehaviour>> onMoveLister = new List<Action<MonoBehaviour>>();

		public List<Action<MonoBehaviour>> onReleasedLister = new List<Action<MonoBehaviour>>();

		public GuideObject guideObject;

		public GuideScale guideScale;

		public ObjectCtrlInfo objectCtrlInfo;

		public Vector3 oldPos;

		public Vector3 oldRot;

		public Vector3 oldScale;

		public bool isMoveObj;

		private Renderer renderer;

		public Renderer visibleReference;

		private void Start()
		{
			renderer = GetComponent<Renderer>();
		}

		public void OnMoveStart()
		{
			if (guideObject != null)
			{
				oldPos = guideObject.changeAmount.pos;
				oldRot = guideObject.changeAmount.rot;
				oldScale = guideObject.changeAmount.scale;
			}
		}

		public void OnMoved()
		{
			foreach (Action<MonoBehaviour> item in onMoveLister)
			{
				try
				{
					item(this);
				}
				catch
				{
				}
			}
		}

		public void OnReleased()
		{
			if (guideObject != null)
			{
				if (guideScale == null)
				{
					if (guideObject.enablePos)
					{
						GuideCommand.EqualsInfo equalsInfo = new GuideCommand.EqualsInfo
						{
							dicKey = guideObject.dicKey,
							oldValue = oldPos,
							newValue = guideObject.changeAmount.pos
						};
						Singleton<UndoRedoManager>.Instance.Push(new GuideCommand.MoveEqualsCommand(new GuideCommand.EqualsInfo[1] { equalsInfo }));
					}
					if (guideObject.enableRot)
					{
						GuideCommand.EqualsInfo equalsInfo2 = new GuideCommand.EqualsInfo
						{
							dicKey = guideObject.dicKey,
							oldValue = oldRot,
							newValue = guideObject.changeAmount.rot
						};
						Singleton<UndoRedoManager>.Instance.Push(new GuideCommand.RotationEqualsCommand(new GuideCommand.EqualsInfo[1] { equalsInfo2 }));
					}
				}
				else if (guideObject.enableScale)
				{
					GuideCommand.EqualsInfo[] changeAmountInfo = new GuideCommand.EqualsInfo[1]
					{
						new GuideCommand.EqualsInfo
						{
							dicKey = guideObject.dicKey,
							oldValue = oldScale,
							newValue = guideObject.changeAmount.scale
						}
					};
					Singleton<UndoRedoManager>.Instance.Push(new GuideCommand.ScaleEqualsCommand(changeAmountInfo));
				}
			}
			foreach (Action<MonoBehaviour> item in onReleasedLister)
			{
				try
				{
					item(this);
				}
				catch
				{
				}
			}
		}

		private void Update()
		{
			if (isMoveObj)
			{
				base.transform.localScale = Vector3.one * 0.1f * global::Studio.Studio.optionSystem.manipulateSize;
			}
			if (guideScale != null)
			{
				base.transform.localScale = Vector3.one * 0.05f * global::Studio.Studio.optionSystem.manipulateSize;
			}
			if (visibleReference != null && renderer != null)
			{
				renderer.gameObject.layer = visibleReference.gameObject.layer;
			}
		}
	}
}
