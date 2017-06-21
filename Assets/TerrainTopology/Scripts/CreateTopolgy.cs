using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainTopology
{

    public abstract class CreateTopology : MonoBehaviour
    {

        public Material m_material;

        void Start()
        {

            if (m_material == null) return;

            string fileName = Application.dataPath + "/TerrainTopology/Heights.raw";
            float[] heights = Load16Bit(fileName);

            int width = 1024;
            int height = 1024;

            CreateMap(heights, width, height);
        }

        void OnDestroy()
        {
            if (m_material == null) return;
            m_material.mainTexture = null;
        }

        protected abstract void CreateMap(float[] heights, int width, int height);

        protected float[] Load16Bit(string fileName, bool bigendian = false)
        {
            byte[] bytes = System.IO.File.ReadAllBytes(fileName);

            int size = bytes.Length / 2;
            float[] data = new float[size];

            for (int x = 0, i = 0; x < size; x++)
            {
                data[x] = (bigendian) ? (bytes[i++] * 256.0f + bytes[i++]) : (bytes[i++] + bytes[i++] * 256.0f);
                data[x] /= ushort.MaxValue;
            }

            return data;
        }

    }

}
