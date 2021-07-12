using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime;

namespace AntlrParser
{
    public class GodScriptListener : GodscriptBaseListener
    {
        public GodScriptListener(string filename)
        {
            m_filename = filename;
        }

        public override void EnterProgramStart([NotNull] GodscriptParser.ProgramStartContext context)
        {
            base.EnterProgramStart(context);
            m_sb.AppendLine("public class " + m_filename);
            m_sb.AppendLine("{");
        }

        public override void ExitProgramStart([NotNull] GodscriptParser.ProgramStartContext context)
        {
            base.ExitProgramStart(context);
            m_sb.AppendLine("}");
        }


        public override void ExitExpression([NotNull] GodscriptParser.ExpressionContext context)
        {
            base.ExitExpression(context);
            
        }

        public override void EnterFunctionName([NotNull] GodscriptParser.FunctionNameContext context)
        {
            base.EnterFunctionName(context);
            ITerminalNode itn = context.IDENTIFIER();
            String val = itn.GetText();
            m_sb.Append("public object " + val);
        }

        public override void EnterFunctionCall([NotNull] GodscriptParser.FunctionCallContext context)
        {
            base.EnterFunctionCall(context);
            // array version
            m_sb.Append(context.IDENTIFIER()[0].GetText());
            m_sb.Append("(");
        }

        public override void ExitFunctionCall([NotNull] GodscriptParser.FunctionCallContext context)
        {
            base.ExitFunctionCall(context);
            m_sb.Append(")");
        }


        public override void EnterFunctionEnd([NotNull] GodscriptParser.FunctionEndContext context)
        {
            base.EnterFunctionEnd(context);
            m_sb.AppendLine("}");
        }

        public override void EnterFunctionArguments([NotNull] GodscriptParser.FunctionArgumentsContext context)
        {
            
            base.EnterFunctionArguments(context);
            m_sb.Append("(");
            IList<IParseTree> idList = context.children;
            for(int i=0;i<idList.Count;++i)
            {
                m_sb.Append("object ");
                if(idList[i] is GodscriptParser.FunctionArgumentContext)
                {
                    m_sb.Append( ((GodscriptParser.FunctionArgumentContext)idList[i]).IDENTIFIER().GetText());
                }
                else if(idList[i] is ITerminalNode)
                {
                    m_sb.Append(",");
                }
            }
            m_sb.AppendLine(")");
            m_sb.AppendLine("{");
        }

        public override void EnterArgumentList([NotNull] GodscriptParser.ArgumentListContext context)
        {
            base.EnterArgumentList(context);
            //m_sb.Append("(");
        }

        public override void ExitArgumentList([NotNull] GodscriptParser.ArgumentListContext context)
        {
            base.ExitArgumentList(context);
            //m_sb.Append(")");
        }

        public override void EnterAssignmentOperator([NotNull] GodscriptParser.AssignmentOperatorContext context)
        {
            base.EnterAssignmentOperator(context);
            m_sb.Append(" " + context.GetText() + " ");
        }

        //public override void EnterNewVec3([NotNull] GodscriptParser.NewVec3Context context)
        //{
        //    base.EnterNewVec3(context);
        //    //m_sb.Append(" = new Vector3();");
        //    m_ruleContextDictionary[context] = " = new Vector3();";
        //}

        public override void EnterExpressionName([NotNull] GodscriptParser.ExpressionNameContext context)
        {
            if(context.GLOBAL() != null || context.SHARED() != null)
            {
                m_sb.Append("static ");
            }
            String val = context.IDENTIFIER().ToString();
            //m_sb.Append(val);
            //m_sb.Append(" ");

            base.EnterExpressionName(context);
            m_ruleContextDictionary[context] = val;
        }


        public override void ExitAssignmentExpression([NotNull] GodscriptParser.AssignmentExpressionContext context)
        {
            base.ExitAssignmentExpression(context);
            m_sb.AppendLine(";");
        }


        public string GetResults()
        {
            return m_sb.ToString();
        }


        public override void EnterOperatorAddSub([NotNull] GodscriptParser.OperatorAddSubContext context)
        {
            base.EnterOperatorAddSub(context);
            m_sb.Append(" " + context.GetText() + " ");
        }

        public override void EnterForStatement([NotNull] GodscriptParser.ForStatementContext context)
        {
            base.EnterForStatement(context);
            
            
        }

        public override void ExitForStatement([NotNull] GodscriptParser.ForStatementContext context)
        {
            base.ExitForStatement(context);
            String forVar = context.IDENTIFIER()[0].GetText();
            m_sb.Append("for(int " +forVar  + " = ");
            m_sb.Append(ParseTreeResult(context.children[3], context));
            m_sb.Append(" ; "+forVar+" < ");
            m_sb.Append(ParseTreeResult(context.children[5], context));
            m_sb.Append(";");
            String increment = "1";
            if(context.children.Count > 6 && context.children[6].ToString() == "step")
            {
                increment = ParseTreeResult(context.children[7],context);
            }
            m_sb.Append(forVar + "+=" + increment);
            m_sb.AppendLine(")");

            m_sb.AppendLine("{");


            m_sb.AppendLine("}");
        }

        public String ParseTreeResult(IParseTree ipt,IParseTree walkLimit)
        {
            if(ipt == null)
            {
                return null;
            }

            String result = "";
            m_ruleContextDictionary.TryGetValue(ipt, out result);
            if(result == null && ipt.Parent != walkLimit)
            {
                result = ParseTreeResult(ipt.Parent,walkLimit);
            }
            return result;
        }

        public override void EnterReturnStatement([NotNull] GodscriptParser.ReturnStatementContext context)
        {
            base.EnterReturnStatement(context);
            m_sb.Append("return ");
        }

        public override void ExitReturnStatement([NotNull] GodscriptParser.ReturnStatementContext context)
        {
            base.ExitReturnStatement(context);
            m_sb.AppendLine(";");
        }

        public override void EnterOperatorUnary([NotNull] GodscriptParser.OperatorUnaryContext context)
        {
            base.EnterOperatorUnary(context);
            m_sb.Append(context.GetText());
        }

        StringBuilder m_sb = new StringBuilder();
        string m_filename;
        Dictionary<IParseTree, String> m_ruleContextDictionary = new Dictionary<IParseTree, string>();
    }




    

}
