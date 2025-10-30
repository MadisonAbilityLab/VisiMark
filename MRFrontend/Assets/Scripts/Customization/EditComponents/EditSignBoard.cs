using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditSignBoard : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        
    }

    public void SetSize(float _size)
    {
        this.transform.localScale = new Vector3(_size, _size, _size);
    }

    public float GetSize()
    {
        return this.transform.localScale.x;
    }
}
