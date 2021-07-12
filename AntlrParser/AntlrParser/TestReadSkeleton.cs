using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntlrParser
{
    public class TestReadSkeleton
    {

        public void Test1()
        {
            String basePath = @"C:\UnityProjects\GladiusDFGui\Assets\XBoxModels\arenas\";
            String outputPath = @"c:\tmp\gladius-model-output\";

            String modelName = "imperiaField.mdl.bytes";

            XboxModelReader modelReader = new XboxModelReader();
            //modelReader.LoadModels(basePath, null,"*imperiaField*");
            modelReader.LoadModels(basePath, null, "*tablet*");
            foreach (XboxModel model in modelReader.m_models)
            {
                using (StreamWriter sw = new StreamWriter(outputPath + model.m_name + ".txt"))
                {
                    model.DebugOBBT(sw);
                    //model.ob
                    //foreach (BoneNode bn in model.BoneList)
                    //{
                    //    if (Math.Abs(bn.rotation.W) != 1)
                    //    {
                    //        sw.WriteLine(bn.ToString());
                    //    }
                    //}

                }
            }
        }

    }
}
