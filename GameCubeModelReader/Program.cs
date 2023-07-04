//String modelPath = @"C:\tmp\unpacking\gc-probable-models-renamed\probable-models-renamed";
String modelPath = @"D:\GladiusISOWorkingExtracted\python-gc\gc\data\mesh\weapons\axeh\";
String infoFile = @"d:\tmp\unpacking\gc-models\results.txt";
String sectionInfoFile = @"d:\tmp\unpacking\gc-models\sectionInfo.txt";

//modelPath = @"D:\gladius-extracted-archive\gc-compressed\probable-models-renamed";
//sectionInfoFile = @"D:\gladius-extracted-archive\gc-compressed\probable-models-renamed-sectionInfo.txt";
GCModelReader reader = new GCModelReader();
//reader.LoadModels(modelPath,infoFile);
//reader.DumpPoints(infoFile);
reader.DumpSectionLengths(modelPath, sectionInfoFile);
