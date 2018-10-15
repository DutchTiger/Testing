using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script controls the loading circle in the UI
/// </summary>
public class LoadingCircle : MonoBehaviour
{
    private RectTransform rectComponent;
    private float rotateSpeed = 400f;
    public static Image load;
    public static Image loadingProgress;
    private GameObject load_;
    private GameObject loadingProgress_;
    private static bool isShown = false;
    
    /// <summary>
    /// Initalize the load circle as well as its rectTransform 
    /// </summary>
    private void Start()
    {
        rectComponent = GetComponent<RectTransform>();

        load_ = GameObject.FindGameObjectWithTag("Loading_Circle");
        loadingProgress_ = GameObject.FindGameObjectWithTag("Progress");

        load = load_.GetComponent<Image>();
        loadingProgress = loadingProgress_.GetComponent<Image>();

        Hide();
    }

    /// <summary>
    /// Updates the rotation 
    /// </summary>
    private void Update()
    {
        if (isShown)
        {
            rectComponent.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Hides the load circle
    /// </summary>
    public static void Hide()
    {
        load.enabled = false;
        loadingProgress.enabled = false;
        isShown = false;
    }

    /// <summary>
    /// Shows the load circle 
    /// </summary>
    public static void Show()
    {
        load.enabled = true;
        loadingProgress.enabled = true;
        isShown = true;
        
    }
}
