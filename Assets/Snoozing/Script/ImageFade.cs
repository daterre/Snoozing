﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public static class ImageFade
{
	static Dictionary<Component, Coroutine> _fades = new Dictionary<Component, Coroutine>();

	public static YieldInstruction Start(Graphic img, float from, float to, float time, System.Action onComplete = null)
	{
		Stop(img);
		var coroutine = img.StartCoroutine(AnimateGraphic(img, from, to, time, onComplete));
		_fades.Add(img, coroutine);
		return coroutine;
	}

	public static YieldInstruction Start(CanvasGroup group, MonoBehaviour script, float from, float to, float time, System.Action onComplete = null)
	{
		Stop(group, script);
		var coroutine = script.StartCoroutine(AnimateGroup(group, from, to, time, onComplete));
		_fades.Add(group, coroutine);
		return coroutine;
	}

	public static void Stop(Graphic img)
	{
		Coroutine current;
		if (!_fades.TryGetValue(img, out current))
			return;

		if (current != null)
			img.StopCoroutine(current);
		_fades.Remove(img);
	}

	public static void Stop(CanvasGroup group, MonoBehaviour script)
	{
		Coroutine current;
		if (!_fades.TryGetValue(group, out current))
			return;

		script.StopCoroutine(current);
		_fades.Remove(group);
	}

	public static bool IsInProgress(Component img)
	{
		return _fades.ContainsKey(img);
	}

	static IEnumerator AnimateGraphic(Graphic img, float from, float to, float time, System.Action onComplete = null)
	{
		Color color = img.color;
		float state = (color.a - from) / (to - from);

		for (float t = state * time; t <= time; t += Time.deltaTime)
		{
			color = img.color;
			color.a = Mathf.Lerp(from, to, t / time);
			img.color = color;
			yield return null;
		}

		color = img.color;
		color.a = to;
		img.color = color;
		_fades.Remove(img);

		if (onComplete != null)
			onComplete.Invoke();
	}

	static IEnumerator AnimateGroup(CanvasGroup group, float from, float to, float time, System.Action onComplete = null)
	{
		float state = (group.alpha - from) / (to - from);
		string name = group.ToString();

		for (float t = state * time; t <= time; t += Time.deltaTime)
		{
			try { group.alpha = Mathf.Lerp(from, to, t / time); }
			catch (MissingReferenceException) { Debug.LogErrorFormat("Still trying to animate '{0}' even though its gone", name); }
			yield return null;
		}

		group.alpha = to;
		_fades.Remove(group);

		if (onComplete != null)
			onComplete.Invoke();
	}
}