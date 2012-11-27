using System.Collections.Generic;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using VSTalk.Engine.Core;
using VSTalk.Tools;

namespace Microsoft.VSTalk
{
    public class EnvironmentManager : IEnvironmentManager
    {
        private DTE2 dte;

        public EnvironmentManager()
        {
            dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
        }


        public IEnumerable<string> GetCallStack()
        {
            var stackLines = new List<string>();
            var stackFrames = dte
                .With(_dte => _dte.Debugger)
                .With(debugger => debugger.CurrentThread)
                .With(curTh => curTh.StackFrames);

            if (stackFrames == null) return stackLines;
            foreach (StackFrame frame in stackFrames)
            {
                var functionStringBuilder = new StringBuilder();
                var argsStringBuilder = new StringBuilder();

                if (string.IsNullOrEmpty(frame.ReturnType)) continue;

                functionStringBuilder.AppendFormat("{0} {1}", frame.ReturnType, frame.FunctionName);

                foreach (var argument in frame.Arguments)
                {
                    var expr = argument as EnvDTE.Expression;
                    if (expr == null || !expr.IsValidValue) continue;
                    argsStringBuilder.Append(string.Format("{0} {1} = {2} ", expr.Type, expr.Name, expr.Value));
                }

                stackLines.Add(string.Format("{0}({1})", functionStringBuilder, argsStringBuilder));
            }
            return stackLines;
        }

        public string GetActiveDocument()
        {
            var doc = dte
                .With(_dte => _dte.ActiveDocument)
                .With(activeDoc => activeDoc.Object())
                .With(_doc => _doc as TextDocument);
            if (doc == null) return string.Empty;
            return doc.StartPoint.CreateEditPoint()
                .GetText(doc.EndPoint);
        }

        public string GetDebugOutput()
        {
            var output = dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput)
                .With(win => win.Object as OutputWindow);
            if (output == null) return string.Empty;
            
            const string DEBUG_OUTPUT = "Debug";
            var debugOutput = output.OutputWindowPanes.Item(DEBUG_OUTPUT);
            if (debugOutput == null) return string.Empty;
            TextDocument doc;
            try
            {
                doc = debugOutput.TextDocument;
            }
            catch
            {
                return string.Empty;
            }
            return doc.StartPoint.CreateEditPoint()
                .GetText(doc.EndPoint);
        }
    }
}