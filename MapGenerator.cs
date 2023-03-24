
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    // 定义一个枚举，表示节点的类型
    public enum NodeType
    {
        Battle = 0, // 战斗
        Shop = 1, // 商店
        Rest = 2, // 营火
        Elite = 3, // 精英
        Random = 4,//随机

        //不出现
        Treasure = 5, // 宝箱
        Boss =6,

    }
    private bool isCompelete = false;
    float battleProb = 0.35f; // 战斗节点的概率
    float shopProb = 0.1f; // 商店节点的概率
    float restProb = 0.15f; // 营火节点的概率
    float eliteProb = 0.15f;//精英节点的概率
    float randomProb = 0.25f; //随机事件概率

    // 定义一些常量，表示地图的参数
    [Header("地图参数")]
    public  int LAYERS = 5; // 地图的层数
    public  int LAYER_NODES = 7; // 每层的节点数
    const int MAXNODES = 6;
    const int MINNODES = 2;
    const float RANDOMRANGE_ICON = 40;  //Icon
    const float RANDOMRANGE_LINE = 8;   //line



    // 定义一些变量，表示节点的概率和限制
    int minBattlePerLayer = 1; // 每层的最少战斗节点数
    int maxTreasurePerLayer = 1; // 每层的最多宝箱节点数
    int maxShopPerLayer = 1; // 每层的最多商店节点数
    int maxRestPerLayer = 1; // 每层的最多营火节点数
        

    // 定义一个列表，存储所有的节点

    List<List<Node>> nodes = new List<List<Node>>();

    // 定义一个随机数生成器

    public Sprite[] icons;
    public Node currentNode = null;
    [Header("Prefab")]
    public GameObject linePrefab;  //道路连线
    public GameObject nodePrefab;  //节点图像
    public Transform NodesParent;  //父节点
    public Transform LineParent;   //道路连线层级
    public Sprite Boss;            //特殊节点
    public GameObject RoadMapPrefab;  
    public GameObject CurrentMap;

   
    /// <summary>
    /// 为 子节点和父节点建立 连接
    /// </summary>
    /// <param name="layer">层数</param>
    /// <param name="i">第几个上层节点</param>
    /// <param name="father">父节点</param>
    /// <param name="max">可选取的上线</param>
    /// <param name="min">可选取的下线</param>
    public void Awake()
    {
       StartCoroutine("IE_InitMap");

    }
    private void GenerateNodes()
    {

        Debug.Log("1111");
        if (nodes[LAYERS - 1][LAYER_NODES / 2] != null)
            SetBoss(nodes[LAYERS - 1][LAYER_NODES / 2]);
        else
        {
            Debug.Log("StartNode Is Null");
            return;
        }
        int MaxNode = MAXNODES;
        for (int i = LAYERS - 2; i >= 0; i--)
        {
            ConnectNodes(i, ref MaxNode);
        }


    }
    private void ConnectNodes(int layer, ref int MaxNode)
    {
        //先根据上次的节点 生成这轮节点的数量   是这样的 上一次的preNode的数量*2 是总共可接受的数量
        if (MaxNode * 2 >= MAXNODES)
        {
            MaxNode = MAXNODES;
        }
        else
        {
            MaxNode *= 2;
        }
        int MinNode = Mathf.Max((MaxNode + 1) / 2, MINNODES);

        //定义当前层接受的节点数
        int ContainCount = Random.Range(MinNode, MaxNode);
        MaxNode = ContainCount;
        int UpIndex = 0;

        foreach (var father in nodes[layer + 1])
        {

            if (father.IsSeleced == false)
            {
                UpIndex++;
                continue;
            }
            if (father.type == NodeType.Boss)  //Boss类型特殊照顾
            {
                for (int j = 0; j < MaxNode; j++)
                {
                    //随机挑选
                    GenerateConnection(layer, UpIndex, father, 7, 0, ref ContainCount);
                }
                break;
            }
            int upperNodeCount = Random.Range(1,4);
            // 如果是最外层的节点，只能连接一个下层节点
            if (UpIndex == 0 || UpIndex == LAYER_NODES - 1)
            {
                upperNodeCount = 1;
            }
            for (int j = 0; j < upperNodeCount; j++)
            {
                int max = 0;
                int min = UpIndex;
                if (UpIndex == 0)
                {
                    min = UpIndex;
                    max = UpIndex + 2;
                    GenerateConnection(layer, UpIndex, father, max, min, ref ContainCount);
                 
                }
                else if (UpIndex == LAYER_NODES - 1)
                {
                    min = UpIndex - 1;
                    max = UpIndex+1;
                    GenerateConnection(layer, UpIndex, father, max, min, ref ContainCount);
                
                }
                else
                {
                    min = UpIndex - 1;
                    max = UpIndex + 2;
                    GenerateConnection(layer, UpIndex, father, max, min, ref ContainCount);
                    
                }
            }
            UpIndex++;

        }
    }
    private void GenerateConnection(int layer, int UpIndex, Node father, int max, int min,ref int  Count)
    {
        int index = Random.Range(min, max);
        int limit = 0;
    
        if(Count==0)  //已经用完了所有的节点  //那么寻找最后一个子节点 连线
        {
            Node lastChildNode=new Node(Vector2.zero);
            foreach(var temp in nodes[layer])
            {
                if(temp.IsSeleced==true)
                {
                    lastChildNode = temp;
                }
            }
            if (father.lowerNodes.Contains(lastChildNode))
                return;
            father.lowerNodes.Add(lastChildNode);
            lastChildNode.upperNodes.Add(father);
            return;

        }

        Debug.LogFormat("Layer[{0}]:Index[{1}]", layer, index);
        while (nodes[layer][index].IsSeleced == true||IsCrossing(layer,index,UpIndex))
        {
            index = Random.Range(min, max);
            limit++;
            if (limit > 15)
            {
                return;
            }
        }

        if (nodes[layer][index].IsSeleced ==false)
        {
            nodes[layer][index].IsSeleced = true;
            Count--;
        }
       ;
        //建立连接
        father.lowerNodes.Add(nodes[layer][index]);
        nodes[layer][index].upperNodes.Add(father);

    }
    private bool IsCrossing(int layer, int lowerIndex, int upperIndex)
    {
        if(lowerIndex==upperIndex)
        {
            return false;
        }else//lowIndex==upperIndex-1
        {
            if(lowerIndex==upperIndex-1)
            {
                if (nodes[layer + 1][upperIndex - 1].IsSeleced == true)
                {
                    if (nodes[layer + 1][upperIndex - 1].lowerNodes.Contains(nodes[layer][upperIndex]))
                    {
                        return true;
                    }

                }
            }
            if(lowerIndex==upperIndex+1)
            {
                if (nodes[layer + 1][upperIndex+1].IsSeleced == true)
                {
                    if (nodes[layer + 1][upperIndex + 1].lowerNodes.Contains(nodes[layer][upperIndex]))
                    {
                        return true;
                    }

                }
            }
        }

        return false;
    }
   // 定义一个方法，根据概率和限制，为每层的节点随机分配一个类型
    private void AssignNodeType()
    {

        for(int i=0;i<LAYERS-1;i++)
        {
            if(i==0)  //第一层必然是普通怪物
            {
                foreach(var node in nodes[i])
                {
                    if (node.IsSeleced == false)
                        continue;
                    node.type = NodeType.Battle;
                }
                continue;
            }

            if(i==LAYERS-2)//倒数第二层必然是休息节点
            {
                foreach (var node in nodes[i])
                {
                    if (node.IsSeleced == false)
                        continue;
                    node.type = NodeType.Rest;
                }
                continue;
            }

            if(i==(int)(LAYERS/2f+1)) //必然是宝箱节点
            {
                foreach (var node in nodes[i])
                {
                    if (node.IsSeleced == false)
                        continue;
                    node.type = NodeType.Treasure;
                }
                continue;
            }




            // 定义一个数组，存储每种类型的节点数
            int[]typeCount = new int[5];
            List<Node> thisLayerNode = new List<Node>();
            foreach (var node in nodes[i])
            {
                if (node.IsSeleced == false)
                    continue;
                float r = (float)Random.Range(0f, 1f);
                thisLayerNode.Add(node);
                if (r < battleProb)
                {
                    node.type = NodeType.Battle;
                    typeCount[(int)NodeType.Battle]++;
                }
                else if (r < battleProb + shopProb)
                {
                    node.type = NodeType.Shop;
                    typeCount[(int)NodeType.Shop]++;
                }
                else if (r < battleProb + restProb+ shopProb)
                {
                    node.type = NodeType.Rest;
                    typeCount[(int)NodeType.Rest]++;

                }
                else if(r< battleProb + restProb + shopProb+eliteProb)
                {
                    node.type = NodeType.Elite;
                    typeCount[(int)NodeType.Elite]++;
                }else
                {
                    node.type = NodeType.Random;
                    typeCount[(int)NodeType.Random]++;
                }
            }


            //while (typeCount[(int)NodeType.Battle] < minBattlePerLayer)
            //{
            //    int index = Random.Range(0, LAYERS - 1);
            //    if (thisLayerNode[index].type != NodeType.Battle)
            //    {
            //        thisLayerNode[index].type = NodeType.Battle;
            //        typeCount[(int)NodeType.Battle]++;
            //        typeCount[(int)System.Enum.Parse(typeof(NodeType), thisLayerNode[index].type.ToString())]--;
            //    }
            //}
            // 如果商店节点多于最大值，随机替换一个商店节点
            while (typeCount[(int)NodeType.Shop] > maxShopPerLayer)
            {
                int index = Random.Range(0, thisLayerNode.Count - 1);
                if (thisLayerNode[index].type == NodeType.Shop)
                {
                    thisLayerNode[index].type = NodeType.Battle;
                    typeCount[(int)NodeType.Battle]++;
                    typeCount[(int)NodeType.Shop]--;
                }
            }
            // 如果营火节点多于最大值，随机替换一个营火节点
            while (typeCount[(int)NodeType.Rest] > maxRestPerLayer)
            {
                int index = Random.Range(0, thisLayerNode.Count - 1);


                if (thisLayerNode[index].type == NodeType.Rest)
                {
                    thisLayerNode[index].type = NodeType.Battle;
                    typeCount[(int)NodeType.Battle]++;
                    typeCount[(int)NodeType.Rest]--;
                }
            }
        }
    }
    void AssignNodeIcon()
    {
        for (int i = 0; i < LAYERS - 1; i++)
        {
            foreach (var node in nodes[i])
            {
                if (node.IsSeleced == false)
                    continue;

                switch (node.type)
                {
                    case NodeType.Battle:
                        node.icon = icons[0];
                        break;
                    case NodeType.Treasure:
                        node.icon = icons[5];
                        break;
                    case NodeType.Shop:
                        node.icon = icons[1];
                        break;
                    case NodeType.Rest:
                        node.icon = icons[2];
                        break;
                    case NodeType.Elite:
                        node.icon = icons[3];
                        break;
                    case NodeType.Random:
                        node.icon = icons[4];
                        break;
                }
            }
      
        }
          
        
    }
    //// 定义一个方法，为每个节点创建一个UI元素
    void CreateNodeUI()
    {
        for (int i = 0; i < LAYERS - 1; i++)
        {
            foreach (var node in nodes[i])
            {
                if (node.IsSeleced == false)
                    continue;
                GameObject nodeUI = Instantiate(nodePrefab, node.transform);
                nodeUI.transform.position += new Vector3(Random.Range(-RANDOMRANGE_ICON, RANDOMRANGE_ICON), Random.Range(-RANDOMRANGE_ICON, RANDOMRANGE_ICON));
                node.position = nodeUI.transform.position;
                // 获取UI元素上的Image组件，设置其sprite为节点的图标
                Image image = nodeUI.GetComponent<Image>();
                image.sprite = node.icon;
                node.ui = image;
                // 获取UI元素上的Button组件，添加一个点击事件，调用SelectNode方法
                ///Button button = nodeUI.GetComponent<Button>();
                //button.onClick.AddListener(() => SelectNode(node));
                // 将UI元素赋值给节点的ui属性
                // node.ui = nodeUI;
            }
        }

    }
    // 定义一个方法，为每对相连的节点创建一条UI线段
    void CreateLineUI()
    {
        for (int i = 0; i < LAYERS;)
        {
            foreach (var node in nodes[i])
            {
                if (node.IsSeleced == false)
                    continue;
                foreach (Node lowerNode in node.lowerNodes)
                {
                    // 创建一条UI线段，作为两个节点之间的图形表示
                    GameObject lineUI = Instantiate(linePrefab,LineParent);
                    // 获取UI线段上的LineRenderer组件，设置其起点和终点为两个节点的位置
                    MyLine myline = lineUI.GetComponent<MyLine>();
                    GeneratePositions(myline, node.position, lowerNode.position);
                    // 将UI线段加入节点的lineUIs列表
                    node.lineUIs.Add(lineUI);
                }
            }
            i++;
        }
    }
    void GeneratePositions(MyLine line,Vector3 source,Vector3 Destination)
    {
        float segement= (int)((Destination-source).magnitude/30);
        line.PositionCount=(int)segement+1;
        for(float i=0;i<=segement;i++)
        {
                Vector3 temp = Vector3.Lerp(source, Destination, i / segement);
                temp += new Vector3(Random.Range(-RANDOMRANGE_LINE, RANDOMRANGE_LINE), Random.Range(-RANDOMRANGE_LINE, RANDOMRANGE_LINE));
                line.SetPosition((int)i, temp);
        }
    }
   

    //未实现的功能，这个项目就是实现一下地图的功能 暂时没必要实现这个选择节点的功能
    void SelectNode(Node node)
    {
        // 如果当前的节点不为空，将其UI元素的颜色设为白色
        if (currentNode != null)
        {
            currentNode.ui.color = Color.white;
        }
        // 将传入的节点赋值给当前的节点
        currentNode = node;
        // 将当前的节点的UI元素的颜色设为红色
        currentNode.ui.color = Color.red;
        // 遍历每个节点
        //for (int i = 0; i < nodes.Count; i++)
        //{
        //     获取当前的节点
        //    Node otherNode = nodes[i];
        //     如果该节点不是当前的节点，且不在当前的节点的下层节点列表中，将其UI元素设为不可交互
        //    if (otherNode != currentNode && !currentNode.lowerNodes.Contains(otherNode))
        //    {
        //        otherNode.ui.GetComponent<Button>().interactable = false;
        //    }
        //     否则，将其UI元素设为可交互
        //    else
        //    {
        //        otherNode.ui.GetComponent<Button>().interactable = true;
        //    }
        //}
    }

   

    public void SetBoss(Node node)
    {
        GameObject nodeUI = Instantiate(nodePrefab, node.transform);
        nodeUI.transform.position += new Vector3(0, 100,0);
        node.type = NodeType.Boss;
        node.IsSeleced = true;
        node.icon = Boss;
        Image image = nodeUI.GetComponent<Image>();
        image.sprite = node.icon;
        node.ui = image;
        image.SetNativeSize();
    }

    // 定义一个方法，开始游戏
    public void StartGame()
    {
        Debug.Log("Start");
        StartCoroutine("IE_StartGame");
    }
    public void Clear()
    {
        Debug.Log("Clear");
        StartCoroutine("IE_InitMap");
    }
    private IEnumerator IE_InitMap()
    {
        isCompelete = false;

        
        if(CurrentMap!=null)
        Destroy(CurrentMap.gameObject);

        int cout = LineParent.transform.childCount;
        if(cout!=0)
        {
            foreach (Transform child in LineParent) { Object.Destroy(child.gameObject); }
 
        }
        yield return new WaitForSeconds(0.5f);


        CurrentMap = Instantiate(RoadMapPrefab, NodesParent);
        nodes.Clear();
        List<Node> tempNodes = new List<Node>();
        tempNodes.AddRange(NodesParent.GetComponentsInChildren<Node>(true));

        for (int i = 0; i < LAYERS; i++)
        {
            nodes.Add(new List<Node>());
            for (int j = 0; j < LAYER_NODES; j++)
            {
                nodes[i].Add(tempNodes[i * LAYER_NODES + j]);
            }
        }

        isCompelete=true;

    }
    private IEnumerator IE_StartGame()
    {
        yield return new WaitUntil(() => this.isCompelete == true);

        // 生成节点
        GenerateNodes();
        //为每个节点分配类型
        AssignNodeType();
        // 为每个节点随机分配一个图标
        AssignNodeIcon();
        // 为每个节点创建一个UI元素
        CreateNodeUI();
        // 为每对相连的节点创建一条UI线段
        CreateLineUI();
        // 选择第一个节点，作为当前的节点
    }





}



