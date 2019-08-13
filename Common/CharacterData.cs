using UnityEngine;
using System.Collections.Generic;

public class CharacterData : MonoBehaviour
{
    public int hCount = 4;
    public int vCount = 4;
    public Sprite[] sprites;

    private Dictionary<int, Sprite[]> m_Dict;
    private void InitSprites()
    {
        m_Dict = new Dictionary<int, Sprite[]>();
        for (int i = 0; i < vCount; i++)
        {
            Sprite[] list = new Sprite[hCount];
            for (int j = 0; j < hCount; j++)
            {
                int index = i * hCount + j;
                list[j] = sprites[index];
            }

            m_Dict.Add(i, list);
        }
    }
    public Sprite[] GetSprites(int dir)
    {
        if (m_Dict == null) InitSprites();
        return m_Dict[dir];
    }
}
