using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParamAnimation : MonoBehaviour
{
    [SerializeField] private string paramName;
    [SerializeField] private float value;
    void Start()
    {
        GetComponent<Animator>().SetFloat(paramName, value);
        // :p
    }
}
