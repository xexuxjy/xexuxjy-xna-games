    using UnityEngine;

    public class TestCreateSkinDataStub : MonoBehaviour
    {
        public TextAsset OriginalModel;        
        public string DumpSearchDirectory;
        public string DumpFileName;
        public uint LodLevel;
        public int AnimShift;
        public int RamSize;
        [Range(1,4)]
        public int MaxWeights;

        public int MaxVertices;

    }
