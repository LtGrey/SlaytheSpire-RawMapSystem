
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MapGenerator;

public class Node : MonoBehaviour
{
        public Vector3 position; // �ڵ��λ��
        public NodeType type=NodeType.Battle; // �ڵ������
        public Image ui;
        public Sprite icon;
        public List<Node> upperNodes; // �ڵ���ϲ�ڵ�
        public List<Node> lowerNodes; // �ڵ���²�ڵ�
        public bool IsSeleced = false;
        public bool IsActice=false;
        public List<GameObject> lineUIs;
        public Node(Vector2 position, NodeType type=NodeType.Battle)
        {
            this.position = position;
            this.type = type;
            this.upperNodes = new List<Node>();
            this.lowerNodes = new List<Node>();
        }


    public void Start()
    {
        this.icon = null;
        position = this.transform.position;
        this.upperNodes = new List<Node>();
        this.lowerNodes = new List<Node>();
    }

    public void SetTrue()
    {
        this.ui.color = new Color(0, 0, 0, 1);
    }





}
