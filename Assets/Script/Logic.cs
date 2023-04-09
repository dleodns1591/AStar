using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour
{
    Node[,] node;
    Node startNode;
    Node endNode;
    Node currentNode;

    public List<Node> open = new List<Node>();
    public List<Node> close = new List<Node>();

    public Vector2Int startLocation; // ������ġ
    public Vector2Int endLocation; // ������ġ

    public int sizeX = 0; // �� Xũ��
    public int sizeY = 0; // �� Yũ��

    void Start()
    {
        Test();
    }

    void Update()
    {

    }

    public void Test()
    {
        CreatNode(sizeX, sizeY);
        SetTergetLocation(startLocation, endLocation);
        PathFinding();
    }

    // x,y ũ�⿡ �´� ��� �迭
    public void CreatNode(int sizeX, int sizeY)
    {
        node = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false; // ���� �� üũ
                node[i, j] = new Node(isWall, i, j);
            }
        }
    }

    // ���������� ��ǥ����
    public void SetTergetLocation(Vector2Int start, Vector2Int end)
    {
        startNode = node[start.x, start.y];
        endNode = node[end.x, end.y];
    }

    // �� ã�� �˰���
    public void PathFinding()
    {
        open.Clear(); // �� �� �ִ� �� ���
        close.Clear(); // �ѹ� �鸥 ���

        // ���� �������� ����
        open.Add(startNode);

        while (open.Count > 0)
        {
            // �� �� �ִ� �� �߿� ����ġ�� ���� ���� ã�´�.
            currentNode = open[0];

            for (int i = 1; i < open.Count; i++)
            {
                if (open[i].f <= currentNode.f && open[i].h < currentNode.h)
                    currentNode = open[i];
            }

            // ã�� ���� �� �� �ִ� �� ��Ͽ��� �����.
            // �� �� �鸥 �� ��Ͽ� �־��ش�.
            // �̷ν� �� �� ���� ���� �ߺ����� �˻��ϴ� ���� ��������.
            open.Remove(currentNode);
            close.Add(currentNode);

            // �������� �����ߴ��� üũ�Ѵ�.
            if (currentNode == endNode)
            {
                for (int i = 0; i < close.Count; i++)
                    Debug.Log(i + "��°�� " + close[i].x + ", " + close[i].y);
            }

            // �� �� �ִ� ���� ã�Ƽ� ��Ͽ� �־��ش�.
            // ������ �Ϲ������� ��, ������, �Ʒ�, ���� ���̴�.
            OpenListAdd(currentNode.x, currentNode.y + 1);
            OpenListAdd(currentNode.x + 1, currentNode.y);
            OpenListAdd(currentNode.x, currentNode.y - 1);
            OpenListAdd(currentNode.x - 1, currentNode.y);
        }
    }

    void OpenListAdd(int checkX, int checkY)
    {
        // �����¿� ������ ����ٸ� �������´�.
        if (checkX < 0 || checkX >= node.GetLength(0) || checkY < 0 || checkY >= node.GetLength(1))
            return;

        // �� �� �鸥 ���̸� �������´�.
        if (close.Contains(node[checkX, checkY]))
            return;

        Node neighborNode = node[checkX, checkY];
        int cost = currentNode.g + (currentNode.x - checkX == 0 || currentNode.y - checkY == 0 ? 10 : 14);

        if (cost < neighborNode.g || !open.Contains(neighborNode))
        {
            neighborNode.g = cost;
            neighborNode.h = (Mathf.Abs(neighborNode.x - endNode.x) + Mathf.Abs(neighborNode.y - endNode.y)) * 10;
            neighborNode.parentNode = currentNode;

            open.Add(neighborNode);
        }
    }

}
