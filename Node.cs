
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MapGenerator;

public class Node : MonoBehaviour
{
        public Vector3 position; // 节点的位置
        public NodeType type=NodeType.Battle; // 节点的类型
        public Image ui;
        public Sprite icon;
        public List<Node> upperNodes; // 节点的上层节点
        public List<Node> lowerNodes; // 节点的下层节点
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
