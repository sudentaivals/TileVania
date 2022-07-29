using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCanvas : MonoBehaviour
{
    public void OpenCanvas()
    {
        foreach (Transform tr in transform)
        {
            tr.gameObject.SetActive(true);
        }
    }

    public void CloseCanvas()
    {
        foreach (Transform tr in transform)
        {
            tr.gameObject.SetActive(false);
        }

    }
}
