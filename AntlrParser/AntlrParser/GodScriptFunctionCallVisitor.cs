//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Antlr4.Runtime.Misc;
//using Antlr4.Runtime.Tree;

//namespace AntlrParser
//{
//    public class GodScriptFunctionCallVisitor : GodscriptBaseVisitor<string>
//    {

//        public override string VisitFunctionCall([NotNull] GodscriptParser.FunctionCallContext context)
//        {
//            string functionName = context.children[0].GetText() == "call" ? context.children[1].GetText() : context.children[0].GetText();




//            for (int i = 0; i < context.children.Count; ++i)
//            {
//                if (context.children[i] is GodscriptParser.ArgumentListContext)
//                {
//                    GodscriptParser.ArgumentListContext args = context.children[i] as GodscriptParser.ArgumentListContext;
//                    foreach(IParseTree child in args.children)
//                    {
//                        if(child is GodscriptParser.ExpressionNameContext || child is GodscriptParser.EXPRNAMEContext)
//                        {
//                            m_currentCallInfo.variableList.Add(new VarNameType(child.GetText()));
//                        }
//                    }
//                }
//            }


//            return base.VisitFunctionCall(context);
//        }

//        //public override void EnterFunctionCall([NotNull] GodscriptParser.FunctionCallContext context)
//        //{
//        //    base.EnterFunctionCall(context);

//        //    m_currentCallInfo = new FunctionCallInfo();
//        //    m_functionCalls.Add(m_currentCallInfo);

//        //}

//        //public override void ExitFunctionCall([NotNull] GodscriptParser.FunctionCallContext context)
//        //{
//        //    base.ExitFunctionCall(context);
//        //}

//        //public override void EnterArgumentList([NotNull] GodscriptParser.ArgumentListContext context)
//        //{
//        //    base.EnterArgumentList(context);
//        //}

//        //public void BuildData(GodScriptVariableListener listener)
//        //{
//        //    foreach(FunctionCallInfo fci in m_functionCalls)
//        //    {
//        //        foreach(VarNameType vnt in fci.variableList)
//        //        {
//        //            foreach(var so in listener.m_scopeObectList)
//        //            {
//        //                //if(so.m_currentFunctionName == fci.functionName)
//        //                {
//        //                    foreach(VariableDecl vd in so.m_variableDeclList)
//        //                    {
//        //                        if((so.m_currentFunctionName == fci.functionName || vd.VariableScopeType == VariableScopeType.vst_static ) && vd.VariableName == vnt.Name)
//        //                        {
//        //                            vnt.VarDecl = vd;
//        //                        }
//        //                    }
//        //                }
//        //            }

//        //        }
//        //    }

//        //}

//    }
//}
