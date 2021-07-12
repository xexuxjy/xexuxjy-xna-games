using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace AntlrParser
{
    public class GodScriptVariableListener : GodscriptBaseVisitor<string>
    {

        public override string VisitFunctionName([NotNull] GodscriptParser.FunctionNameContext context)
        {
            String result = base.VisitFunctionName(context);
            ITerminalNode itn = context.IDENTIFIER();
            if (itn != null)
            {
                String functionName = itn.GetText();
                FunctionSymbol functionSymbol = m_currentScope.Resolve(functionName) as FunctionSymbol;
                if (functionSymbol == null)
                {
                    functionSymbol = new FunctionSymbol(itn.GetText(), null, m_currentScope);
                    m_currentScope.Define(functionSymbol);
                }
                
                m_currentScope = functionSymbol;
                m_currentFunction = functionSymbol;

                if ("OnInit".Equals(functionName))
                {
                    m_currentFunction.IsInit = true;
                }
                else
                {
                }
            }
            return result;
        }

        //public override string VisitReturnStatement([NotNull] GodscriptParser.ReturnStatementContext context)
        //{
        //    if(context.expression() != null)
        //    {
        //        Symbol s = m_currentScope.Resolve(context.expression().GetText());
        //        if (s != null)
        //        {
        //            context.type = s.Type;
        //        }
        //    }

        //    m_currentFunction.Type = context.type;
        //    return base.VisitReturnStatement(context);
        //}

        public override string VisitExpressionReturnStatement([NotNull] GodscriptParser.ExpressionReturnStatementContext context)
        {
            if(m_currentFunction.Name == "GetNumClassOnTeam")
            {
                int ibreak = 0;
            }

            if (context.expression().type != null)
            {
                m_currentFunction.Type = context.expression().type;
            }
            else
            {
                string argName = context.children[2].GetText();
                Symbol s = m_currentScope.Resolve(argName);
                if (s != null && s.Type != null)
                {
                    m_currentFunction.Type = s.Type;
                }
                int ibreak = 0;
            }
            return base.VisitExpressionReturnStatement(context);
        }

        public override string VisitFunctionArgument([NotNull] GodscriptParser.FunctionArgumentContext context)
        {
            VariableSymbol vs = new VariableSymbol(context.IDENTIFIER().GetText(), null);
            m_currentFunction.AddArgument(vs);
            return base.VisitFunctionArgument(context);

        }


        public override string VisitFunctionEnd([NotNull] GodscriptParser.FunctionEndContext context)
        {
            if (m_currentFunction != null && m_currentFunction.Type == null)
            {
                m_currentFunction.Type = SymbolTable._VOID;
            }
            m_currentScope = m_currentScope.GetParentScope();
            m_currentFunction = null;

            return base.VisitFunctionEnd(context);
        }

        public override string VisitLeftHandSide([NotNull] GodscriptParser.LeftHandSideContext context)
        {
                String name = null;
                Type type = context.type;
            if (context.children.Count > 0)
            {
                IParseTree child = context.children[0];
                VariableSymbol vs = null;
                if (child is GodscriptParser.ExpressionNameContext)
                {
                    vs = PopulateVariableInfo(((GodscriptParser.ExpressionNameContext)child));
                }
                else if (context.children[0] is GodscriptParser.ArrayAccessContext)
                {
                    int ibreak = 0;
                    child = child.GetChild(0);
                    if (child is GodscriptParser.ExpressionNameContext)
                    {
                        vs = PopulateVariableInfo(((GodscriptParser.ExpressionNameContext)child));
                    }
                }
                else if (context.children[0] is GodscriptParser.InstanceVariableContext)
                {
                    int ibreak = 0;
                }
                if (vs != null)
                {
                    name = vs.Name;
                    Symbol existingSymbol = m_currentScope.Resolve(name);
                    if (existingSymbol == null)
                    {
                        m_currentScope.Define(vs);
                    }
                    else
                    {
                        int ibreak = 0;
                    }
                }
            }

            return name;
            //return base.VisitLeftHandSide(context);
        }

        public static VariableSymbol PopulateVariableInfo(GodscriptParser.ExpressionNameContext exprNameContext)
        {
            VariableSymbol vs = null;
            String name = exprNameContext.IDENTIFIER().GetText();
            if (name != null)
            {
                Type type = exprNameContext.type;
                vs = new VariableSymbol(name, type);

                vs.IsGlobal = (exprNameContext.GLOBAL() != null);
                vs.IsShared = (exprNameContext.SHARED() != null);
            }
            return vs;
        }


        public override string VisitAssignmentExpression([NotNull] GodscriptParser.AssignmentExpressionContext context)
        {
            String lhs = Visit(context.children[0]);
            Symbol symbol = m_currentScope.Resolve(lhs);
            

            //VariableDecl varDecl = m_currentScope.FindVariableDecl(argName);

            String op = context.children[1].GetText();
            String rhs = Visit(context.children[2]);
            //if(varDecl != null)
            //{
            //    varDecl.Assignments.Add(rhs);
            //}

            


            String result = base.VisitAssignmentExpression(context);
            GodscriptParser.ExpressionContext exprContext = (GodscriptParser.ExpressionContext)context.children[2];

            Type t = exprContext.type;
            if (symbol != null)
            {
                symbol.Type = t;
            }
            return result;
        }

        public override string VisitMETHINV([NotNull] GodscriptParser.METHINVContext context)
        {
            String result = base.VisitMETHINV(context);
            context.type = context._methodInvoke.type;
            return result;
        }

        public override string VisitFUNCCALL([NotNull] GodscriptParser.FUNCCALLContext context)
        {
            String result = base.VisitFUNCCALL(context);
            context.type = context._functionCall.type;
            return result;
        }

        public override string VisitMethodInvoke([NotNull] GodscriptParser.MethodInvokeContext context)
        {
            string result = base.VisitMethodInvoke(context);
            String objectName = context.a.Text;
            String methodName = context.b.Text;
            if(methodName == "SummonEntity")
            {
                int ibreak = 0;
            }
            Symbol s = m_currentScope.Resolve(objectName);
            if (s != null)
            {
                String typeName = s.Type != null ? s.Type.Name : null;
                if (typeName != null)
                {
                    ScopedSymbol typeSymbol = m_currentScope.Resolve(typeName) as ScopedSymbol;
                    if (typeSymbol != null)
                    {
                        FunctionSymbol fs = typeSymbol.Resolve(methodName) as FunctionSymbol;
                        context.type = fs.Type;
                    }
                }
            }
            return result;
        }

        public override string VisitFunctionCall([NotNull] GodscriptParser.FunctionCallContext context)
        {
            String result = base.VisitFunctionCall(context);

            String methodName = "";
            if (context.a != null)
            {
                methodName = context.a.Text;

                Symbol s = m_currentScope.Resolve(methodName);
                if (s is FunctionSymbol)
                {
                    FunctionSymbol fs = m_currentScope.Resolve(methodName) as FunctionSymbol;
                    context.type = fs.Type;
                }
                VisitArgumentListHelper(methodName, null,context);
            }
            else if(context.b != null && context.b != null)
            {
                methodName = context.b.Text;

                string scriptName = context.c.Text;
                if(scriptName != null)
                {
                    scriptName = scriptName.ToLower();
                }
                ScriptScope ss = m_rootScope.Resolve(scriptName) as ScriptScope;
                if (ss != null)
                {
                    Symbol s = ss.Resolve(methodName);
                    if (s is FunctionSymbol)
                    {
                        FunctionSymbol fs = s as FunctionSymbol;
                        context.type = fs.Type;
                    }
                }
                VisitArgumentListHelper(methodName,scriptName, context);
            }

            return result;
        }

        public override string VisitEXPRNAME([NotNull] GodscriptParser.EXPRNAMEContext context)
        {
            string result = base.VisitEXPRNAME(context);
            if(context.Parent is GodscriptParser.BlockContext)
            {
                IParseTree child = context.children[0];
                VariableSymbol vs = null;
                if (child is GodscriptParser.ExpressionNameContext)
                {
                    vs = PopulateVariableInfo(((GodscriptParser.ExpressionNameContext)child));
                }
                if(vs != null)
                {
                    m_currentScope.Define(vs);
                }
            }

            return result;
        }

        //public override string VisitArgumentList([NotNull] GodscriptParser.ArgumentListContext context)
        //{
        //    string result = base.VisitArgumentList(context);


        //    return result;
        //}


        public String VisitArgumentListHelper(String functionName, string scriptName,ParserRuleContext context)
        {
            string result = "";
            Scope scope = m_currentScope;
            if(scriptName != null)
            {
                scope = m_rootScope.Resolve(scriptName) as ScopedSymbol;
            }
            if (scope != null)
            {
                Symbol s = scope.Resolve(functionName);
                if (s != null)
                {
                    FunctionSymbol functionSymbol = null;
                    if (s != null)
                    {
                        functionSymbol = s as FunctionSymbol;
                    }

                    foreach (IParseTree childTree in context.children)
                    {
                        if (childTree is GodscriptParser.ArgumentListContext)
                        {
                            GodscriptParser.ArgumentListContext argumentListContext = (GodscriptParser.ArgumentListContext)childTree;
                            int argCount = 0;
                            for (int i = 0; i < argumentListContext.children.Count; ++i)
                            {
                                Type argType = null;
                                if (argumentListContext.children[i] is GodscriptParser.STRINGContext)
                                {
                                    argType = SymbolTable._STRING;
                                }
                                else if (argumentListContext.children[i] is GodscriptParser.INTEGERContext)
                                {
                                    argType = SymbolTable._INTEGER;
                                }
                                else if (argumentListContext.children[i] is GodscriptParser.FLOATContext)
                                {
                                    argType = SymbolTable._FLOAT;
                                }
                                else if (argumentListContext.children[i] is GodscriptParser.BOOLEANContext)
                                {
                                    argType = SymbolTable._BOOL;
                                }
                                else if (argumentListContext.children[i] is GodscriptParser.EXPRNAMEContext)
                                {
                                    GodscriptParser.EXPRNAMEContext exprNameContext = argumentListContext.children[i] as GodscriptParser.EXPRNAMEContext;
                                    argType = exprNameContext.type;
                                }

                                else if (argumentListContext.children[i] is GodscriptParser.FUNCCALLContext)
                                {
                                    //lookup function return type...
                                    argType = SymbolTable._STRING;
                                }
                                else if (argumentListContext.children[i] is GodscriptParser.METHINVContext)
                                {
                                    //lookup function return type...
                                    argType = SymbolTable._STRING;
                                }


                                if (argType == SymbolTable._ACTOR)
                                {
                                    int ibreak = 0;
                                }

                                if (argType != null)
                                {
                                    VariableSymbol vs = functionSymbol.GetVariableAtIndex(argCount);
                                    if (vs != null)
                                    {
                                        vs.Type = argType;
                                    }
                                    argCount++;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }



        public FunctionSymbol m_currentFunction;
        //public SymbolTable m_symbolTable;
        public Scope m_currentScope;
        public Scope m_rootScope;
    }


}
