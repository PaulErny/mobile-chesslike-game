using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHandler : MonoBehaviour
{
    private bool isCharacterClicked = false;
    private Entity currentEntity = null;
    private MapCells clickedCell = null;
    private MapCells currentCell = null;
    private float movementSpeed = 12f;
    private MapGenerator map = null;
    private GameObject selector = null;
    private List<MapCells> cellsWithinMovementRange = new List<MapCells>();
    private List<MapCells> cellsWithinAttackRange = new List<MapCells>();
    public bool isPlacedByUser = true;

    // has to be uneven
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

    // IEnumerator move(Vector3 destination) {
    //     while (transform.position != destination) {
    //         transform.position = Vector3.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);
    //         yield return new WaitForEndOfFrame();
    //     }
    // }

    // private void setSelectorVisibility(bool visible) {
    //     selector.SetActive(visible);
    // }
    
    // private void getCellsWithinMovementRange() {
    //     cellsWithinMovementRange.Clear();
    //     int maxY = movementPattern.GetLength(0);
    //     int maxX = movementPattern.GetLength(1);
    //     Vector3 currentCellPos = currentCell.cellObject.transform.position / 2;
 
    //     for (int i = 0; i < maxY; i++) {
    //         int y = i - (maxY - 1) / 2;
    //         for (int j = 0; j < maxX; j++) {
    //             int x = j - (maxX - 1) / 2;
    //             Vector3 inspectedCellPos = new Vector3(currentCellPos.x + x, 0, currentCellPos.z + y);
    //             if (inspectedCellPos.x >= 0 && inspectedCellPos.x <= map.mapWidth &&
    //                 inspectedCellPos.z >= 0 && inspectedCellPos.z <= map.mapLength &&
    //                 movementPattern[i, j] == 1) {
    //                     MapCells cell = map.GetMapCells(inspectedCellPos * 2);
    //                     if (cell.contains == null) { // cell is empty -> can move
    //                         cellsWithinMovementRange.Add(cell);
    //                     }
    //             }
    //         }
    //     }
    // }

    // private void getCellsWithinAttackRange() {
    //     cellsWithinAttackRange.Clear();
    //     int maxY = attackPattern.GetLength(0);
    //     int maxX = attackPattern.GetLength(1);
    //     Vector3 currentCellPos = currentCell.cellObject.transform.position / 2;
 
    //     for (int i = 0; i < maxY; i++) {
    //         int y = i - (maxY - 1) / 2;
    //         for (int j = 0; j < maxX; j++) {
    //             int x = j - (maxX - 1) / 2;
    //             Vector3 inspectedCellPos = new Vector3(currentCellPos.x + x, 0, currentCellPos.z + y);
    //             if (inspectedCellPos.x >= 0 && inspectedCellPos.x <= map.mapWidth &&
    //                 inspectedCellPos.z >= 0 && inspectedCellPos.z <= map.mapLength &&
    //                 attackPattern[i, j] == 1) {
    //                     MapCells cell = map.GetMapCells(inspectedCellPos * 2);
    //                     if (cell.contains != null && !cell.contains.isPlacedByUser) { // cell contains an ennemy
    //                         cellsWithinAttackRange.Add(cell);
    //                     }
    //             }
    //         }
    //     }
    // }

    // private void displayMovementPattern(bool isDisplayed) {
    //     foreach (MapCells cell in cellsWithinMovementRange) {
    //         Transform mark = cell.cellObject.transform.Find("move");
    //         if (mark && cell.contains == null) {
    //             mark.gameObject.SetActive(isDisplayed);
    //         }
    //     }


    //     // Transform mark = map.GetMapCells(inspectedCellPos * 2).cellObject.transform.Find("move");
    //     // if (mark) {
    //     //     mark.gameObject.SetActive(isDisplayed);
    //     // }
    // }

    private void Awake() {
        // selector = transform.Find("selector").gameObject;
        // setSelectorVisibility(isCharacterClicked);
    }

    // Start is called before the first frame update
    void Start()
    {
        map = GameObject.FindWithTag("MapGenerator").GetComponent<MapGenerator>();
        // ! tmp ?
        // if (map != null) {
        //     currentCell =  map.GetMapCells(transform.position);
        //     if (currentCell != null) {
        //         currentCell.contains = this;
        //     }
        //     getCellsWithinMovementRange();
        // }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) ) {
                if (currentEntity == null) { // if no cell / entity is currently selected
                    currentCell = map.GetMapCells(hit.transform.position);
                    if (currentCell != null) { // if the click is within map bounds
                        currentEntity = currentCell.contains;
                        if (currentEntity != null) {
                            // * select entity / display movement range and pattern
                            currentEntity.isClicked = true;
                            currentEntity.displayEntitySelector(true);
                            currentEntity.displayMovementPattern(true);
                        } else {
                            // TODO select cell
                        }
                    }
                } else if (currentEntity != null) { // if an entity was selected
                    MapCells newCell = map.GetMapCells(hit.transform.position);
                    if (newCell != null) { // check that the click is within map bounds
                        Entity newEntity = newCell.contains;
                        if (newEntity != null) {
                            // TODO if ennemy -> attack
                            if (newEntity.isPlacedByUser) { // * if friendy -> select instead of the current entity
                                currentEntity.displayEntitySelector(false);
                                currentEntity.displayMovementPattern(false);
                                currentEntity.isClicked = false;

                                newEntity.displayEntitySelector(true);
                                newEntity.displayMovementPattern(true);
                                newEntity.isClicked = true;

                                currentEntity = newEntity;
                            } else if (currentEntity == newEntity) { // * if newEntity == currentEntity -> de-select
                                currentEntity.displayEntitySelector(false);
                                currentEntity.displayMovementPattern(false);
                                currentEntity.isClicked = false;
                                currentEntity = null;
                                currentCell = null;
                            }
                        } else {
                            // * move if the cell is within range
                            if (currentCell != newCell && currentEntity.isInMovementRange(newCell)) {
                                // * move
                                currentEntity.moveTo(newCell);
                            }
                            // * deselect entity after moving / if click is out of range
                            currentEntity.displayEntitySelector(false);
                            currentEntity.displayMovementPattern(false);
                            currentEntity.isClicked = false;
                            currentEntity = null;
                            currentCell = null;
                        }
                    }
                }

                // if character was clicked last time and this click is on a different cell
                // if (hit.transform.position != currentCell.cellObject.transform.position) {
                    // move the character
                    // MapCells newCell = map.GetMapCells(hit.transform.position);
                    // if (newCell != null) {
                    //     if (newCell.contains == null) { // if cell is empty -> move
                            // displayMovementPattern(false);
                            // StartCoroutine(move(newCell.cellObject.transform.position));
                            // currentCell.contains = null;
                            // newCell.contains = this;
                            // currentCell = newCell;
                        // } else if (!newCell.contains.isPlacedByUser) { // if cell contains ennemy
                            // displayMovementPattern(false);
                            // ! TODO -> attack
                        // } else if (newCell.contains.isPlacedByUser) { // cell contains a friendly
                            // displayMovementPattern(false);
                        // }
                    // }
                    // isCharacterClicked = false;
                    // displayMovementPattern(false);
                    // getCellsWithinMovementRange();
                // } else if (isCharacterClicked) { // else if same cell is clicked again
                    // isCharacterClicked = false;
                    // displayMovementPattern(false);
                    // ? select cell itself / show cell infos
                // } else if (!isCharacterClicked) {
                    // if no char is selected -> 
                        // if click is on char / cell containing char -> show char infos
                        // if click is on random cell -> show cell infos
                    // if (hit.transform.name == "caracter1" ||
                    //     hit.transform.position == currentCell.cellObject.transform.position) {
                        // isCharacterClicked = true;
                        // displayMovementPattern(true);
                    // }
                // }
                // setSelectorVisibility(isCharacterClicked);
            }
        }
    }

    // / <summary>
    // / OnMouseDown is called when the user has pressed the mouse button while
    // / over the GUIElement or Collider.
    // / </summary>
    // void OnMouseDown()
    // {
    //     Debug.Log("CharacterHandler.OnMouseDown()" + transform.name + transform.transform.position);
    // }
}
