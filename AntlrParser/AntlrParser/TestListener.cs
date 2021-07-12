using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntlrParser
{
    public class TestListener
    {
        public void Test1()
        {
            String basePath = @"C:\UnityProjects\GladiusDFGui\Assets\Resources\GODScripts\";
            String fileName = "File 000431";//"Sample.txt";
            String fileData = File.ReadAllText(basePath + @"CodeScripts\"+fileName);

            var lexer = new GodscriptLexer(new Antlr4.Runtime.AntlrInputStream(fileData));
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            var parser = new GodscriptParser(commonTokenStream);
            IParseTree parseTree = parser.programStart();
            GodScriptListener listener = new GodScriptListener(fileName);
            ParseTreeWalker.Default.Walk(listener, parseTree);
            String results = listener.GetResults();
            File.WriteAllText(basePath + @"CodeScriptsOutput\" + fileName, results);
        }
    }

}
