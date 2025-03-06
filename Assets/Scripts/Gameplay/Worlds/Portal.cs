using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private void Start()
    {
        //ShowLockedPortal();
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void ShowLockedPortal()
    {

    }

    public void Activate()
    {
        //StartCoroutine(ActivatePortalCoroutine());
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private IEnumerator ActivatePortalCoroutine()
    {
        throw new NotImplementedException();
    }

}
