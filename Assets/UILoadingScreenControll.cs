using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoadingScreenControll : MonoBehaviour
{
    void Awake()
    {
        var Loadings = Find<ILoading>();
        StartCoroutine(LoadLoadings(Loadings));
    }

    IEnumerator LoadLoadings(IEnumerable<ILoading> Loadings)
    {
        Time.timeScale = 0;
        var menuController = transform.parent.GetComponentInChildren<UIMenuControllScript>();
        menuController.enabled = false;
        foreach (var loading in Loadings)
        {
            yield return loading.LoadingRoutine();
        }
        menuController.enabled = true;
        Time.timeScale = 1;
    }

    public static List<T> Find<T>()
    {
        var foundItems = SceneManager.GetActiveScene().GetRootGameObjects().
            SelectMany(rootGO => rootGO.GetComponentsInChildren<T>());
        return foundItems.ToList();
    }
}

public interface ILoading
{
    IEnumerator LoadingRoutine();
}