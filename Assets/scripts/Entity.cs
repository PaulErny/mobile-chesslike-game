using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public bool isPlacedByUser = true;
    public bool isClicked {
        get {return m_isClicked;}
        set {
            if (m_isClicked == value) return;
            m_isClicked = value;
            OnIsClickedChange();
        }
    }
    private bool m_isClicked = false;
    private MapGenerator map = null;
    private MapCells currentCell = null;
    private GameObject selector = null;
    private float movementSpeed = 12f;
    private short[,] movementPattern = new short[5,5] {{0, 1, 1, 1, 0}, 
                                                       {1, 1, 1, 1, 1},
                                                       {1, 1, 0, 1, 1},
                                                       {1, 1, 1, 1, 1},
                                                       {0, 1, 1, 1, 0}
                                                       };
    private short[,] attackPattern = new short[5,5] {{0, 1, 1, 1, 0}, 
                                                     {1, 1, 1, 1, 1},
                                                     {1, 1, 0, 1, 1},
                                                     {1, 1, 1, 1, 1},
                                                     {0, 1, 1, 1, 0}
                                                    };

    private List<MapCells> getCellsWithinMovementRange() {
        List<MapCells> cellsWithinMovementRange = new List<MapCells>();
        // cellsWithinMovementRange.Clear();
        int maxY = movementPattern.GetLength(0);
        int maxX = movementPattern.GetLength(1);
        Vector3 currentCellPos = currentCell.cellObject.transform.position / 2;
 
        for (int i = 0; i < maxY; i++) {
            int y = i - (maxY - 1) / 2;
            for (int j = 0; j < maxX; j++) {
                int x = j - (maxX - 1) / 2;
                Vector3 inspectedCellPos = new Vector3(currentCellPos.x + x, 0, currentCellPos.z + y);
                if (inspectedCellPos.x >= 0 && inspectedCellPos.x <= map.mapWidth &&
                    inspectedCellPos.z >= 0 && inspectedCellPos.z <= map.mapLength &&
                    movementPattern[i, j] == 1) {
                        MapCells cell = map.GetMapCells(inspectedCellPos * 2);
                        if (cell.contains == null) { // cell is empty -> can move
                            cellsWithinMovementRange.Add(cell);
                        }
                }
            }
        }
        return cellsWithinMovementRange;
    }

    private List<MapCells> getCellsWithinAttackRange() {
        // cellsWithinAttackRange.Clear();
        List<MapCells> cellsWithinAttackRange = new List<MapCells>();
        int maxY = attackPattern.GetLength(0);
        int maxX = attackPattern.GetLength(1);
        Vector3 currentCellPos = currentCell.cellObject.transform.position / 2;
 
        for (int i = 0; i < maxY; i++) {
            int y = i - (maxY - 1) / 2;
            for (int j = 0; j < maxX; j++) {
                int x = j - (maxX - 1) / 2;
                Vector3 inspectedCellPos = new Vector3(currentCellPos.x + x, 0, currentCellPos.z + y);
                if (inspectedCellPos.x >= 0 && inspectedCellPos.x <= map.mapWidth &&
                    inspectedCellPos.z >= 0 && inspectedCellPos.z <= map.mapLength &&
                    attackPattern[i, j] == 1) {
                        MapCells cell = map.GetMapCells(inspectedCellPos * 2);
                        if (cell.contains != null && !cell.contains.isPlacedByUser) { // cell contains an ennemy
                            cellsWithinAttackRange.Add(cell);
                        }
                }
            }
        }
        return cellsWithinAttackRange;
    }

    public void displayMovementPattern(bool isDisplayed) {
        List<MapCells> cellsWithinMovementRange = getCellsWithinMovementRange();

        foreach (MapCells cell in cellsWithinMovementRange) {
            Transform mark = cell.cellObject.transform.Find("move");
            if (mark && cell.contains == null) {
                mark.gameObject.SetActive(isDisplayed);
            }
        }
    }

    public void displayEntitySelector(bool isDisplayed) {
        selector.SetActive(isDisplayed);
    }

    public bool isInMovementRange(MapCells cell) {
        List<MapCells> cellsWithinMovementRange = getCellsWithinMovementRange();

        if (cellsWithinMovementRange.Contains(cell)) {
            return true;
        }
        return false;
    }

    public void moveTo(MapCells newCell) {
        StartCoroutine(move(newCell.cellObject.transform.position));
        currentCell.contains = null;
        newCell.contains = this;
    }

    IEnumerator move(Vector3 destination) {
        while (transform.position != destination) {
            transform.position = Vector3.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    // * called on isClicked change (setter)
    private void OnIsClickedChange() {
        // TODO display the entity's infos UI
    }

    private void Awake() {
        selector = transform.Find("selector").gameObject;
        displayEntitySelector(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        map = GameObject.FindWithTag("MapGenerator").GetComponent<MapGenerator>();
        currentCell = map.GetMapCells(transform.position);
        currentCell.contains = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
