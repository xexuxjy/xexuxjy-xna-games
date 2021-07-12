using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntlrParser
{
    public class GodScriptVisitor : GodscriptBaseVisitor<string>
    {

        public GodScriptVisitor(string filename,StreamWriter debugStreamWriter)
        {
            m_filename = filename;
            m_debugStreamWriter = debugStreamWriter;
        }

        public override String VisitProgramStart([NotNull] GodscriptParser.ProgramStartContext context)
        {
            m_classname = m_filename.Replace(" ", "");
            m_classname = m_classname.Replace(".txt", "");
            String result = "public class " + m_classname+" : BaseScript";
            result += "{";
            result += CR;

            String classVariables = "";

            ScopedSymbol onInit = m_currentScope.Resolve("OnInit") as ScopedSymbol;
            if(onInit != null)
            {
                foreach(Symbol s in onInit.GetMembers().Values)
                {
                    String typeDecl = "";

                    if (s is VariableSymbol && !s.Declared && s.Type != null)
                    {
                        typeDecl = s.Type.Name + " ";
                        typeDecl += s.Name + " ";

                        typeDecl += ";" + CR;

                        s.Declared = true;
                        classVariables += typeDecl;
                    }
                }
            }

            int ibreak = 0;
            result += classVariables;

            //// static vars..
            //String statics = "";
            //foreach (ScopeObject so in m_listener.m_scopeObectList)
            //{
            //    statics += AddStaticVariables(so);
            //    statics += CR;
            //}

            //result += statics;

            //String instanceVars = "";
            //foreach (ScopeObject so in m_listener.m_scopeObectList)
            //{
            //    instanceVars += AddInstanceVariables(so);
            //    instanceVars += CR;
            //}

            //result += instanceVars;




            String fd = VisitAll(context);
            result += fd;
            //CloseScope(ref result);
            result += "}";
            result += CR;

            return result;
        }


        public void OpenScope(ref string result)
        {
            result += "{";
            result += CR;
        }

        public void CloseScope(ref string result)
        {
            result += "}";
            result += CR;
        }

        public override string VisitFunctionArguments([NotNull] GodscriptParser.FunctionArgumentsContext context)
        {
            String result = "";
            //for (int i = 0; i < context.children.Count; ++i)
            //{
            //    String argName = context.children[i].GetText();
            //    if (argName != ",")
            //    {
            //        if (m_currentScope != null)
            //        {
            //            VariableDecl varDecl = m_currentScope.FindVariableDecl(argName);
            //            if (varDecl != null)
            //            {
            //                result += varDecl.GetTypeString();
            //                if (varDecl.IsArray)
            //                {
            //                    result += "[]";
            //                }
            //            }
            //            result += " " + argName;
            //        }
            //    }
            //    else
            //    {
            //        result += argName;
            //    }
            //}
            //result = VisitAll(context);

            return result;
        }

        public override string VisitFunctionDef([NotNull] GodscriptParser.FunctionDefContext context)
        {
            String functionStartResult = Visit(context.children[0]);
            String functionBlockResult = "";
            String variableDeclareBlock = "";

            if (context.children.Count == 3)
            {
                //foreach(VariableDecl vd in m_currentScope.m_variableDeclList)
                //{
                //    if(vd.VariableScopeType == VariableScopeType.vst_local)
                //    {
                //        variableDeclareBlock += vd.GetTypeString();
                //        variableDeclareBlock += " ";
                //        variableDeclareBlock += vd.VariableName;
                //        variableDeclareBlock += " = ";
                //        variableDeclareBlock += vd.GetTypeDefault();
                //        variableDeclareBlock += ";" + CR;
                //        vd.DeclaredType = true;
                //    }
                //}

                functionBlockResult += Visit(context.children[1]);
            }
            String functionEndResult = Visit(context.children.Last());
            return functionStartResult + "{"+variableDeclareBlock + functionBlockResult+"}" +functionEndResult;
        }


        public override string VisitFunctionStart([NotNull] GodscriptParser.FunctionStartContext context)
        {
            string functionName = context.children[1].GetText();
            m_currentFunction = (FunctionSymbol)m_currentScope.Resolve(functionName);
            m_currentScope = m_currentFunction;
            //m_currentScope = m_listener.m_scopeObectList.Find(x => x.m_currentFunctionName == functionName);

            String result = "public "+m_currentFunction.Type+" "+ functionName;
            OpenBracket(ref result);
            for(int i=0;i<m_currentFunction.m_arguments.Count;++i)
            {
                string type = m_currentFunction.m_arguments[i].Type != null ? m_currentFunction.m_arguments[i].Type.Name : "UNKNOWN";
                result += type;
                result += " ";
                result += m_currentFunction.m_arguments[i].Name;

                if(i< m_currentFunction.m_arguments.Count-1)
                {
                    result += ",";
                }
            }
            String functionArgs = "";
            result += functionArgs;

            CloseBracket(ref result);
            result += CR;


            return result;
        }

        public override string VisitSharedVariableDecl([NotNull] GodscriptParser.SharedVariableDeclContext context)
        {
            bool isInt = context.children[1].GetText() == "int";
            String varName = isInt ? context.children[2].GetText() : context.children[1].GetText();
            int assignmentIndex = -1;
            for (int i = 2; i < context.children.Count; ++i)
            {
                if (context.children[i].GetText() == "=")
                {
                    assignmentIndex = i;
                    break;
                }
            }

            //VariableDecl scopeVariable = m_currentScope.FindVariableDecl(varName);

            //if (scopeVariable != null)
            //{
            //    if (assignmentIndex != -1)
            //    {
            //        scopeVariable.StaticAssignment = VisitAll(context.children[assignmentIndex + 1]);
            //    }
            //}
            // declaration
            int ibreak = 0;

            return "";
        }


        public override string VisitBlock([NotNull] GodscriptParser.BlockContext context)
        {
            String result = "";
            //OpenScope(ref result);
            for (int i = 0; i < context.children.Count; ++i)
            {
                if (ShouldVisitBlockChild(context.children[i]))
                {
                    result += Visit(context.children[i]);
                    result += ";";
                    result += CR;
                }
            }
            //result += VisitAll(context);

            //result += ";";
            //result += CR;
            //result += base.VisitBlock(context);
            //CloseScope(ref result);
            return result;
        }

        public bool ShouldVisitBlockChild(IParseTree child)
        {
            if(child is GodscriptParser.EXPRNAMEContext)
            {
                string symbolString = Visit(child);
                Symbol s = m_currentScope.Resolve(symbolString);
                if(s != null && s.Declared == true)
                {
                    return false;
                }
            }
            return true;
        }


        public override string VisitAssignmentExpression([NotNull] GodscriptParser.AssignmentExpressionContext context)
        {
            String lhs = Visit(context.children[0]);
            Symbol symbol = m_currentScope.Resolve(lhs);
            String typeDecl = "";
            if(symbol != null && !symbol.Declared)
            {
                if(symbol.Type != null)
                    {
                    typeDecl = symbol.Type.Name + " ";
                    symbol.Declared = true;
                }
            }

            //VariableDecl varDecl = m_currentScope.FindVariableDecl(argName);

            String op = context.children[1].GetText();
            String rhs = Visit(context.children[2]);
            //if(varDecl != null)
            //{
            //    varDecl.Assignments.Add(rhs);
            //}

            String result =  typeDecl + lhs + op + rhs + " ;"+CR;
            return result;
        }

        public override string VisitOperatorAddSub([NotNull] GodscriptParser.OperatorAddSubContext context)
        {
            return context.GetText();
        }

        public override string VisitOPAND([NotNull] GodscriptParser.OPANDContext context)
        {
            return base.VisitOPAND(context);
        }

        public override string VisitOperatorComparison([NotNull] GodscriptParser.OperatorComparisonContext context)
        {
            //return base.VisitOperatorComparison(context);
            return context.GetText();
        }


        //public override string VisitEXPRNAME([NotNull] GodscriptParser.EXPRNAMEContext context)
        //{
        //    String text = context.GetText();
        //    //text += " ";
        //    return text;
        //}

        public override string VisitExpressionName([NotNull] GodscriptParser.ExpressionNameContext context)
        {
            //String text = context.GetText();
            //text += " ";
            String text = context.IDENTIFIER().GetText();

            return text;
        }

        public override string VisitOperatorBitwise([NotNull] GodscriptParser.OperatorBitwiseContext context)
        {
            String text = context.GetText();
            text += " ";
            return text;
        }



        public override string VisitExpression([NotNull] GodscriptParser.ExpressionContext context)
        {
            String result = "";
            //if (context.Parent is GodscriptParser.BlockContext && context.ChildCount == 1 && context.GetChild(0) is GodscriptParser.EXPRNAMEContext)
            //{
            //    // don't do anything as this is a declaration we don't want?
            //}
            //else
            //{
            //    result = VisitAll(context);
            //}
            result = VisitAll(context);
            return result;
        }

        //private String VisitAll(ParserRuleContext context)
        private String VisitAll(IParseTree context)
        {
            String result = "";
            int numChildren = context.ChildCount;
            //for (int i = 0; i < context.children.Count; ++i)
            for (int i = 0; i < numChildren; ++i)
            {
                IParseTree child = context.GetChild(i);
                if(child is GodscriptParser.ExpressionNameContext)
                {
                    result += VisitExpressionName((GodscriptParser.ExpressionNameContext)child);
                }
                else if (child is GodscriptParser.EXPRNAMEContext)
                {
                    result += VisitEXPRNAME((GodscriptParser.EXPRNAMEContext)child);
                }
                else if (child is GodscriptParser.OPUNARYContext)
                {
                    result += VisitOPUNARY((GodscriptParser.OPUNARYContext)child);
                }
                else if (child is GodscriptParser.OPANDContext)
                {
                    string lhs = VisitAll(child.GetChild(0)); 
                    string op = child.GetChild(1).GetText();
                    string rhs = VisitAll(child.GetChild(2));

                    result += lhs;
                    result += op;
                    result += rhs;
                    int ibreak = 0;
                    //result += VisitOPUNARY((GodscriptParser.OPUNARYContext)child);
                }
                else if (child is ITerminalNode)
                {
                    result += child.GetText();
                }
                else
                {
                    result += Visit(child);
                }
            }
            return result;
        }


        public override string VisitOperatorUnary([NotNull] GodscriptParser.OperatorUnaryContext context)
        {
            String text = context.GetText();
            text += " ";
            return text;

        }

        public override string VisitArgumentList([NotNull] GodscriptParser.ArgumentListContext context)
        {
            String result = VisitAll(context);
            //for(int i=0;i<context.children.Count;++i)
            //{
            //    VisitAll()
            //    if (context.children[i] is GodscriptParser.EXPRNAMEContext)
            //    {
            //        result += VisitEXPRNAME((GodscriptParser.EXPRNAMEContext)context.children[i]);
            //    }
            //    else
            //    {
            //        result += ",";
            //    }
            //}
            return result;
        }



        public override string VisitForStatement([NotNull] GodscriptParser.ForStatementContext context)
        {
            String result = "for(";
            String forVariable = context.children[1].GetText();
            result += "int ";
            result += forVariable;
            result += "=";
            result += Visit(context.children[3]);
            result += ";";
            result += forVariable;
            result += "<";
            result += Visit(context.children[5]);
            result += ";";
            result += forVariable;
            result += "+=";
            if((context.children.Count > 5) && string.Equals(context.children[6].GetText(),"step",StringComparison.InvariantCultureIgnoreCase))
            {
                result += context.children[7].GetText();
            }
            else
            {
                result += "1";
            }
            result += ")";
            result += CR;
            String temp = FindAndPrintBlocks(context);
            result += "{";
            result += temp;
            result += "}";
            return result;
        }

        public string FindAndPrintBlocks(ParserRuleContext context)
        {
            String result = "";
            foreach(IParseTree childTree in context.children)
            {
                if (childTree is GodscriptParser.BlockContext)
                {
                    result += VisitBlock((GodscriptParser.BlockContext)childTree);
                }
            }

            return result;
        }

        public String VisitArgumentListHelper(String functionName,ParserRuleContext context)
        {
            string result = "";
            foreach (IParseTree childTree in context.children)
            {
                if (childTree is GodscriptParser.ArgumentListContext)
                {
                    result += VisitArgumentList((GodscriptParser.ArgumentListContext)childTree);
                }
            }
            return result;
        }


        public override string VisitLeftHandSide([NotNull] GodscriptParser.LeftHandSideContext context)
        {
            string lhs = Visit(context.children[0]);
            //string cleanedLHS = VariableDecl.GetVariableName(lhs);
            //bool isInstanceVar = false;
            //if (context.children[0] is GodscriptParser.ExpressionNameContext)
            //{
            //    for(int i=0;i< context.children[0].ChildCount;++i)
            //    {
            //        if(String.Equals("global",context.children[0].GetChild(i).GetText(),StringComparison.CurrentCultureIgnoreCase))
            //        {
            //            isInstanceVar = true;
            //            break;
            //        }
            //    }
            //}
            

            // need to do something clever here to differentiate between globals/ function passed and local declared...
            /// look at values passed in function call?
            /// ok - this kind of works - now need to get cleverer and figure out if something is an array or not.
            string result = "";
            bool localDef = false;
            //VariableDecl varDecl = null;
            //varDecl = m_currentScope.FindVariableDecl(cleanedLHS);

            //if (m_currentScope != null)
            //{
            //    varDecl = m_currentScope.FindVariableDecl(cleanedLHS);
            //    if (varDecl != null)
            //    {
            //        localDef = !varDecl.IsArgument;
            //    }
            //}

            
            //if(localDef && !varDecl.DeclaredType && varDecl.VariableScopeType != VariableScopeType.vst_static)
            //{
            //    result = varDecl.GetTypeString();
            //    result += " ";
            //    //result = "var ";
            //    varDecl.DeclaredType = true;
            //}
            //return result + lhs;
            // if we use cleaned, then we lose the array access info...
            //return result + cleanedLHS;
            return result + lhs;
        }


        public override string VisitFunctionCall([NotNull] GodscriptParser.FunctionCallContext context)
        {
            int functionNameIndex = context.children[0].GetText() == "call" ? 1 : 0;
            string staticDecl = "";
            string functionName = context.children[functionNameIndex].GetText();
            if (context.children[functionNameIndex + 1].GetText() == "@")
            {
                staticDecl = context.children[functionNameIndex + 2].GetText();
                staticDecl += ".";
            }

            string result = staticDecl+functionName;
            OpenBracket(ref result);
            result += VisitArgumentListHelper(functionName,context);
            CloseBracket(ref result);

            return result;
        }

        //public override string VisitFUNCCALL([NotNull] GodscriptParser.FUNCCALLContext context)
        //{
        //    string result = Visit(context.children[0]);
        //    return result;
        //}

        public bool LastInParent(ParserRuleContext context)
        {
            return (context.parent != null && context.parent.ChildCount > 0 && context.parent.GetChild(context.parent.ChildCount - 1) == context);
        }

        public override string VisitExpressionReturnStatement([NotNull] GodscriptParser.ExpressionReturnStatementContext context)
        {
            String result = "return ";
            String val = Visit(context.children[2]);
            return result + val + ";" + CR;
        }


        public override string VisitIfStatement([NotNull] GodscriptParser.IfStatementContext context)
        {
            String result = "";
            result += "if";
            OpenBracket(ref result);
            String temp1 = VisitAll(context.children[2]);
            //String temp3 = VisitAll(context.children[2]);
            //String temp1 = context.children[2].GetText();
            result += temp1;
            CloseBracket(ref result);

            String temp2 = "";
            OpenScope(ref temp2);
            temp2+=Visit(context.children[4]);
            CloseScope(ref temp2);
            result += temp2;

            for(int i=5;i<context.children.Count;++i)
            {
                result += Visit(context.children[i]);
            }

            return result;
        }

        public override string VisitOPCOMP([NotNull] GodscriptParser.OPCOMPContext context)
        {
            String result = "";
            String lhs = Visit(context.children[0]);
            String op = Visit(context.children[1]);
            String rhs = Visit(context.children[2]);

            result += lhs;
            result += op;
            result += rhs;
            return result;
            //return base.VisitOPCOMP(context);
        }


        public override string VisitElseIfStatement([NotNull] GodscriptParser.ElseIfStatementContext context)
        {
            String result = "";
            result += "else if";
            OpenBracket(ref result);
            //String temp1 = Visit(context.children[2]);
            String temp1 = context.children[2].GetText();
            result += temp1;
            CloseBracket(ref result);
            String temp2 = "";
            OpenScope(ref temp2);
            temp2 += Visit(context.children[4]);
            CloseScope(ref temp2);
            result += temp2;
            return result;
        }

        public override string VisitElseStatement([NotNull] GodscriptParser.ElseStatementContext context)
        {
            String result = "";
            result += "else ";
            //String temp1 = Visit(context.children[2]);
            //String temp1 = context.children[1].GetText();
            String temp1 = "";
            OpenScope(ref temp1);
            temp1 += Visit(context.children[1]);
            CloseScope(ref temp1);
            result += temp1;
            return result;
        }

        public override string VisitWhileStatement([NotNull] GodscriptParser.WhileStatementContext context)
        {
            //            whileStatement
            //: WHILE '(' expression ')' block ENDWHILE
            //;
            string result = "";
            result += "while(";
            String whileExpr = VisitAll(context.children[2]);
            result += whileExpr;
            result += "){";
            String whileWork = VisitAll(context.children[4]);
            result += whileWork;
            result += ";";
            result += "}";
            //return base.VisitWhileStatement(context);
            return result;
        }


        //public override string VisitInterfaceAccess([NotNull] GodscriptParser.InterfaceAccessContext context)
        //{
        //    //return base.VisitInterfaceAccess(context);
        //    return "GlobalInterfaces.Get" + context.children[0].GetText() + "();" + CR;
        //}

        public override string VisitMethodInvoke([NotNull] GodscriptParser.MethodInvokeContext context)
        {
            String result = "";
            String objectName = context.children[0].GetText();
            String functionName = context.children[2].GetText();


            result += objectName;
            result += ".";
            result += functionName;
            OpenBracket(ref result);

            //String functionArgs = context.children.Count > 4 ? Visit(context.children[4]) : "";
            String functionArgs = context.children.Count > 4 ? VisitAll(context.children[4]) : "";

            result += functionArgs;
            CloseBracket(ref result);


            // if we're not the child of another method invoke then finish line?
            //if (!HasParentOfType(context, typeof(GodscriptParser.MethodInvokeContext)))
            //if(!InFunctionCall())
            //{
            //    result += ";";
            //    result += CR;
            //}
            return result;

        }

        public static bool HasImmediateParentOfType(RuleContext context, Type toCheck)
        {
            return context.Parent.GetType() == toCheck;
        }

        public bool HasParentOfType(RuleContext context,Type toCheck)
        {
            RuleContext parent = context.Parent;
            while(parent != null)
            {
                if(parent.GetType() == toCheck)
                {
                    return true;
                }
                parent = parent.Parent;
            }
            return false;
        }

        public override string VisitSTRING([NotNull] GodscriptParser.STRINGContext context)
        {
            String result = "";
            result = context.children[0].GetText();
            return result;
        }

        public override string VisitINTEGER([NotNull] GodscriptParser.INTEGERContext context)
        {
            String result = "";
            result = context.children[0].GetText();
            return result;
        }

        public override string VisitFLOAT([NotNull] GodscriptParser.FLOATContext context)
        {
            String result = "";
            result = context.children[0].GetText();
            result += "f";
            return result;
        }



        public override string VisitARRACC([NotNull] GodscriptParser.ARRACCContext context)
        {
            return base.VisitARRACC(context);
        }

        public override string VisitIVAR([NotNull] GodscriptParser.IVARContext context)
        {
            String result = context.GetText();
            //return base.VisitIVAR(context);
            return result;
        }

        public override string VisitBooleanType([NotNull] GodscriptParser.BooleanTypeContext context)
        {
            String result = context.GetText().ToLower();
            //return base.VisitIVAR(context);
            return result;
        }

        public override string VisitArrayAccess([NotNull] GodscriptParser.ArrayAccessContext context)
        {
            String result = "";
            String arg1 = Visit(context.children[0]);
            String arg2 = Visit(context.children[2]);

            result = arg1;
            result += "[";
            result += arg2;
            result += "]";

            return result;
        }

        public void OpenBracket(ref string data)
        {
            m_functionCallCounter++;
            data += "(";
        }

        public void CloseBracket(ref string data)
        {
            m_functionCallCounter--;
            data += ")";

        }

        public bool InFunctionCall()
        {
            return m_functionCallCounter > 0;
        }

        public Scope m_currentScope;
        public FunctionSymbol m_currentFunction;

        public int m_functionCallCounter = 0;
        public StreamWriter m_debugStreamWriter;
        private string m_filename;
        public string m_classname;
        public static string CR = "\n";
    }
}
