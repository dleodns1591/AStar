using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Node
{
    public bool isWall; // ���� �ִ��� üũ���ش�.
    public Node parentNode;

    public int x = 0;
    public int y = 0;
    public int g = 0; // �̵� ���
    public int h = 0; // ���� ���
    public int f // �ּ� ���
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
    [Header("�� ũ��")]
    [SerializeField] Vector2Int bottomLeft;
    [SerializeField] Vector2Int topRight;

    [Header("��ǥ")]
    [SerializeField] GameObject startPos;
    [SerializeField] GameObject endPos;

    [Header("��ֹ�")]
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

    // �ִܰŸ��� ��ǥ���� ���� �̵�
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
        // �迭�� ũ�⸦ ���ϱ� ���� x,y�� +1�� ���ش�.
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;
        nodeArray = new Node[sizeX, sizeY];

        // �� 
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;
                // Collider2D ������Ʈ�� �� "Wall" �±װ� �ִ� ���� �ִٸ�, �ش� ���� ���̹Ƿ� isWall�� true�� �����Ѵ�.
                foreach (Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i + bottomLeft.x, j + bottomLeft.y), 0.4f))
                {
                    if (col.gameObject.CompareTag("Wall"))
                        isWall = true;
                }
                nodeArray[i, j] = new Node(isWall, i + bottomLeft.x, j + bottomLeft.y);
            }
        }

        // �ʱ�ȭ
        startNode = nodeArray[0, 0];
        endNode = nodeArray[(int)endPos.transform.position.x - bottomLeft.x, (int)endPos.transform.position.y - bottomLeft.y];

        open = new List<Node>() { startNode };
        close = new List<Node>();
        finalNode = new List<Node>();

        while (open.Count > 0)
        {
            // F�� ���� H�� ���� �� �ð���� ������ ���������� ���ش�.
            currentNode = open[0];
            for (int i = 1; i < open.Count; i++)
            {
                // ��������Ʈ �� �ּ� ����� �������� �ּҺ�뺸�� �۰ų� ����, ���� ����� �������� ������� ���ش�.
                if (open[i].f <= currentNode.f && open[i].h < currentNode.h)
                    currentNode = open[i];
            }

            // ���� ��带 ��������Ʈ���� �����ְ� ��������Ʈ�� �־��ش�.
            open.Remove(currentNode);
            close.Add(currentNode);


            // �����尡 �������� ���ٸ�
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

            // �ð���� ������� Ȯ���Ѵ�.
            OpenListAdd(currentNode.x, currentNode.y + 1);
            OpenListAdd(currentNode.x + 1, currentNode.y);
            OpenListAdd(currentNode.x, currentNode.y - 1);
            OpenListAdd(currentNode.x - 1, currentNode.y);
        }
    }

    void OpenListAdd(int checkX, int checkY)
    {
        // �����¿� ������ ����� �ʰ�, ���� �ƴϸ鼭, ��������Ʈ�� ���ٸ�
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkY >= bottomLeft.y && checkY < topRight.y + 1 &&
            !nodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isWall && !close.Contains(nodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y]))
        {
            Node NeighborNode = nodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y];

            // �̿���忡 �ְ�, ������ 10 ���
            int moveCost = 0;
            if (currentNode.x - checkX == 0 || currentNode.y - checkY == 0)
                moveCost = currentNode.g + 10;

            // �̵������ �̿����G���� �۰ų� �Ǵ� ��������Ʈ�� �̿���尡 ���ٸ� G, H, ParentNode�� ���� �� ��������Ʈ�� �߰�
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
