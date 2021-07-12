using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Microsoft.CodeAnalysis.CSharp;
using NArrange.Core;
using NArrange.Core.CodeElements;
using NArrange.CSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntlrParser
{
    public class TestVisitor
    {
        public void Test1()
        {
            String baseInputPath = @"C:\UnityProjects\GladiusDFGui\Assets\Resources\GODScripts\CodeScripts\";
            String baseOutputPath = @"C:\tmp\gladius-antlr-parser\xexuxjy\AntlrParser\AntlrParser\Generated\";
            baseOutputPath = @"C:\Development\AntlrParser\AntlrParser\Generated\";

            Directory.CreateDirectory(baseOutputPath);
            //String searchString = " * 00097*";
            //String searchString = "*battle_utils*";

            List<String> commonScripts = new List<string>();
            String[] variableSetupFiles = Directory.GetFiles(baseInputPath, "*", SearchOption.AllDirectories);
            foreach (String fileName in variableSetupFiles)
            {
                if (fileName.Contains(".meta"))
                {
                    continue;
                }
                commonScripts.Add(fileName);
            }
            //commonScripts.Add(baseInputPath + "battle_utils");
            //commonScripts.Add("default_battle");
            //commonScripts.Add("leagueUtils");
            //commonScripts.Add("utilgen");
            //commonScripts.Add("battle_destroybarrels_template");
            //commonScripts.Add("battle_points_template");
            //commonScripts.Add("battle_findteam0_template");
            //commonScripts.Add("battle_domination_template");
            //commonScripts.Add("battle_kingofthehill_template");
            //commonScripts.Add("battle_ally_template");
            //commonScripts.Add("Test2");

            SymbolTable symbolTable = new SymbolTable();

            foreach (String fileName in commonScripts)
            {
                //PopulateVariableData(baseInputPath + fileName,symbolTable);
                PopulateVariableData(fileName, symbolTable);
            }

            foreach (String fileName in commonScripts)
            {
                //PopulateVariableData(baseInputPath + fileName,symbolTable);
                PopulateVariableData(fileName, symbolTable);
            }

            symbolTable.globals.GuessTypes();


            String searchString = "**";
            String[] files = Directory.GetFiles(baseInputPath, searchString, SearchOption.AllDirectories);
            List<string> filesToProcess = new List<string>();
            filesToProcess.AddRange(files);
            //filesToProcess.Add(baseInputPath + "battle_utils");

            int counter = 0;

            foreach (String fileName in filesToProcess)
            {
                if (fileName.Contains(".meta"))
                {
                    continue;
                }
                //string fullName = baseInputPath + fileName;
                String fileData = File.ReadAllText(fileName);
                FileInfo fileInfo = new FileInfo(fileName);
                var lexer = new GodscriptLexer(new Antlr4.Runtime.AntlrInputStream(fileData));
                CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
                var parser = new GodscriptParser(commonTokenStream);
                parser.m_currentScope = symbolTable.globals;
                parser.m_rootScope = symbolTable.globals;

                ScriptScope scriptScope = (ScriptScope)parser.m_currentScope.Resolve(fileInfo.Name);
                if (scriptScope == null)
                {
                    scriptScope = new ScriptScope(fileInfo.Name, symbolTable.globals);
                    parser.m_currentScope.Define(scriptScope);
                }
                parser.m_scriptScope = scriptScope;



                parser.m_currentScope = parser.m_scriptScope;

                IParseTree parseTree = parser.programStart();


                GodScriptVariableListener listener = new GodScriptVariableListener();
                listener.m_currentScope = parser.m_currentScope;
                listener.m_rootScope = parser.m_rootScope;
                listener.Visit(parseTree);

                // visit twice to pick up function args?
                listener.Visit(parseTree);

                //listener.FinalizeTypes();
                //listener.DumpVariableDecls();



                using (StreamWriter sw = new StreamWriter(new FileStream("c:/tmp/scopedebug.txt", FileMode.Create)))
                {
                    string shortFileName = new FileInfo(fileName).Name;
                    GodScriptVisitor visitor = new GodScriptVisitor(shortFileName, null);
                    visitor.m_currentScope = parser.m_currentScope;
                    sw.WriteLine(symbolTable.globals.ToString());

                    String results = visitor.Visit(parseTree);
                    using (StreamWriter sw2 = new StreamWriter(baseOutputPath + visitor.m_classname + ".cs"))
                    {

                        String csCode = CSharpSyntaxTree.ParseText(results).GetRoot().ToFullString();

                        CSharpParser csparser = new CSharpParser();

                        //CSharpTestFile testFile = CSharpTestUtilities.GetAssemblyAttributesFile();
                        using (TextReader reader = new StringReader(csCode))
                        {
                            ReadOnlyCollection<ICodeElement> elements = csparser.Parse(reader);
                            if (elements != null)
                            {
                                //ICodeElementWriter codeWriter = _projectManager.GetSourceHandler(inputFile).CodeWriter;
                                //codeWriter.Configuration = _configuration;

                                CodeWriter codeWriter = new CSharpWriter();

                                StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
                                try
                                {
                                    codeWriter.Write(elements, writer);
                                }
                                catch (Exception ex)
                                {
                                }

                                string outputFileText = writer.ToString();

                                String[] lines = outputFileText.Split('\n');

                                foreach (string line in lines)
                                {
                                    //if (line.Trim() != ";")
                                    {
                                        sw2.WriteLine(line);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void PopulateVariableData(String fileName,SymbolTable symbolTable)
        {
            String fileData = File.ReadAllText(fileName);
            FileInfo fileInfo = new FileInfo(fileName);
            var lexer = new GodscriptLexer(new Antlr4.Runtime.AntlrInputStream(fileData));
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            var parser = new GodscriptParser(commonTokenStream);
            parser.m_currentScope = symbolTable.globals;
            parser.m_rootScope = symbolTable.globals;
            parser.m_scriptScope = new ScriptScope(fileInfo.Name, symbolTable.globals);
            parser.m_currentScope.Define(parser.m_scriptScope);
            parser.m_currentScope = parser.m_scriptScope;

            IParseTree parseTree = parser.programStart();


            GodScriptVariableListener listener = new GodScriptVariableListener();
            listener.m_currentScope = parser.m_currentScope;
            listener.m_rootScope = parser.m_rootScope;
            listener.Visit(parseTree);

        }



    }

}
