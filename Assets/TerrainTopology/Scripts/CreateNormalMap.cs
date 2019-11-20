using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainTopology
{

    public class CreateNormalMap : CreateTopology
    {


        protected override void CreateMap()
        {

            Texture2D normalMap = new Texture2D(m_width, m_height);

            for (int y = 0; y < m_height; y++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    Vector2 d1 = GetFirstDerivative(x, y);

                    //Not to sure of the orientation.
                    //Might need to flip x or y

                    var n = new Vector3();
                    n.x = d1.x * 0.5f + 0.5f;
                    n.y = -d1.y * 0.5f + 0.5f; 
                    n.z = 1.0f;

                    n.Normalize();

                    normalMap.SetPixel(x, y, new Color(n.x, n.y, n.z, 1));
                }

            }

            normalMap.Apply();
            m_material.mainTexture = normalMap;
        }

    }

}
