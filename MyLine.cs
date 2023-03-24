using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MyLine : MonoBehaviour
{


    public Image imageprefab;
    List<Vector3> positions;
    List<Image> images=new List<Image>();
    public int PositionCount
    {
        get
        {
            return positions.Count;
        }
        set
        {
            positions = new List<Vector3>(value);
        }
           
    }

    public void  SetPosition(int index,Vector3 position)
    {
        if(index<0)
        {
            Debug.Log("error Value");
            return;
        }

        if(index==0)
        {
            positions.Add(position);
        }else
        {
            positions.Add(position);
            Image go =Instantiate(imageprefab,this.transform);
            go.transform.position = position;
            go.transform.rotation = Quaternion.FromToRotation(Vector3.up, (positions[index] - positions[index-1]).normalized);
            images.Add(go);
        }

    }

}
