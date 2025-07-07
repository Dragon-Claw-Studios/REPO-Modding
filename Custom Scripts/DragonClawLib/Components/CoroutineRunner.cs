using System.Collections;
using UnityEngine;

public static class CoroutineRunner
{
    private class Runner : MonoBehaviour { }

    private static Runner _runner;

    public static Coroutine Run(IEnumerator coroutine)
    {
        if (_runner == null)
        {
            var go = new GameObject("CoroutineRunner");
            GameObject.DontDestroyOnLoad(go);
            _runner = go.AddComponent<Runner>();
            _runner.hideFlags = HideFlags.HideAndDontSave;
        }
        return _runner.StartCoroutine(coroutine);
    }
}
