
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    // ����һ��ö�٣���ʾ�ڵ������
    public enum NodeType
    {
        Battle = 0, // ս��
        Shop = 1, // �̵�
        Rest = 2, // Ӫ��
        Elite = 3, // ��Ӣ
        Random = 4,//���

        //������
        Treasure = 5, // ����
        Boss =6,

    }
    private bool isCompelete = false;
    float battleProb = 0.35f; // ս���ڵ�ĸ���
    float shopProb = 0.1f; // �̵�ڵ�ĸ���
    float restProb = 0.15f; // Ӫ��ڵ�ĸ���
    float eliteProb = 0.15f;//��Ӣ�ڵ�ĸ���
    float randomProb = 0.25f; //����¼�����

    // ����һЩ��������ʾ��ͼ�Ĳ���
    [Header("��ͼ����")]
    public  int LAYERS = 5; // ��ͼ�Ĳ���
    public  int LAYER_NODES = 7; // ÿ��Ľڵ���
    const int MAXNODES = 6;
    const int MINNODES = 2;
    const float RANDOMRANGE_ICON = 40;  //Icon
    const float RANDOMRANGE_LINE = 8;   //line



    // ����һЩ��������ʾ�ڵ�ĸ��ʺ�����
    int minBattlePerLayer = 1; // ÿ�������ս���ڵ���
    int maxTreasurePerLayer = 1; // ÿ�����౦��ڵ���
    int maxShopPerLayer = 1; // ÿ�������̵�ڵ���
    int maxRestPerLayer = 1; // ÿ������Ӫ��ڵ���
        

    // ����һ���б��洢���еĽڵ�

    List<List<Node>> nodes = new List<List<Node>>();

    // ����һ�������������

    public Sprite[] icons;
    public Node currentNode = null;
    [Header("Prefab")]
    public GameObject linePrefab;  //��·����
    public GameObject nodePrefab;  //�ڵ�ͼ��
    public Transform NodesParent;  //���ڵ�
    public Transform LineParent;   //��·���߲㼶
    public Sprite Boss;            //����ڵ�
    public GameObject RoadMapPrefab;  
    public GameObject CurrentMap;

   
    /// <summary>
    /// Ϊ �ӽڵ�͸��ڵ㽨�� ����
    /// </summary>
    /// <param name="layer">����</param>
    /// <param name="i">�ڼ����ϲ�ڵ�</param>
    /// <param name="father">���ڵ�</param>
    /// <param name="max">��ѡȡ������</param>
    /// <param name="min">��ѡȡ������</param>
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
        //�ȸ����ϴεĽڵ� �������ֽڵ������   �������� ��һ�ε�preNode������*2 ���ܹ��ɽ��ܵ�����
        if (MaxNode * 2 >= MAXNODES)
        {
            MaxNode = MAXNODES;
        }
        else
        {
            MaxNode *= 2;
        }
        int MinNode = Mathf.Max((MaxNode + 1) / 2, MINNODES);

        //���嵱ǰ����ܵĽڵ���
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
            if (father.type == NodeType.Boss)  //Boss���������չ�
            {
                for (int j = 0; j < MaxNode; j++)
                {
                    //�����ѡ
                    GenerateConnection(layer, UpIndex, father, 7, 0, ref ContainCount);
                }
                break;
            }
            int upperNodeCount = Random.Range(1,4);
            // ����������Ľڵ㣬ֻ������һ���²�ڵ�
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
    
        if(Count==0)  //�Ѿ����������еĽڵ�  //��ôѰ�����һ���ӽڵ� ����
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
        //��������
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
   // ����һ�����������ݸ��ʺ����ƣ�Ϊÿ��Ľڵ��������һ������
    private void AssignNodeType()
    {

        for(int i=0;i<LAYERS-1;i++)
        {
            if(i==0)  //��һ���Ȼ����ͨ����
            {
                foreach(var node in nodes[i])
                {
                    if (node.IsSeleced == false)
                        continue;
                    node.type = NodeType.Battle;
                }
                continue;
            }

            if(i==LAYERS-2)//�����ڶ����Ȼ����Ϣ�ڵ�
            {
                foreach (var node in nodes[i])
                {
                    if (node.IsSeleced == false)
                        continue;
                    node.type = NodeType.Rest;
                }
                continue;
            }

            if(i==(int)(LAYERS/2f+1)) //��Ȼ�Ǳ���ڵ�
            {
                foreach (var node in nodes[i])
                {
                    if (node.IsSeleced == false)
                        continue;
                    node.type = NodeType.Treasure;
                }
                continue;
            }




            // ����һ�����飬�洢ÿ�����͵Ľڵ���
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
            // ����̵�ڵ�������ֵ������滻һ���̵�ڵ�
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
            // ���Ӫ��ڵ�������ֵ������滻һ��Ӫ��ڵ�
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
    //// ����һ��������Ϊÿ���ڵ㴴��һ��UIԪ��
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
                // ��ȡUIԪ���ϵ�Image�����������spriteΪ�ڵ��ͼ��
                Image image = nodeUI.GetComponent<Image>();
                image.sprite = node.icon;
                node.ui = image;
                // ��ȡUIԪ���ϵ�Button��������һ������¼�������SelectNode����
                ///Button button = nodeUI.GetComponent<Button>();
                //button.onClick.AddListener(() => SelectNode(node));
                // ��UIԪ�ظ�ֵ���ڵ��ui����
                // node.ui = nodeUI;
            }
        }

    }
    // ����һ��������Ϊÿ�������Ľڵ㴴��һ��UI�߶�
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
                    // ����һ��UI�߶Σ���Ϊ�����ڵ�֮���ͼ�α�ʾ
                    GameObject lineUI = Instantiate(linePrefab,LineParent);
                    // ��ȡUI�߶��ϵ�LineRenderer����������������յ�Ϊ�����ڵ��λ��
                    MyLine myline = lineUI.GetComponent<MyLine>();
                    GeneratePositions(myline, node.position, lowerNode.position);
                    // ��UI�߶μ���ڵ��lineUIs�б�
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
   

    //δʵ�ֵĹ��ܣ������Ŀ����ʵ��һ�µ�ͼ�Ĺ��� ��ʱû��Ҫʵ�����ѡ��ڵ�Ĺ���
    void SelectNode(Node node)
    {
        // �����ǰ�Ľڵ㲻Ϊ�գ�����UIԪ�ص���ɫ��Ϊ��ɫ
        if (currentNode != null)
        {
            currentNode.ui.color = Color.white;
        }
        // ������Ľڵ㸳ֵ����ǰ�Ľڵ�
        currentNode = node;
        // ����ǰ�Ľڵ��UIԪ�ص���ɫ��Ϊ��ɫ
        currentNode.ui.color = Color.red;
        // ����ÿ���ڵ�
        //for (int i = 0; i < nodes.Count; i++)
        //{
        //     ��ȡ��ǰ�Ľڵ�
        //    Node otherNode = nodes[i];
        //     ����ýڵ㲻�ǵ�ǰ�Ľڵ㣬�Ҳ��ڵ�ǰ�Ľڵ���²�ڵ��б��У�����UIԪ����Ϊ���ɽ���
        //    if (otherNode != currentNode && !currentNode.lowerNodes.Contains(otherNode))
        //    {
        //        otherNode.ui.GetComponent<Button>().interactable = false;
        //    }
        //     ���򣬽���UIԪ����Ϊ�ɽ���
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

    // ����һ����������ʼ��Ϸ
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

        // ���ɽڵ�
        GenerateNodes();
        //Ϊÿ���ڵ��������
        AssignNodeType();
        // Ϊÿ���ڵ��������һ��ͼ��
        AssignNodeIcon();
        // Ϊÿ���ڵ㴴��һ��UIԪ��
        CreateNodeUI();
        // Ϊÿ�������Ľڵ㴴��һ��UI�߶�
        CreateLineUI();
        // ѡ���һ���ڵ㣬��Ϊ��ǰ�Ľڵ�
    }





}



