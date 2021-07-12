using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;

namespace AntlrParser
{
    public class GodScriptFunctionCallVisitor : GodscriptBaseVisitor
    {

        public override void EnterFunctionCall([NotNull] GodscriptParser.FunctionCallContext context)
        {
            base.EnterFunctionCall(context);

            m_currentCallInfo = new FunctionCallInfo();
            m_functionCalls.Add(m_currentCallInfo);

        }

        public override void ExitFunctionCall([NotNull] GodscriptParser.FunctionCallContext context)
        {
            base.ExitFunctionCall(context);
        }

        public override void EnterArgumentList([NotNull] GodscriptParser.ArgumentListContext context)
        {
            base.EnterArgumentList(context);
        }

        private FunctionCallInfo m_currentCallInfo;
        public List<FunctionCallInfo> m_functionCalls = new List<FunctionCallInfo>();
    }

    public class FunctionCallInfo
    {
        public string ownerFunction;
        public string functionName;
        public List<VariableDecl> variableList = new List<VariableDecl>();
    }


}
