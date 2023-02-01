using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width = 7;
    public int height = 7;

    [SerializeField]
    GameObject bgTile;

    [SerializeField]
    // lưu 5 gem
    Gem[] gems;

    public int gemSpeed = 8;

    public Gem[,] allGems;

    
    public FindMatcher matcher;

    private void Awake()
    {
        matcher = FindObjectOfType<FindMatcher>();
    }

    // Start is called before the first frame update
    void Start()
    {
        allGems = new Gem[width, height];
        Setup();   
    }

    // Update is called once per frame
    void Update()
    {
        matcher.FindAllMatches();
    }

    void Setup()
    {
        for (int x = 0; x < width; x++) 
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = new Vector2(x, y);
                GameObject tile = Instantiate(bgTile, pos, Quaternion.identity);
                tile.transform.parent = transform;
                tile.name = $"BG - {x}, {y}";

                // Random.Range(min, max) = sinh giá trị ngẫu nhiên trong đoạn [min, max-1]
                int gemToUse = Random.Range(0, gems.Length);
                Vector2Int gemPos = new Vector2Int(x, y);



                int loopCheck = 0;
                while (MatchAt(gemPos, gems[gemToUse]) && loopCheck < 100)
                {
                    gemToUse = Random.Range(0, gems.Length);
                    loopCheck++;
                }

                GenerateGems(gemPos, gems[gemToUse]);
            }
        }
    }

    void GenerateGems(Vector2Int pos, Gem gem)
    {
        Gem currentGem = Instantiate(gem, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
        currentGem.transform.parent = transform;
        currentGem.name = $"Gem - {pos.x}, {pos.y}";
        currentGem.InitGem(pos, this);
        // lưu currentGem vào mảng allGems
        allGems[pos.x, pos.y] = currentGem;
    }

    bool MatchAt(Vector2Int pos, Gem gem)
    {
        bool result = false;
        if ( pos.x > 1 )
        {
            if (allGems[pos.x - 1, pos.y].type == gem.type
                && allGems[pos.x - 2, pos.y].type == gem.type)
            {
                result = true;
            }
        }
        if (pos.y > 1)
        {
            if (allGems[pos.x, pos.y - 1].type == gem.type
                && allGems[pos.x, pos.y - 2].type == gem.type)
            {
                result = true;
            }
        }
        return result;
    }

    void DeleteMatchAt(Vector2Int pos)
    {
        if (allGems[pos.x, pos.y] != null && allGems[pos.x, pos.y].isMatched)
        {
            // xóa gem mà script đang gắn vào
            Destroy(allGems[pos.x, pos.y].gameObject);
            allGems[pos.x, pos.y] = null;
        }
    }

    public void DeleteAllMatches()
    {
        for (int i = 0; i < matcher.matches.Count; i++)
        {
            DeleteMatchAt(matcher.matches[i].pos);
        }
    }


}
