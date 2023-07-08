//String modelPath = @"C:\tmp\unpacking\gc-probable-models-renamed\probable-models-renamed";
string drive = @"F:\";
String modelPath = drive+@"GladiusISOWorkingExtracted\python-gc\gc\data\mesh\weapons\";
//modelPath = drive+@"GladiusISOWorkingExtracted\python-gc\gc\data\levels\";
String outputBaseDir = drive+@"tmp\unpacking\gc-models\";
String infoFile = outputBaseDir+"results.txt";
String sectionInfoFile = outputBaseDir+"sectionInfo.txt";

//modelPath = @"D:\gladius-extracted-archive\gc-compressed\probable-models-renamed";
//sectionInfoFile = @"D:\gladius-extracted-archive\gc-compressed\probable-models-renamed-sectionInfo.txt";
GCModelReader reader = new GCModelReader();
//reader.LoadModels(modelPath,infoFile);
//reader.DumpPoints(infoFile);
//reader.DumpSectionLengths(modelPath, sectionInfoFile);
GCModel axeModel = reader.LoadSingleModel(modelPath+@"axeh\axeh_cultellus.pax",null);

using(BinaryWriter bw = new BinaryWriter(File.OpenWrite(outputBaseDir+"axe.pax")))
{
    axeModel.WriteData(bw);
}

