//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace ModelNamer
//{
//    public class XboxModelReader
//    {
//    }

//    public class XboxModel
//    {

//        static char[] versTag = new char[] { 'V', 'E', 'R', 'S' };
//        static char[] cprtTag = new char[] { 'C', 'P', 'R', 'T' };
//        static char[] selsTag = new char[] { 'S', 'E', 'L', 'S' };
//        static char[] cntrTag = new char[] { 'C', 'N', 'T', 'R' };
//        static char[] shdrTag = new char[] { 'S', 'H', 'D', 'R' };
//        static char[] txtrTag = new char[] { 'T', 'X', 'T', 'R' };
//        static char[] stypTag = new char[] { 'S', 'T', 'Y', 'P' };
//        static char[] nameTag = new char[] { 'N', 'A', 'M', 'E' };
//        static char[] skelTag = new char[] { 'S', 'K', 'E', 'L' };

//        //static char[] paddTag = new char[] { 'P', 'A', 'D', 'D' };
//        static char[] xrndTag = new char[] { 'X', 'R', 'N', 'D' };
//        static char[] doegTag = new char[] { 'd', 'o', 'e', 'g' };




//        static char[][] allTags = { versTag, cprtTag, selsTag, cntrTag, shdrTag, txtrTag,stypTag,nameTag,skelTag,xrndTag,doegTag};

//        public XboxModel(String name)
//        {
//            m_name = name;
//        }

//        public String m_name;
//        public sVector3 m_center;
//        public List<sVector3> m_points = new List<sVector3>();
//        public List<sVector3> m_normals = new List<sVector3>();
//        public List<sVector2> m_uvs = new List<sVector2>();


//        // doeg tag always 7C (124) , encloes start and end doeg values , 2 per file , has FFFF following

//        public Dictionary<char[], int> m_tagSizes = new Dictionary<char[], int>();

//    }

//    public class DoegSection
//    {

//    }


//}

