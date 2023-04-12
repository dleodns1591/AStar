using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Node
{
    public bool isWall; // 벽이 있는지 체크해준다.
    public Node parentNode;

    public int x = 0;
    public int y = 0;
    public int g = 0; // 이동 비용
    public int h = 0; // 예상 비용
    public int f // 최소 비용
    {
        get
        {
            return g + h;
        }
    }

    public Node(bool isWall, int x, int y)
    {
        this.isWall = isWall;
        this.x = x;
        this.y = y;
    }
}

public class Astar : MonoBehaviour
{
    [Header("맵 크기")]
    [SerializeField] Vector2Int bottomLeft;
    [SerializeField] Vector2Int topRight;

    [Header("목표")]
    [SerializeField] GameObject startPos;
    [SerializeField] GameObject endPos;

    [Header("장애물")]
    [SerializeField] GameObject wall;

    [Header("UI")]
    [SerializeField] Button wayBtn;
    [SerializeField] Button wall01Btn;
    [SerializeField] Button wall02Btn;
    [SerializeField] Button wall03Btn;

    [SerializeField] List<Node> finalNode;

    int sizeX = 0;
    int sizeY = 0;

    Node[,] nodeArray;
    Node startNode;
    Node endNode;
    Node currentNode;

    List<Node> open;
    List<Node> close;


    void Start()
    {
        Btns();
    }

    // 최단거리로 목표물을 향해 이동
    IEnumerator Move()
    {
        Vector2 start = startPos.transform.position;
        startPos.transform.position = start;

        for (int i = 0; i < finalNode.Count; i++)
        {
            startPos.transform.position = new Vector2(finalNode[i].x, finalNode[i].y);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void PathFinding()
    {
        // 배열의 크기를 구하기 위해 x,y에 +1을 해준다.
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;
        nodeArray = new Node[sizeX, sizeY];

        // 벽 
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;
                // Collider2D 컴포넌트들 중 "Wall" 태그가 있는 것이 있다면, 해당 노드는 벽이므로 isWall을 true로 설정한다.
                foreach (Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i + bottomLeft.x, j + bottomLeft.y), 0.4f))
                {
                    if (col.gameObject.CompareTag("Wall"))
                        isWall = true;
                }
                nodeArray[i, j] = new Node(isWall, i + bottomLeft.x, j + bottomLeft.y);
            }
        }

        // 초기화
        startNode = nodeArray[0, 0];
        endNode = nodeArray[(int)endPos.transform.position.x - bottomLeft.x, (int)endPos.transform.position.y - bottomLeft.y];

        open = new List<Node>() { startNode };
        close = new List<Node>();
        finalNode = new List<Node>();

        while (open.Count > 0)
        {
            // F도 같고 H도 같을 시 시계방향 순서인 윗방향으로 해준다.
            currentNode = open[0];
            for (int i = 1; i < open.Count; i++)
            {
                // 열린리스트 중 최소 비용이 현재노드의 최소비용보다 작거나 같고, 예상 비용이 작은것을 현재노드로 해준다.
                if (open[i].f <= currentNode.f && open[i].h < currentNode.h)
                    currentNode = open[i];
            }

            // 현재 노드를 열린리스트에서 지워주고 닫힌리스트에 넣어준다.
            open.Remove(currentNode);
            close.Add(currentNode);


            // 현재노드가 도착노드와 같다면
            if (currentNode == endNode)
            {
                Node endCurrentNode = endNode;
                while (endCurrentNode != startNode)
                {
                    finalNode.Add(endCurrentNode);
                    endCurrentNode = endCurrentNode.parentNode;
                }
                finalNode.Add(startNode);
                finalNode.Reverse();
            }

            // 시계방향 순서대로 확인한다.
            OpenListAdd(currentNode.x, currentNode.y + 1);
            OpenListAdd(currentNode.x + 1, currentNode.y);
            OpenListAdd(currentNode.x, currentNode.y - 1);
            OpenListAdd(currentNode.x - 1, currentNode.y);
        }
    }

    void OpenListAdd(int checkX, int checkY)
    {
        // 상하좌우 범위를 벗어나지 않고, 벽이 아니면서, 닫힌리스트에 없다면
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkY >= bottomLeft.y && checkY < topRight.y + 1 &&
            !nodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isWall && !close.Contains(nodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y]))
        {
            Node NeighborNode = nodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y];

            // 이웃노드에 넣고, 직선은 10 비용
            int moveCost = 0;
            if (currentNode.x - checkX == 0 || currentNode.y - checkY == 0)
                moveCost = currentNode.g + 10;

            // 이동비용이 이웃노드G보다 작거나 또는 열린리스트에 이웃노드가 없다면 G, H, ParentNode를 설정 후 열린리스트에 추가
            if (moveCost < NeighborNode.g || !open.Contains(NeighborNode))
            {
                NeighborNode.g = moveCost;
                NeighborNode.h = (Mathf.Abs(NeighborNode.x - endNode.x) + Mathf.Abs(NeighborNode.y - endNode.y)) * 10;
                NeighborNode.parentNode = currentNode;

                open.Add(NeighborNode);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (finalNode.Count != 0)
        {
            for (int i = 0; i < finalNode.Count - 1; i++)
                Gizmos.DrawLine(new Vector2(finalNode[i].x, finalNode[i].y), new Vector2(finalNode[i + 1].x, finalNode[i + 1].y));
        }
    }

    void Btns()
    {
        wayBtn.onClick.AddListener(() =>
        {
            PathFinding();
            StartCoroutine(Move());
        });

        wall01Btn.onClick.AddListener(() =>
        {
            for (int i = 0; i < wall.transform.childCount; i++)
                wall.transform.GetChild(i).gameObject.SetActive(false);
        });

        wall02Btn.onClick.AddListener(() =>
        {
            wall.transform.GetChild(0).gameObject.SetActive(true);
            wall.transform.GetChild(1).gameObject.SetActive(false);
        });

        wall03Btn.onClick.AddListener(() =>
        {
            wall.transform.GetChild(0).gameObject.SetActive(false);
            wall.transform.GetChild(1).gameObject.SetActive(true);
        });
    }
}
