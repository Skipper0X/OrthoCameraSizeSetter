using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GCore.Runtime.Common
{
	[RequireComponent(typeof(Camera))]
	public sealed class CameraOrthoSizeSetter : MonoBehaviour
	{
		/// <summary>
		/// <see cref="SizeMatchingType"/> Defines Which Ration To Match While Scaling...
		/// </summary>
		private enum SizeMatchingType
		{
			ByWidth = 0,
			ByHeight = 1,
			ByBounds = 2,
		}

		[SerializeField, BoxGroup] private SizeMatchingType _sizeMatch = default;
		[SerializeField, BoxGroup, Range(-5.0f, 5.0f)] private float _orthoSizeOffSet = 0;
		[SerializeField, BoxGroup, ReadOnly] private Vector2Int _screenRes = Vector2Int.zero;

		[Title("Refs:", "Set Camera Ortho Size According To Given Sprite Renderer Bounds.")]
		[SerializeField, Required] private Camera _orthoCamera = default;
		[SerializeField, Required] private Renderer _targetRenderer = default;

		private void Awake() => _screenRes = new Vector2Int(Screen.width, Screen.height);

		private void Start() => SetCameraOrthoSize();

		[Button(ButtonStyle.CompactBox)]
		private void SetCameraOrthoSize()
		{
			var orthoSize = _sizeMatch switch
			{
				SizeMatchingType.ByWidth => GetSizeByWidth(),
				SizeMatchingType.ByHeight => GetSizeByHeight(),
				SizeMatchingType.ByBounds => GetSizeByBounds(),
				_ => throw new ArgumentOutOfRangeException()
			};

			_orthoCamera.orthographicSize = (orthoSize + _orthoSizeOffSet);
		}

		private float GetSizeByHeight() => _targetRenderer.bounds.size.y / 2.0f;
		private float GetSizeByWidth() => _targetRenderer.bounds.size.x * _screenRes.y / _screenRes.x * 0.5f;

		private float GetSizeByBounds()
		{
			var screenRatio = (float) _screenRes.x / (float) _screenRes.y;

			var targetSize = _targetRenderer.bounds.size;
			var targetRatio = targetSize.x / targetSize.y;

			if (screenRatio >= targetRatio) return GetSizeByHeight();

			var diffInSize = targetRatio / screenRatio;
			return GetSizeByHeight() * diffInSize;
		}
	}
}