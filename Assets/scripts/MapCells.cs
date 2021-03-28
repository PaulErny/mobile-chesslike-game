using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCells
{
    public int height=0;
    public string cellType;
    public GameObject cellObject;
    public Entity contains;

    public MapCells()
    {
        cellType = "null";
    }

    public MapCells(GameObject original, string cellType , Vector3 pos, Quaternion rotation)
    {
        this.cellType = cellType.ToLower();
        cellObject = GameObject.Instantiate( original, pos, rotation );
    }

    public void Instanciate(GameObject original, string cellType, Vector3 pos, Quaternion rotation)
    {
        this.cellType = cellType.ToLower();
        cellObject = GameObject.Instantiate( original, pos, rotation ) as GameObject;
        // cellObject.Component
        // cellObject.GetComponent<Rigidbody>().detectCollisions = true;
    }

    // public void ChangeMesh(GameObject original, string cellType, Vector3 pos, Quaternion rotation)
    // {
    //     this.cellType = cellType.ToLower();
    //     Destroy(cellObject);
    //     cellObject = GameObject.Instantiate( original, pos, rotation );
    // }

    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
