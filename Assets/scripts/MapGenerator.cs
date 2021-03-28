using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapLength=19;
    public int mapWidth=19;
    public GameObject grass1;
    public GameObject grass2;
    public GameObject sand;
    public GameObject water1;
    public GameObject water2;
    [Range(0, 100)]
    public int obstaclesPercent=60;
    [Range(0, 20)]
    public int riverIterations=3;

    private List<List<MapCells>> map = new List<List<MapCells>>();
    private List<List<bool>> isObstacles = new List<List<bool>>();
    private List<List<GameObject>> obstacles = new List<List<GameObject>>(); //tmp

    private void visualizeObstacles()
    {
        for (int z = 0; z < mapLength; z++) {
            obstacles.Add( new List<GameObject>() );
            for (int x = 0; x < mapWidth; x++) {
                if ( isObstacles[z][x] == true ) {
                    obstacles[z].Add( GameObject.CreatePrimitive(PrimitiveType.Cube) );
                    obstacles[z][x].transform.position = new Vector3(2*x, 0.21f, 2*z);
                    obstacles[z][x].transform.localScale = new Vector3(2, 0.1f, 2);
                } else {
                    obstacles[z].Add( null );
                }
            }
        }
    }

    private void destroyVisibleObstacles()
    {
        for (int z = mapLength - 1; z >= 0; z--) {
            for (int x = mapWidth - 1; x >= 0; x--) {
                Destroy(obstacles[z][x]);
                obstacles[z].RemoveAt(x);
            }
            obstacles[z].Clear();
            obstacles.RemoveAt(z);
        }
        obstacles.Clear();
        obstacles = new List<List<GameObject>>();
    }

    private int countAdjacentObstacles(int x, int z)
    {
        int count = 0;
    
        if (isObstacles[z][x] == true) {
            if (z != 0 && isObstacles[z - 1][x] == true)
                count++;
            if (z != mapLength - 1 && isObstacles[z + 1][x] == true)
                count++;
            if (x != 0 && isObstacles[z][x - 1] == true)
                count++;
            if (x != mapWidth - 1 && isObstacles[z][x + 1] == true)
                count++;
            return (count);
        }

        return (-1);
    }

    private void removeIsolated()
    {
        destroyVisibleObstacles();
        for (int z = 0; z < mapLength; z++) {
            for (int x = 0; x < mapWidth; x++) {
                int adjacentObstacles = countAdjacentObstacles(x, z);
                if ( adjacentObstacles != -1 && adjacentObstacles < 2 ) {
                    isObstacles[z][x] = false;
                }
            }
        }
        visualizeObstacles();
    }

    private void RemoveLayer()
    {
        destroyVisibleObstacles();
        List<Vector2> toRm = new List<Vector2>();
        for (int z = 0; z < mapLength; z++) {
            for (int x = 0; x < mapWidth; x++) {
                if (isObstacles[z][x] == false) {
                    if (z != 0 && isObstacles[z - 1][x] == true) {
                        toRm.Add(new Vector2(x, z - 1));
                    }
                    if (z != mapLength - 1 && isObstacles[z + 1][x] == true) {
                        toRm.Add(new Vector2(x, z + 1));
                    }
                    if (x != 0 && isObstacles[z][x - 1] == true) {
                        toRm.Add(new Vector2(x - 1, z));
                    }
                    if (x != mapWidth - 1 && isObstacles[z][x + 1] == true) {
                        toRm.Add(new Vector2(x + 1, z));
                    }
                }
            }
        }
        foreach (Vector2 coord in toRm) {
            isObstacles[ (int)coord.y ][ (int)coord.x ] = false;
        }
        visualizeObstacles();
    }

    private void addLayer()
    {
        destroyVisibleObstacles();
        List<Vector2> toAdd = new List<Vector2>();
        for (int z = 0; z < mapLength; z++) {
            for (int x = 0; x < mapWidth; x++) {
                if (isObstacles[z][x] == true) {
                    if (z != 0 && isObstacles[z - 1][x] == false) {
                        toAdd.Add(new Vector2(x, z - 1));
                    }
                    if (z != mapLength - 1 && isObstacles[z + 1][x] == false) {
                        toAdd.Add(new Vector2(x, z + 1));
                    }
                    if (x != 0 && isObstacles[z][x - 1] == false) {
                        toAdd.Add(new Vector2(x - 1, z));
                    }
                    if (x != mapWidth - 1 && isObstacles[z][x + 1] == false) {
                        toAdd.Add(new Vector2(x + 1, z));
                    }
                }
            }
        }
        foreach (Vector2 coord in toAdd) {
            isObstacles[ (int)coord.y ][ (int)coord.x ] = true;
        }
        visualizeObstacles();
    }

    private void CreateShores2()
    {
        for (int y = 0; y < mapLength; y++) {
            for (int x = 0; x < mapWidth; x++) {
                if ((x - 1 >= 0 && map[y][x - 1].cellType.Equals("sand") ||
                    x + 1 < mapWidth && map[y][x + 1].cellType.Equals("sand") ||
                    y - 1 >= 0 && map[y - 1][x].cellType.Equals("sand") ||
                    y + 1 < mapLength && map[y + 1][x].cellType.Equals("sand")) &&
                    map[y][x].cellType.Equals("null"))
                {
                    map[y][x].Instanciate(grass2, "grass2", new Vector3(2*x, 0, 2*y), Quaternion.identity);
                    if (isObstacles[y][x] == true)
                        isObstacles[y][x] = false;
                }
            }
        }
    }

    private void CreateShores(List<PathNode> waterCells)
    {
        foreach (PathNode cell in waterCells) {
            // left
            if (cell.x - 1 >= 0 && map[cell.y][cell.x - 1].cellType.Equals("null")) {
                map[cell.y][cell.x - 1].Instanciate(sand, "sand", new Vector3(2*(cell.x - 1), 0, 2*cell.y), Quaternion.identity);
                if (isObstacles[cell.y][cell.x - 1] == true)
                    isObstacles[cell.y][cell.x - 1] = false;
            }
            // right
            if (cell.x + 1 < mapWidth && map[cell.y][cell.x + 1].cellType.Equals("null")) {
                map[cell.y][cell.x + 1].Instanciate(sand, "sand", new Vector3(2*(cell.x + 1), 0, 2*cell.y), Quaternion.identity);
                if (isObstacles[cell.y][cell.x + 1] == true)
                    isObstacles[cell.y][cell.x + 1] = false;
            }
            // up
            if (cell.y - 1 >= 0 && map[cell.y - 1][cell.x].cellType.Equals("null")) {
                map[cell.y - 1][cell.x].Instanciate(sand, "sand", new Vector3(2*cell.x, 0, 2*(cell.y - 1)), Quaternion.identity);
                if (isObstacles[cell.y - 1][cell.x] == true)
                    isObstacles[cell.y - 1][cell.x] = false;
            }
            // down
            if (cell.y + 1 < mapLength && map[cell.y + 1][cell.x].cellType.Equals("null")) {
                map[cell.y + 1][cell.x].Instanciate(sand, "sand", new Vector3(2*cell.x, 0, 2*(cell.y + 1)), Quaternion.identity);
                if (isObstacles[cell.y + 1][cell.x] == true)
                    isObstacles[cell.y + 1][cell.x] = false;
            }
        }
    }

    private bool CreateRivers()
    {
        // create rivers
        bool isRiver = false;
        List<PathNode> waterCells = new List<PathNode>();
        List<Vector2> addedObstacles = new List<Vector2>();
        int riverBgn = (int)Random.Range(6, 13);
        int riverEnd = (int)Random.Range(6, 13);
        
        for (int it = 0; it < riverIterations; it++) {
            PathFinding pf= new PathFinding(mapWidth, mapLength);
            List<PathNode> path = pf.FindPath(isObstacles, 0, riverBgn, 18, riverEnd);
            if (path != null) {
                isRiver = true;
                for (int i = 0; i < path.Count - 1; i++) {
                    waterCells.Add(path[i]);
                    map[path[i].y][path[i].x].Instanciate(water1, "water1", new Vector3(2*path[i].x, 0, 2*path[i].y), Quaternion.identity);
                    if (Random.Range(0, 100) < 50 && !map[path[i].y][path[i].x].cellType.Equals("null") &&
                        path[i].y != riverBgn && path[i].y != riverEnd) {
                        isObstacles[path[i].y][path[i].x] = true;
                        addedObstacles.Add( new Vector2(path[i].x, path[i].y) );
                    }
                }
            }
        }
        foreach (PathNode cell in waterCells) {
            if (cell.x - 1 >= 0 && (map[cell.y][cell.x - 1].cellType.Equals("water1") || map[cell.y][cell.x - 1].cellType.Equals("water2")) &&
                cell.x + 1 < mapWidth && (map[cell.y][cell.x + 1].cellType.Equals("water1") || map[cell.y][cell.x + 1].cellType.Equals("water2")) &&
                cell.y - 1 >= 0 && (map[cell.y - 1][cell.x].cellType.Equals("water1") || map[cell.y - 1][cell.x].cellType.Equals("water2")) &&
                cell.y + 1 < mapLength && (map[cell.y + 1][cell.x].cellType.Equals("water1") || map[cell.y + 1][cell.x].cellType.Equals("water2")))
            {
                Destroy(map[cell.y][cell.x].cellObject);
                map[cell.y][cell.x].Instanciate(water2, "water2", new Vector3(2*cell.x, 0, 2*cell.y), Quaternion.identity);
            }
        }
        foreach (Vector2 obstacle in addedObstacles) {
            isObstacles[(int)obstacle.y][(int)obstacle.x] = false;
        }
        CreateShores(waterCells);
        CreateShores2();
        return (isRiver);
    }

    private void RaiseObstacles()
    {
        bool isObstaclesLeft = true;

        while (isObstaclesLeft) {
            isObstaclesLeft = false;
            for (int z = 0; z < mapLength; z++) {
                for (int x = 0; x < mapWidth; x++) {
                    if (isObstacles[z][x]) {
                        isObstaclesLeft = true;
                        map[z][x].cellObject.transform.position = new Vector3(map[z][x].cellObject.transform.position.x, 
                                                                              map[z][x].cellObject.transform.position.y + 0.5f,
                                                                              map[z][x].cellObject.transform.position.z);
                    }
                }
            }
            RemoveLayer();
        }
    }

    private void CreateGround()
    {
        if (CreateRivers()) {
            // create ground
            for (int z = 0; z < mapLength; z++) {
                for (int x = 0; x < mapWidth; x++) {
                    if (map[z][x].cellType.Equals("null"))
                        map[z][x].Instanciate( grass1, "grass1", new Vector3(2*x, 0, 2*z), Quaternion.identity );
                }
            }
        } else {
            // if there are no rivers -> create different ground
            for (int z = 0; z < mapLength; z++) {
                for (int x = 0; x < mapWidth; x++) {
                    if (map[z][x].cellType.Equals("null") && isObstacles[z][x] == false)
                        map[z][x].Instanciate( grass2, "grass2", new Vector3(2*x, 0, 2*z), Quaternion.identity );
                    else if (map[z][x].cellType.Equals("null"))
                        map[z][x].Instanciate( grass1, "grass1", new Vector3(2*x, 0, 2*z), Quaternion.identity );
                }
            }
        }
        RaiseObstacles();
    }

    private static Vector3 GetMouseGridPos() {
        Debug.Log("debug " + Input.mousePosition);
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // pos.z = 0f;
        return pos;
    }

    // Start is called before the first frame update
    void Start()
    {
        // init map grid
        for (int z = 0; z < mapLength; z++) {
            map.Add( new List<MapCells>() );
            for (int x = 0; x < mapWidth; x++) {
                map[z].Add(new MapCells());
                // map[z][x].transform.position = new Vector3(2*x, 0, 2*z);
            }
        }

        // create random obstacles
        for (int z = 0; z < mapLength; z++) {
            isObstacles.Add( new List<bool>() );
            for (int x = 0; x < mapWidth; x++) {
                int randNb = Random.Range(0, 100);
                if ( randNb <= obstaclesPercent ) {
                    isObstacles[z].Add( true );
                } else {
                    isObstacles[z].Add( false );
                }
            }
        }

        visualizeObstacles(); // tmp
        // improve obstacles
        removeIsolated();
        addLayer();

        CreateGround();

    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.A)) {
        //     removeIsolated();
        // }

        // if (Input.GetKeyDown(KeyCode.Z)) {
        //     addLayer();
        // }

        // if (Input.GetKeyDown(KeyCode.E)) {
        //     RemoveLayer();
        // }
        if (Input.GetMouseButtonDown(0)) {
            // Debug.Log(GetMouseGridPos());

            // Cast a ray from screen point
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Save the info
            RaycastHit hit;
            // You successfully hit
            if (Physics.Raycast (ray, out hit)) {
                // Find the direction to move in
                // Vector3 dir = hit.point - transform.position;
                
                // Make it so that its only in x and y axis
                // dir.z = 0; // No vertical movement
                Debug.Log("grid " + hit.transform.name);
                Debug.Log("pos " + hit.transform.position);
                // Debug.Log(hit.)
                // Now move your character in world space 
                // transform.Translate (dir * Time.DeltaTime * speed, Space.World);
                
                // transform.Translate (dir * Time.DeltaTime * speed); // Try this if it doesn't work
            }
        }
    }

}
