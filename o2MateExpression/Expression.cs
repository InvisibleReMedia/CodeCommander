using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Converters;
using System.Drawing;

namespace o2Mate
{
    /// <summary>
    /// Interface declaration of expression translator
    /// </summary>
    [CoClass(typeof(IExpression))]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IExpression
    {
        /// <summary>
        /// Evaluates a string expression and compute it
        /// </summary>
        /// <param name="expr">a string arithmetic or boolean expression representation</param>
        /// <param name="scope">the current scope where to find others variable</param>
        /// <returns>a new data object (which doesn't reside in scope)</returns>
        [DispId(1)]
        IDataNotInScope Evaluate(string expr, Object scope);
    }

    internal class Compute : ILanguageConverter, IDisposable
    {
        #region Constants
        private const char StackIdentifier = '$';
        private readonly string[] errors = new string[] { null, "Erreur parenthèses", "Expression {0:G} incorrecte", "Terme inconnu" };
        private readonly char[] opers = new char[] { ',', '<', '>', '=', '+', '-', '/', '*', '.', '?', ':' };
        #endregion

        #region Private Fields
        int index;
        IScope scope;
        ICodeConverter converter;
        List<uint> stock = new List<uint>();
        List<string> words = new List<string>();
        Stack<string> stack = new Stack<string>();
        List<Variable> vars = new List<Variable>();
        EnumDataType outputType;
        bool withVariables = false;
        bool isByRef = false;
        bool cannotRef = false;
        Form canvas;
        #endregion

        #region Private Methods

        private void Enqueue(string s)
        {
            this.stack.Push(s);
        }

        private string Dequeue()
        {
            return this.stack.Pop();
        }

        private bool function(string expr, ref int i)
        {
            bool result = false;
            if (expr.EndsWith(Compute.StackIdentifier.ToString()))
            {
                this.words.Add(expr.Substring(0, expr.Length - 1));
                this.stock.Add('µ');
                this.stock.Add((uint)(this.words.Count - 1));
                result = true;
            }
            i = expr.Length;
            return result;
        }

        private bool variable(string expr, ref int i)
        {
            string trimedExpr = expr.Trim();
            if (String.IsNullOrEmpty(trimedExpr))
                trimedExpr = " ";
            IData data = this.scope.GetVariable(trimedExpr);
            if (data != null)
            {
                i = trimedExpr.Length;
                int c = 0;
                try
                {
                    this.words.Add(trimedExpr);
                    this.stock.Add('ù');
                    this.stock.Add((uint)(this.words.Count - 1));
                }
                catch
                {
                    // ce n'est pas une constante
                    noQuoteString(data.ValueString, ref c);
                }
                return true;
            }
            else
            {
                // 1- search this char @ for variable
                // 2- return the string
                int c = 0;
                noQuoteString(trimedExpr, ref c);
                return true;
            }
        }

        private void noQuoteString(string expr, ref int i)
        {
            i = expr.Length;
            this.words.Add(expr);
            if (expr.StartsWith(Compute.StackIdentifier.ToString()))
            {
                this.stock.Add('a');
            }
            else
            {
                this.stock.Add('!');
            }
            this.stock.Add((uint)(this.words.Count - 1));
        }

        private void constante(string expr, ref int i)
        {
            int right;
            right = 0;
            string trimedExpr = expr.TrimStart();
            while (right < trimedExpr.Length && trimedExpr[right] >= '0' && trimedExpr[right] <= '9') ++right;
            i = right;
            string subexpr = trimedExpr.Substring(0, right);
            this.stock.Add('@');
            this.stock.Add(System.Convert.ToUInt32(subexpr));
        }

        private void monome(string expr)
        {
            int i, l;
            string trimedExpr = expr.TrimStart();
            l = trimedExpr.Length;
            if (l == 0) throw new Exception(String.Format(this.errors[2], expr));
            switch (trimedExpr[0])
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    i = 0;
                    this.constante(trimedExpr, ref i);
                    if (i == l) return;
                    break;
                case Compute.StackIdentifier:
                    {
                        i = 0;
                        trimedExpr = trimedExpr.TrimEnd();
                        l = trimedExpr.Length;
                        if (l == 1)
                        {
                            string subexpr = this.Dequeue();
                            this.parentheses(subexpr);
                        }
                        else
                        {
                            // c'est un paramètre formel dans l'expression (commence par $)
                            this.noQuoteString(trimedExpr, ref i);
                        }
                        break;
                    }
                default:
                    i = 0;
                    if (this.function(trimedExpr, ref i))
                    {
                        string subexpr = this.Dequeue();
                        if (!String.IsNullOrEmpty(subexpr))
                            this.parentheses(subexpr);
                    }
                    else
                    {
                        i = 0;
                        if (this.variable(trimedExpr, ref i))
                        {
                            if (i == l) return;
                        }
                    }
                    break;
            }
        }

        private void operateurs(string expr, int oper)
        {
            int i, j;
            string subexpr;

            j = i = expr.Length;
            if (i == 0) throw new Exception(String.Format(this.errors[2], expr));
            if (expr[--i] != this.opers[oper])
            {
                while (--i >= 0)
                {
                    if (expr[i] == this.opers[oper]) break;
                }
            }
            if (i == -1)
            {
                if (oper + 1 < this.opers.Length)
                    this.operateurs(expr, oper + 1);
                else
                    this.monome(expr);
                return;
            }
            if (i == 0)
            {
                if (this.opers[oper] != '+' && this.opers[oper] != '-')
                    throw new Exception(String.Format(this.errors[2], expr));
                this.stock.Add(this.opers[oper]);
                monome("0");
                subexpr = expr.Substring(i + 1, j - i - 1);
                operateurs(subexpr, oper);
                return;
            }

            this.stock.Add(this.opers[oper]);
            subexpr = expr.Substring(0, i);
            operateurs(subexpr, oper);
            subexpr = expr.Substring(i + 1, j - i - 1);
            operateurs(subexpr, oper);

        }

        private void parentheses(string expr)
        {
            int i;
            int leftpar = -1, rightpar = -1;
            int profpar = 0;
            string subexpr;

            i = expr.Length;
            while (--i >= 0)
            {
                if (expr[i] == ')')
                    if (rightpar == -1) rightpar = i; else ++profpar;
                else if (expr[i] == '(')
                    if (profpar == 0) leftpar = i; else --profpar;

            }
            if (leftpar < 0 && rightpar < 0)
            {
                // il n'y a pas de parentheses
                this.operateurs(expr, 0);
                return;
            }
            if (leftpar < 0 || rightpar < leftpar)
            {
                throw new Exception(this.errors[1]);
            }

            subexpr = expr.Substring(leftpar + 1, rightpar - leftpar - 1);
            this.Enqueue(subexpr);
            string subexpr2 = expr.Substring(0, leftpar) + Compute.StackIdentifier.ToString() + expr.Substring(rightpar + 1);
            this.Enqueue(subexpr2);
            parentheses(this.Dequeue());
        }

        private Variable calculate()
        {
            Variable a, b;
            Variable res = new Variable("", true);
            switch (this.stock[this.index])
            {
                case '>':
                    ++this.index;
                    a = this.calculate();
                    b = this.calculate();
                    res = new Variable("", a.IsComputable && b.IsComputable, EnumDataType.E_BOOL);
                    res.Set(res.Prefix, "", ((a.ValueInt > b.ValueInt) ? 1 : 0).ToString(), true, EnumDataType.E_BOOL);
                    res.UseThis(EnumDataType.E_BOOL);
                    break;
                case '<':
                    ++this.index;
                    a = this.calculate();
                    b = this.calculate();
                    res = new Variable("", a.IsComputable && b.IsComputable, EnumDataType.E_BOOL);
                    res.Set(res.Prefix, "", ((a.ValueInt < b.ValueInt) ? 1 : 0).ToString(), true, EnumDataType.E_BOOL);
                    res.UseThis(EnumDataType.E_BOOL);
                    break;
                case '=':
                    ++this.index;
                    a = this.calculate();
                    b = this.calculate();
                    res = new Variable("", a.IsComputable && b.IsComputable, EnumDataType.E_BOOL);
                    res.Set(res.Prefix, "", ((a.ValueInt == b.ValueInt) ? 1 : 0).ToString(), true, EnumDataType.E_BOOL);
                    res.UseThis(EnumDataType.E_BOOL);
                    break;
                case '+':
                    ++this.index;
                    a = this.calculate();
                    b = this.calculate();
                    res = new Variable("", a.IsComputable && b.IsComputable, EnumDataType.E_NUMBER);
                    res.Set(res.Prefix, "", (a.ValueInt + b.ValueInt).ToString(), true, EnumDataType.E_NUMBER);
                    res.UseThis(EnumDataType.E_NUMBER);
                    break;
                case '-':
                    ++this.index;
                    a = this.calculate();
                    b = this.calculate();
                    res = new Variable("", a.IsComputable && b.IsComputable, EnumDataType.E_NUMBER);
                    res.Set(res.Prefix, "", (a.ValueInt - b.ValueInt).ToString(), true, EnumDataType.E_NUMBER);
                    res.UseThis(EnumDataType.E_NUMBER);
                    break;
                case '*':
                    ++this.index;
                    a = this.calculate();
                    b = this.calculate();
                    res = new Variable("", a.IsComputable && b.IsComputable, EnumDataType.E_NUMBER);
                    res.Set(res.Prefix, "", (a.ValueInt * b.ValueInt).ToString(), true, EnumDataType.E_NUMBER);
                    res.UseThis(EnumDataType.E_NUMBER);
                    break;
                case '/':
                    ++this.index;
                    a = this.calculate();
                    b = this.calculate();
                    res = new Variable("", a.IsComputable && b.IsComputable, EnumDataType.E_NUMBER);
                    if (b.ValueInt != 0)
                    {
                        res.Set(res.Prefix, "", (a.ValueInt / b.ValueInt).ToString(), true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        res.Set(res.Prefix, "", "0", true, EnumDataType.E_NUMBER);
                    }
                    res.UseThis(EnumDataType.E_NUMBER);
                    break;
                case '?':
                    ++this.index;
                    a = this.calculate();
                    b = this.calculate();
                    res = new Variable("", a.IsComputable && b.IsComputable, EnumDataType.E_BOOL);
                    res.Set(res.Prefix, "", ((a.ValueString == b.ValueString) ? 1 : 0).ToString(), true, EnumDataType.E_BOOL);
                    res.UseThis(EnumDataType.E_BOOL);
                    break;
                case '.':
                    ++this.index;
                    a = this.calculate();
                    b = this.calculate();
                    res = new Variable("", a.IsComputable && b.IsComputable, EnumDataType.E_STRING);
                    res.Set(res.Prefix, "", a.ValueString + b.ValueString, true, EnumDataType.E_STRING);
                    res.UseThis(EnumDataType.E_STRING);
                    break;
                case ':':
                    ++this.index;
                    a = this.calculate();
                    b = this.calculate();
                    res = new Variable("", a.IsComputable && b.IsComputable, EnumDataType.E_WCHAR);
                    if (b.ValueInt > 0 && b.ValueInt <= a.ValueString.Length)
                    {
                        res.Set(res.Prefix, "", a.ValueString[b.ValueInt - 1].ToString(), true, EnumDataType.E_WCHAR);
                    }
                    else
                    {
                        res.Set(res.Prefix, "", "", true, EnumDataType.E_WCHAR);
                    }
                    res.UseThis(EnumDataType.E_WCHAR);
                    break;
                case ',':
                    ++this.index;
                    while (this.stock[this.index] == ',')
                    {
                        ++this.index;
                        this.vars.Add(this.calculate());
                    }
                    this.vars.Add(this.calculate());
                    res = this.calculate();
                    break;
                case '@':
                    ++this.index;
                    res = new Variable("", true, EnumDataType.E_NUMBER);
                    res.Set(res.Prefix, "", ((int)this.stock[this.index]).ToString(), true, EnumDataType.E_NUMBER);
                    res.UseThis(EnumDataType.E_NUMBER);
                    ++this.index;
                    break;
                case '!':
                    ++this.index;
                    res = new Variable("", true, EnumDataType.E_STRING);
                    res.Set(res.Prefix, "", this.words[(int)this.stock[this.index]], true, EnumDataType.E_STRING);
                    res.UseThis(EnumDataType.E_STRING);
                    ++this.index;
                    break;
                case 'a':
                    ++this.index;
                    res = new Variable("", true, EnumDataType.E_STRING);
                    res.Set(res.Prefix, "", this.words[(int)this.stock[this.index]], true, EnumDataType.E_STRING);
                    res.UseThis(EnumDataType.E_STRING);
                    ++this.index;
                    break;
                case 'ù':
                    ++this.index;
                    string varName = this.words[(int)this.stock[this.index]];
                    IData var = this.scope.GetVariable(varName);
                    res = new Variable("", true, EnumDataType.E_ANY);
                    res.CopyFrom(var, false);
                    ++this.index;
                    break;
                case 'µ':
                    ++this.index;
                    string functionName = this.words[(int)this.stock[this.index]];
                    res = new Variable("", true, EnumDataType.E_ANY);
                    ++this.index;
                    switch (functionName)
                    {
                        case "startCanvas":
                            {
                                this.canvas = new Form();
                                Panel p = new Panel();
                                p.Dock = DockStyle.Fill;
                                p.BackColor = Color.White;
                                p.ForeColor = Color.Black;
                                this.vars.Clear();
                                a = this.calculate();
                                this.canvas.Width = this.vars[0].ValueInt;
                                this.canvas.Height = a.ValueInt;
                                this.canvas.Controls.Add(p);
                                this.canvas.Show();
                                break;
                            }
                        case "ellipse":
                            {
                                Graphics g = this.canvas.Controls[0].CreateGraphics();
                                g.DrawEllipse(Pens.Black, new Rectangle(200,200,100,100));
                                this.vars.Clear();
                                a = this.calculate();
                                break;
                            }
                        case "left":
                            {
                                res = this.calculate();
                                break;
                            }
                        case "top":
                            {
                                res = this.calculate();
                                break;
                            }
                        case "color":
                            {
                                res = this.calculate();
                                break;
                            }
                        case "array":
                            {
                                this.vars.Clear();
                                a = this.calculate();
                                string list = String.Empty;
                                bool isComputable = true;
                                foreach (Variable v in this.vars)
                                {
                                    list += v.ValueString + ",";
                                    if (!v.IsComputable) isComputable = false;
                                }
                                list += a.ValueString;
                                if (!a.IsComputable) isComputable = false;
                                res = new Variable("", isComputable, EnumDataType.E_STRING);
                                res.Set(res.Prefix, "", list, isComputable, EnumDataType.E_STRING);
                                res.UseThis(EnumDataType.E_STRING);
                                break;
                            }
                        case "upper":
                            {
                                a = this.calculate();
                                res = new Variable("", a.IsComputable, EnumDataType.E_STRING);
                                res.Set(res.Prefix, "", a.ValueString.ToUpper(), true, EnumDataType.E_STRING);
                                res.UseThis(EnumDataType.E_STRING);
                                break;
                            }
                        default:
                            break;
                    }
                    break;
            }
            return res;
        }

        private IDataNotInScope ConvertExpressionToVBScript(EnumDataType output)
        {
            IDataNotInScope a = null, b = null;
            IDataNotInScope result = new Variable("", true);
            switch (this.stock[this.index])
            {
                case '>':
                    ++this.index;
                    a = ConvertExpressionToVBScript(EnumDataType.E_NUMBER);
                    b = ConvertExpressionToVBScript(EnumDataType.E_NUMBER);
                    if (a.IsComputable && b.IsComputable)
                    {
                        result.Set("", "", ((a.ValueInt > b.ValueInt) ? 1 : 0).ToString(), true, EnumDataType.E_BOOL);
                    }
                    else
                    {
                        result.Set("", "", "(" + a.ValueString + ") > (" + b.ValueString + ")", false, EnumDataType.E_BOOL);
                    }
                    result.UseThis(EnumDataType.E_BOOL);
                    result = TypedVariable.ConvertToVBScript(converter, EnumDataType.E_BOOL, output, result);
                    break;
                case '<':
                    ++this.index;
                    a = ConvertExpressionToVBScript(EnumDataType.E_NUMBER);
                    b = ConvertExpressionToVBScript(EnumDataType.E_NUMBER);
                    if (a.IsComputable && b.IsComputable)
                    {
                        result.Set("", "", ((a.ValueInt < b.ValueInt) ? 1 : 0).ToString(), true, EnumDataType.E_BOOL);
                    }
                    else
                    {
                        result.Set("", "", "(" + a.ValueString + ") < (" + b.ValueString + ")", false, EnumDataType.E_BOOL);
                    }
                    result.UseThis(EnumDataType.E_BOOL);
                    result = TypedVariable.ConvertToVBScript(converter, EnumDataType.E_BOOL, output, result);
                    break;
                case '=':
                    ++this.index;
                    a = ConvertExpressionToVBScript(EnumDataType.E_NUMBER);
                    b = ConvertExpressionToVBScript(EnumDataType.E_NUMBER);
                    if (a.IsComputable && b.IsComputable)
                    {
                        result.Set("", "", ((a.ValueInt == b.ValueInt) ? 1 : 0).ToString(), true, EnumDataType.E_BOOL);
                    }
                    else
                    {
                        result.Set("", "", "(" + a.ValueString + ") = (" + b.ValueString + ")", false, EnumDataType.E_BOOL);
                    }
                    result.UseThis(EnumDataType.E_BOOL);
                    result = TypedVariable.ConvertToVBScript(converter, EnumDataType.E_BOOL, output, result);
                    break;
                case '+':
                    ++this.index;
                    a = ConvertExpressionToVBScript(EnumDataType.E_NUMBER);
                    b = ConvertExpressionToVBScript(EnumDataType.E_NUMBER);
                    if (a.IsComputable && b.IsComputable)
                    {
                        result.Set("", "", (a.ValueInt + b.ValueInt).ToString(), true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        result.Set("", "", a.ValueString + " + " + b.ValueString, false, EnumDataType.E_NUMBER);
                    }
                    result.UseThis(EnumDataType.E_NUMBER);
                    result = TypedVariable.ConvertToVBScript(converter, EnumDataType.E_NUMBER, output, result);
                    break;
                case '-':
                    ++this.index;
                    a = ConvertExpressionToVBScript(EnumDataType.E_NUMBER);
                    b = ConvertExpressionToVBScript(EnumDataType.E_NUMBER);
                    if (a.IsComputable && b.IsComputable)
                    {
                        result.Set("", "", (a.ValueInt - b.ValueInt).ToString(), true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        result.Set("", "", a.ValueString + " - (" + b.ValueString + ")", false, EnumDataType.E_NUMBER);
                    }
                    result.UseThis(EnumDataType.E_NUMBER);
                    result = TypedVariable.ConvertToVBScript(converter, EnumDataType.E_NUMBER, output, result);
                    break;
                case '*':
                    ++this.index;
                    a = ConvertExpressionToVBScript(EnumDataType.E_NUMBER);
                    b = ConvertExpressionToVBScript(EnumDataType.E_NUMBER);
                    if (a.IsComputable && b.IsComputable)
                    {
                        result.Set("", "", (a.ValueInt * b.ValueInt).ToString(), true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        result.Set("", "", "(" + a.ValueString + ") * (" + b.ValueString + ")", false, EnumDataType.E_NUMBER);
                    }
                    result.UseThis(EnumDataType.E_NUMBER);
                    result = TypedVariable.ConvertToVBScript(converter, EnumDataType.E_NUMBER, output, result);
                    break;
                case '/':
                    ++this.index;
                    a = ConvertExpressionToVBScript(EnumDataType.E_NUMBER);
                    b = ConvertExpressionToVBScript(EnumDataType.E_NUMBER);
                    if (a.IsComputable && b.IsComputable)
                    {
                        if (b.ValueInt != 0)
                            result.Set("", "", (a.ValueInt / b.ValueInt).ToString(), true, EnumDataType.E_NUMBER);
                        else
                            result.Set("", "", "0", true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        result.Set("", "", "Int((" + a.ValueString + ") / (" + b.ValueString + "))", false, EnumDataType.E_NUMBER);
                    }
                    result.UseThis(EnumDataType.E_NUMBER);
                    result = TypedVariable.ConvertToVBScript(converter, EnumDataType.E_NUMBER, output, result);
                    break;
                case '?':
                    ++this.index;
                    a = ConvertExpressionToVBScript(EnumDataType.E_STRING);
                    b = ConvertExpressionToVBScript(EnumDataType.E_STRING);
                    if (a.IsComputable && b.IsComputable)
                    {
                        result.Set("", "", ((a.ValueString == b.ValueString) ? 1 : 0).ToString(), true, EnumDataType.E_BOOL);
                    }
                    else
                    {
                        result.Set("", "", "(" + a.ValueString + ") = (" + b.ValueString + ")", false, EnumDataType.E_BOOL);
                    }
                    result.UseThis(EnumDataType.E_BOOL);
                    result = TypedVariable.ConvertToVBScript(converter, EnumDataType.E_BOOL, output, result);
                    break;
                case '.':
                    ++this.index;
                    a = ConvertExpressionToVBScript(EnumDataType.E_STRING);
                    b = ConvertExpressionToVBScript(EnumDataType.E_STRING);
                    if (a.IsComputable && b.IsComputable)
                    {
                        string str = String.Empty;
                        if (a.ValueInt == 0 && !String.IsNullOrEmpty(a.ValueString))
                        {
                            if (a.ValueString.StartsWith("\"") && a.ValueString.EndsWith("\""))
                            {
                                str += a.ValueString.Substring(1, a.ValueString.Length - 2);
                            }
                            else
                            {
                                str += a.ValueString;
                            }
                        }
                        else
                        {
                            str += a.ValueString;
                        }
                        if (b.ValueInt == 0 && !String.IsNullOrEmpty(b.ValueString))
                        {
                            if (b.ValueString.StartsWith("\"") && b.ValueString.EndsWith("\""))
                            {
                                str += b.ValueString.Substring(1, b.ValueString.Length - 2);
                            }
                            else
                            {
                                str += b.ValueString;
                            }
                        }
                        else
                        {
                            str += b.ValueString;
                        }
                        result.Set("", "", "\"" + str + "\"", true, EnumDataType.E_STRING);
                    }
                    else
                    {
                        result.Set("", "", a.ValueString + " & " + b.ValueString, false, EnumDataType.E_STRING);
                    }
                    result.UseThis(EnumDataType.E_STRING);
                    result = TypedVariable.ConvertToVBScript(converter, EnumDataType.E_STRING, output, result);
                    break;
                case ':':
                    ++this.index;
                    a = ConvertExpressionToVBScript(EnumDataType.E_STRING);
                    b = ConvertExpressionToVBScript(EnumDataType.E_NUMBER);
                    if (a.IsComputable && b.IsComputable)
                    {
                        if (a.ValueString.StartsWith("\""))
                            a.Value = a.ValueString.Substring(1, a.ValueString.Length - 2);
                        if (b.ValueInt > 0 && b.ValueInt <= a.ValueString.Length)
                        {
                            result.Set("", "", "\"" + a.ValueString[b.ValueInt - 1].ToString() + "\"", true, EnumDataType.E_STRING);
                        }
                        else
                        {
                            result.Set("", "", "", true, EnumDataType.E_STRING);
                        }
                    }
                    else
                    {
                        result.Set("", "", "Mid(" + a.ValueString + ", " + b.ValueString + ", 1)", false, EnumDataType.E_STRING);
                    }
                    result.UseThis(EnumDataType.E_STRING);
                    result = TypedVariable.ConvertToVBScript(converter, EnumDataType.E_STRING, output, result);
                    break;
                case '@':
                    ++this.index;
                    // constant integer value
                    result.Set("", "", this.stock[this.index].ToString(), true, EnumDataType.E_NUMBER);
                    result.UseThis(EnumDataType.E_NUMBER);
                    result = TypedVariable.ConvertToVBScript(converter, EnumDataType.E_NUMBER, output, result);
                    ++this.index;
                    break;
                case '!':
                    // constant string
                    ++this.index;
                    result.Set("", "", "\"" + this.words[(int)this.stock[this.index]].Replace("\"", "\"\"") + "\"", true, EnumDataType.E_STRING);
                    result.UseThis(EnumDataType.E_STRING);
                    result = TypedVariable.ConvertToVBScript(converter, EnumDataType.E_STRING, output, result);
                    ++this.index;
                    break;
                case 'a':
                    {
                        ++this.index;
                        string word = this.words[(int)this.stock[this.index]];
                        string str = String.Empty;
                        int count = 0;
                        do
                        {
                            word = word.Substring(1);
                            ++count;
                        } while (word.StartsWith(Compute.StackIdentifier.ToString()));
                        for (int counter = 0; counter < count; ++counter)
                        {
                            str = "/*" + str;
                        }
                        str += word;
                        str = str + "*/";
                        result.Set("", "", MicrosoftCPPConverter.Escape(str), false, EnumDataType.E_STRING);
                        result.UseThis(EnumDataType.E_STRING);
                        result = TypedVariable.ConvertToVBScript(this.converter, EnumDataType.E_STRING, output, result);
                        ++this.index;
                    }
                    break;
                case 'ù':
                    {
                        ++this.index;
                        string varName = this.words[(int)this.stock[this.index]];
                        IData var = this.scope.GetVariable(varName);
                        if (!this.withVariables && var.IsComputable)
                        {
                            if (var.ValueInt == 0)
                            {
                                string s = null;
                                if (var.ValueString.StartsWith("\""))
                                    s = var.ValueString.Substring(1, var.ValueString.Length - 2);
                                else
                                    s = var.ValueString;
                                // string constant value
                                result.Set("", "", "\"" + var.ValueString + "\"", true, EnumDataType.E_STRING);
                                result.UseThis(EnumDataType.E_STRING);
                                result = result = TypedVariable.ConvertToVBScript(this.converter, EnumDataType.E_STRING, output, result);
                            }
                            else
                            {
                                result.Set("", "", var.ValueInt.ToString(), true, EnumDataType.E_NUMBER);
                                result.UseThis(EnumDataType.E_NUMBER);
                                result = result = TypedVariable.ConvertToVBScript(this.converter, EnumDataType.E_NUMBER, output, result);
                            }
                        }
                        else
                        {
                            if (!var.IsGlobal && !this.converter.CurrentFunction.UsedVariables.Exists(new Predicate<string>(delegate(string s)
                            {
                                return var.Name == s;
                            })))
                            {
                                this.converter.CurrentFunction.UsedVariables.Add(var.Name);
                            }
                            result.Set("", "", var.Name, false, var.DataType);
                            result.UseThis(var.DataType);
                            result = result = TypedVariable.ConvertToVBScript(this.converter, var.DataType, output, result);
                        }
                        ++this.index;
                    }
                    break;
                case ',':
                    {
                        bool isComputable = true;
                        IData val = null;
                        string str = String.Empty;
                        ++this.index;
                        while (this.stock[this.index] == ',')
                        {
                            ++this.index;
                            val = ConvertExpressionToVBScript(EnumDataType.E_STRING);
                            if (!val.IsComputable) isComputable = false;
                            str += val.ValueString + " & \",\" & ";
                        }
                        val = ConvertExpressionToVBScript(EnumDataType.E_STRING);
                        if (!val.IsComputable) isComputable = false;
                        str += val.ValueString + " & \",\" & ";
                        val = ConvertExpressionToVBScript(EnumDataType.E_STRING);
                        if (!val.IsComputable) isComputable = false;
                        result.Set("", "", str + val.ValueString, isComputable, EnumDataType.E_STRING);
                        result.UseThis(EnumDataType.E_STRING);
                        result = TypedVariable.ConvertToVBScript(this.converter, EnumDataType.E_STRING, output, result);
                        break;
                    }
                case 'µ':
                    ++this.index;
                    string functionName = this.words[(int)this.stock[this.index]];
                    ++this.index;
                    switch (functionName)
                    {
                        case "array":
                            {
                                result = ConvertExpressionToVBScript(EnumDataType.E_STRING);
                                result = TypedVariable.ConvertToVBScript(this.converter, EnumDataType.E_STRING, output, result);
                                break;
                            }
                        case "upper":
                            {
                                IData val = ConvertExpressionToVBScript(EnumDataType.E_STRING);
                                if (val.IsComputable)
                                {
                                    string s = null;
                                    if (val.ValueString.StartsWith("\""))
                                        s = val.ValueString.Substring(1, val.ValueString.Length - 2);
                                    else
                                        s = val.ValueString;
                                    result.Set("", "", s.ToUpper(), true, EnumDataType.E_STRING);
                                }
                                else
                                {
                                    result.Set("", "", "UCase(" + val.ValueString + ")", false, EnumDataType.E_STRING);
                                }
                                result.UseThis(EnumDataType.E_STRING);
                                result = TypedVariable.ConvertToVBScript(this.converter, EnumDataType.E_STRING, output, result);
                                break;
                            }
                        default:
                            break;
                    }
                    break;
            }
            return result;
        }

        private IDataNotInScope ConvertExpressionToPowerShell(EnumDataType output, bool first)
        {
            IDataNotInScope a = null, b = null;
            IDataNotInScope result = new Variable("", true);
            this.cannotRef = !first;
            switch (this.stock[this.index])
            {
                case '>':
                    ++this.index;
                    a = this.ConvertExpressionToPowerShell(EnumDataType.E_NUMBER, false);
                    b = this.ConvertExpressionToPowerShell(EnumDataType.E_NUMBER, false);
                    if (a.IsComputable && b.IsComputable)
                    {
                        result.Set("", "", ((a.ValueInt > b.ValueInt) ? 1 : 0).ToString(), true, EnumDataType.E_BOOL);
                    }
                    else
                    {
                        result.Set("", "", "(" + a.ValueString + ") -gt (" + b.ValueString + ")", false, EnumDataType.E_BOOL);
                    }
                    result.UseThis(EnumDataType.E_BOOL);
                    result = TypedVariable.ConvertToPowerShell(converter, EnumDataType.E_BOOL, output, result);
                    break;
                case '<':
                    ++this.index;
                    a = this.ConvertExpressionToPowerShell(EnumDataType.E_NUMBER, false);
                    b = this.ConvertExpressionToPowerShell(EnumDataType.E_NUMBER, false);
                    if (a.IsComputable && b.IsComputable)
                    {
                        result.Set("", "", ((a.ValueInt < b.ValueInt) ? 1 : 0).ToString(), true, EnumDataType.E_BOOL);
                    }
                    else
                    {
                        result.Set("", "", "(" + a.ValueString + ") -lt (" + b.ValueString + ")", false, EnumDataType.E_BOOL);
                    }
                    result.UseThis(EnumDataType.E_BOOL);
                    result = TypedVariable.ConvertToPowerShell(converter, EnumDataType.E_BOOL, output, result);
                    break;
                case '=':
                    ++this.index;
                    a = this.ConvertExpressionToPowerShell(EnumDataType.E_NUMBER, false);
                    b = this.ConvertExpressionToPowerShell(EnumDataType.E_NUMBER, false);
                    if (a.IsComputable && b.IsComputable)
                    {
                        result.Set("", "", ((a.ValueInt == b.ValueInt) ? 1 : 0).ToString(), true, EnumDataType.E_BOOL);
                    }
                    else
                    {
                        result.Set("", "", "(" + a.ValueString + ") -eq (" + b.ValueString + ")", false, EnumDataType.E_BOOL);
                        result.IsComputable = false;
                    }
                    result.UseThis(EnumDataType.E_BOOL);
                    result = TypedVariable.ConvertToPowerShell(converter, EnumDataType.E_BOOL, output, result);
                    break;
                case '+':
                    ++this.index;
                    a = this.ConvertExpressionToPowerShell(EnumDataType.E_NUMBER, false);
                    b = this.ConvertExpressionToPowerShell(EnumDataType.E_NUMBER, false);
                    if (a.IsComputable && b.IsComputable)
                    {
                        result.Set("", "", (a.ValueInt + b.ValueInt).ToString(), true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        result.Set("", "", a.ValueString + " + " + b.ValueString, false, EnumDataType.E_NUMBER);
                    }
                    result.UseThis(EnumDataType.E_NUMBER);
                    result = TypedVariable.ConvertToPowerShell(converter, EnumDataType.E_NUMBER, output, result);
                    break;
                case '-':
                    ++this.index;
                    a = this.ConvertExpressionToPowerShell(EnumDataType.E_NUMBER, false);
                    b = this.ConvertExpressionToPowerShell(EnumDataType.E_NUMBER, false);
                    if (a.IsComputable && b.IsComputable)
                    {
                        result.Set("", "", (a.ValueInt - b.ValueInt).ToString(), true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        result.Set("", "", a.ValueString + " - (" + b.ValueString + ")", false, EnumDataType.E_NUMBER);
                    }
                    result.UseThis(EnumDataType.E_NUMBER);
                    result = TypedVariable.ConvertToPowerShell(converter, EnumDataType.E_NUMBER, output, result);
                    break;
                case '*':
                    ++this.index;
                    a = this.ConvertExpressionToPowerShell(EnumDataType.E_NUMBER, false);
                    b = this.ConvertExpressionToPowerShell(EnumDataType.E_NUMBER, false);
                    if (a.IsComputable && b.IsComputable)
                    {
                        result.Set("", "", (a.ValueInt * b.ValueInt).ToString(), true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        result.Set("", "", "(" + a.ValueString + ") * (" + b.ValueString + ")", false, EnumDataType.E_NUMBER);
                    }
                    result.UseThis(EnumDataType.E_NUMBER);
                    result = TypedVariable.ConvertToPowerShell(converter, EnumDataType.E_NUMBER, output, result);
                    break;
                case '/':
                    ++this.index;
                    a = this.ConvertExpressionToPowerShell(EnumDataType.E_NUMBER, false);
                    b = this.ConvertExpressionToPowerShell(EnumDataType.E_NUMBER, false);
                    if (a.IsComputable && b.IsComputable)
                    {
                        if (b.ValueInt != 0)
                            result.Set("", "", (a.ValueInt / b.ValueInt).ToString(), true, EnumDataType.E_NUMBER);
                        else
                            result.Set("", "", "0", true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        result.Set("", "", "[Math]::Floor((" + a.ValueString + ") / (" + b.ValueString + "))", false, EnumDataType.E_NUMBER);
                    }
                    result.UseThis(EnumDataType.E_NUMBER);
                    result = TypedVariable.ConvertToPowerShell(converter, EnumDataType.E_NUMBER, output, result);
                    break;
                case '?':
                    ++this.index;
                    a = this.ConvertExpressionToPowerShell(EnumDataType.E_STRING, false);
                    b = this.ConvertExpressionToPowerShell(EnumDataType.E_STRING, false);
                    if (a.IsComputable && b.IsComputable)
                    {
                        result.Set("", "", ((a.ValueString == b.ValueString) ? 1 : 0).ToString(), true, EnumDataType.E_BOOL);
                        result.IsComputable = true;
                    }
                    else
                    {
                        result.Set("", "", "(" + a.ValueString + ") -eq (" + b.ValueString + ")", false, EnumDataType.E_BOOL);
                    }
                    result.UseThis(EnumDataType.E_BOOL);
                    result = TypedVariable.ConvertToPowerShell(converter, EnumDataType.E_BOOL, output, result);
                    break;
                case '.':
                    ++this.index;
                    a = this.ConvertExpressionToPowerShell(EnumDataType.E_STRING, false);
                    b = this.ConvertExpressionToPowerShell(EnumDataType.E_STRING, false);
                    if (a.IsComputable && b.IsComputable)
                    {
                        string str = String.Empty;
                        if (a.ValueInt == 0 && !String.IsNullOrEmpty(a.ValueString))
                        {
                            if (a.ValueString.StartsWith("\"") && a.ValueString.EndsWith("\""))
                            {
                                str += a.ValueString.Substring(1, a.ValueString.Length - 2);
                            }
                            else
                            {
                                str += a.ValueString;
                            }
                        }
                        else
                        {
                            str += a.ValueString;
                        }
                        if (b.ValueInt == 0 && !String.IsNullOrEmpty(b.ValueString))
                        {
                            if (b.ValueString.StartsWith("\"") && b.ValueString.EndsWith("\""))
                            {
                                str += b.ValueString.Substring(1, b.ValueString.Length - 2);
                            }
                            else
                            {
                                str += b.ValueString;
                            }
                        }
                        else
                        {
                            str += b.ValueString;
                        }
                        result.Set("", "", "\"" + str + "\"", true, EnumDataType.E_STRING_OBJECT);
                    }
                    else
                    {
                        result.Set("", "", a.ValueString + " + " + b.ValueString, false, EnumDataType.E_STRING_OBJECT);
                    }
                    result.UseThis(EnumDataType.E_STRING_OBJECT);
                    result = TypedVariable.ConvertToPowerShell(converter, EnumDataType.E_STRING_OBJECT, output, result);
                    break;
                case ':':
                    ++this.index;
                    a = this.ConvertExpressionToPowerShell(EnumDataType.E_STRING, false);
                    b = this.ConvertExpressionToPowerShell(EnumDataType.E_NUMBER, false);
                    if (a.IsComputable && b.IsComputable)
                    {
                        if (a.ValueString.StartsWith("\""))
                            a.Value = a.ValueString.Substring(1, a.ValueString.Length - 2);
                        if (b.ValueInt > 0 && b.ValueInt <= a.ValueString.Length)
                        {
                            result.Set("", "", "\"" + a.ValueString[b.ValueInt - 1].ToString() + "\"", true, EnumDataType.E_STRING);
                        }
                        else
                        {
                            result.Set("", "", "", true, EnumDataType.E_STRING);
                        }
                    }
                    else
                    {
                        result.Set("", "", "(" + a.ValueString + ")[" + b.ValueString + " - 1]", false, EnumDataType.E_STRING);
                    }
                    result.UseThis(EnumDataType.E_STRING);
                    result = TypedVariable.ConvertToPowerShell(converter, EnumDataType.E_STRING, output, result);
                    break;
                case '@':
                    ++this.index;
                    // constant integer value
                    result.Set("", "", this.stock[this.index].ToString(), true, EnumDataType.E_NUMBER);
                    result.UseThis(EnumDataType.E_NUMBER);
                    result = TypedVariable.ConvertToPowerShell(converter, EnumDataType.E_NUMBER, output, result);
                    ++this.index;
                    break;
                case '!':
                    ++this.index;
                    // constant string
                    result.Set("", "", "\"" + PowerShellConverter.Escape(this.words[(int)this.stock[this.index]]) + "\"", true, EnumDataType.E_STRING);
                    result.UseThis(EnumDataType.E_STRING);
                    result = TypedVariable.ConvertToPowerShell(converter, EnumDataType.E_STRING, output, result);
                    ++this.index;
                    break;
                case 'a':
                    {
                        ++this.index;
                        string word = this.words[(int)this.stock[this.index]];
                        string str = String.Empty;
                        int count = 0;
                        do
                        {
                            word = word.Substring(1);
                            ++count;
                        } while (word.StartsWith(Compute.StackIdentifier.ToString()));
                        for (int counter = 0; counter < count; ++counter)
                        {
                            str = "<!--$" + str;
                        }
                        str += word;
                        for (int counter = 0; counter < count; ++counter)
                        {
                            str = str + "-->";
                        }
                        result.Set("", "", PowerShellConverter.Escape(str), false, EnumDataType.E_STRING);
                        result.UseThis(EnumDataType.E_STRING);
                        result = TypedVariable.ConvertToPowerShell(converter, EnumDataType.E_STRING, output, result);
                        ++this.index;
                    }
                    break;
                case 'ù':
                    {
                        ++this.index;
                        string varName = this.words[(int)this.stock[this.index]];
                        IData var = this.scope.GetVariable(varName);
                        if (!this.withVariables && var.IsComputable)
                        {
                            if (var.ValueInt == 0)
                            {
                                string s = null;
                                if (var.ValueString.StartsWith("\""))
                                    s = var.ValueString.Substring(1, var.ValueString.Length - 2);
                                else
                                    s = var.ValueString;
                                // string constant value
                                result.Set("", "", "\"" + s + "\"", true, EnumDataType.E_STRING);
                                result.UseThis(EnumDataType.E_STRING);
                                result = TypedVariable.ConvertToPowerShell(converter, EnumDataType.E_STRING, output, result);
                            }
                            else
                            {
                                result.Set("", "", var.ValueInt.ToString(), true, EnumDataType.E_NUMBER);
                                result.UseThis(EnumDataType.E_NUMBER);
                                result = TypedVariable.ConvertToPowerShell(converter, EnumDataType.E_NUMBER, output, result);
                            }
                        }
                        else
                        {
                            if (!var.IsGlobal && !this.converter.CurrentFunction.UsedVariables.Exists(new Predicate<string>(delegate(string s) {
                                return var.Name == s;
                            })))
                            {
                                this.converter.CurrentFunction.UsedVariables.Add(var.Name);
                            }
                            if (!var.IsGlobal)
                                if (first)
                                    if (this.isByRef && var.DataType == output && (this.index + 1) == this.stock.Count) // il faut être sûr qu'il n'y aura pas de conversion de cette variable après ni d'instructions suivantes
                                        result.Set("", "", "S[byref:" + PowerShellConverter.Escape(var.Name) + "]", false, var.DataType);
                                    else
                                        result.Set("", "", "S[byvalue:" + PowerShellConverter.Escape(var.Name) + "]", false, var.DataType);
                                else
                                    result.Set("", "", "S[byvalue:" + PowerShellConverter.Escape(var.Name) + "]", false, var.DataType);
                            else
                                result.Set("", "", "$" + PowerShellConverter.Escape(var.Name), false, var.DataType);
                            result.UseThis(var.DataType);
                            result = TypedVariable.ConvertToPowerShell(converter, var.DataType, output, result);
                        }
                        ++this.index;
                    }
                    break;
                case ',':
                    {
                        bool isComputable = true;
                        IData val = null;
                        string str = String.Empty;
                        ++this.index;
                        while (this.stock[this.index] == ',')
                        {
                            ++this.index;
                            val = this.ConvertExpressionToPowerShell(EnumDataType.E_STRING, false);
                            if (!val.IsComputable) isComputable = false;
                            str += val.ValueString + " + \",\" + ";
                        }
                        val = this.ConvertExpressionToPowerShell(EnumDataType.E_STRING, false);
                        if (!val.IsComputable) isComputable = false;
                        str += val.ValueString + " + \",\" + ";
                        val = this.ConvertExpressionToPowerShell(EnumDataType.E_STRING, false);
                        if (!val.IsComputable) isComputable = false;
                        result.Set("", "", str + val.ValueString, isComputable, EnumDataType.E_STRING);
                        result.UseThis(EnumDataType.E_STRING);
                        result = TypedVariable.ConvertToPowerShell(converter, EnumDataType.E_STRING, output, result);
                        break;
                    }
                case 'µ':
                    ++this.index;
                    string functionName = this.words[(int)this.stock[this.index]];
                    ++this.index;
                    switch (functionName)
                    {
                        case "array":
                            {
                                result = this.ConvertExpressionToPowerShell(EnumDataType.E_STRING, false);
                                break;
                            }
                        case "upper":
                            {
                                IData val = this.ConvertExpressionToPowerShell(EnumDataType.E_STRING, false);
                                if (val.IsComputable)
                                {
                                    string s = null;
                                    if (val.ValueString.StartsWith("\""))
                                        s = val.ValueString.Substring(1, val.ValueString.Length - 2);
                                    else
                                        s = val.ValueString;
                                    result.Set("", "", s.ToUpper(), true, EnumDataType.E_STRING);
                                    result.UseThis(EnumDataType.E_STRING);
                                    result = TypedVariable.ConvertToPowerShell(converter, EnumDataType.E_STRING, output, result);
                                }
                                else
                                {
                                    result.Set("", "", "(" + val.ValueString + ").ToUpper()", false, EnumDataType.E_STRING);
                                    result.UseThis(EnumDataType.E_STRING);
                                    result = TypedVariable.ConvertToPowerShell(converter, EnumDataType.E_STRING, output, result);
                                }
                                break;
                            }
                        default:
                            break;
                    }
                    break;
            }
            return result;
        }

        private string MakeNewVariable()
        {
            ++this.converter.CurrentFunction.PrivateVariableCounter;
            return "x" + this.converter.CurrentFunction.PrivateVariableCounter.ToString();
        }

        private IDataNotInScope ConvertExpressionToCPP(EnumDataType output, bool first)
        {
            IDataNotInScope a = null, b = null;
            IDataNotInScope result = new Variable("", true);
            this.cannotRef = !first;
            switch (this.stock[this.index])
            {
                case '>':
                    ++this.index;
                    a = this.ConvertExpressionToCPP(EnumDataType.E_NUMBER, false);
                    b = this.ConvertExpressionToCPP(EnumDataType.E_NUMBER, false);
                    if (a.IsComputable && b.IsComputable)
                    {
                        result.Set("", "", (a.ValueInt > b.ValueInt).ToString(), true, EnumDataType.E_BOOL);
                    }
                    else
                    {
                        result.Set("", "", "(" + a.ValueString + ") > (" + b.ValueString + ")", false, EnumDataType.E_BOOL);
                    }
                    result.UseThis(EnumDataType.E_BOOL);
                    result = TypedVariable.ConvertToCPP(this.converter, EnumDataType.E_BOOL, output, result);
                    break;
                case '<':
                    ++this.index;
                    a = this.ConvertExpressionToCPP(EnumDataType.E_NUMBER, false);
                    b = this.ConvertExpressionToCPP(EnumDataType.E_NUMBER, false);
                    if (a.IsComputable && b.IsComputable)
                    {
                        result.Set("", "", (a.ValueInt < b.ValueInt).ToString(), true, EnumDataType.E_BOOL);
                    }
                    else
                    {
                        result.Set("", "", "(" + a.ValueString + ") < (" + b.ValueString + ")", false, EnumDataType.E_BOOL);
                    }
                    result.UseThis(EnumDataType.E_BOOL);
                    result = TypedVariable.ConvertToCPP(this.converter, EnumDataType.E_BOOL, output, result);
                    break;
                case '=':
                    ++this.index;
                    a = this.ConvertExpressionToCPP(EnumDataType.E_NUMBER, false);
                    b = this.ConvertExpressionToCPP(EnumDataType.E_NUMBER, false);
                    if (a.IsComputable && b.IsComputable)
                    {
                        result.Set("", "", (a.ValueInt == b.ValueInt).ToString(), true, EnumDataType.E_BOOL);
                    }
                    else
                    {
                        result.Set("", "", "(" + a.ValueString + ") == (" + b.ValueString + ")", false, EnumDataType.E_BOOL);
                    }
                    result.UseThis(EnumDataType.E_BOOL);
                    result = TypedVariable.ConvertToCPP(this.converter, EnumDataType.E_BOOL, output, result);
                    break;
                case '+':
                    ++this.index;
                    a = this.ConvertExpressionToCPP(EnumDataType.E_NUMBER, false);
                    b = this.ConvertExpressionToCPP(EnumDataType.E_NUMBER, false);
                    if (a.IsComputable && b.IsComputable)
                    {
                        result.Set("", "", (a.ValueInt + b.ValueInt).ToString(), true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        result.Set("", "", a.ValueString + " + " + b.ValueString, false, EnumDataType.E_NUMBER);
                    }
                    result.UseThis(EnumDataType.E_NUMBER);
                    result = TypedVariable.ConvertToCPP(this.converter, EnumDataType.E_NUMBER, output, result);
                    break;
                case '-':
                    ++this.index;
                    a = this.ConvertExpressionToCPP(EnumDataType.E_NUMBER, false);
                    b = this.ConvertExpressionToCPP(EnumDataType.E_NUMBER, false);
                    if (a.IsComputable && b.IsComputable)
                    {
                        result.Set("", "", (a.ValueInt - b.ValueInt).ToString(), true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        result.Set("", "", a.ValueString + " - (" + b.ValueString + ")", false, EnumDataType.E_NUMBER);
                    }
                    result.UseThis(EnumDataType.E_NUMBER);
                    result = TypedVariable.ConvertToCPP(this.converter, EnumDataType.E_NUMBER, output, result);
                    break;
                case '*':
                    ++this.index;
                    a = this.ConvertExpressionToCPP(EnumDataType.E_NUMBER, false);
                    b = this.ConvertExpressionToCPP(EnumDataType.E_NUMBER, false);
                    if (a.IsComputable && b.IsComputable)
                    {
                        result.Set("", "", (a.ValueInt * b.ValueInt).ToString(), true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        result.Set("", "", "(" + a.ValueString + ") * (" + b.ValueString + ")", false, EnumDataType.E_NUMBER);
                    }
                    result.UseThis(EnumDataType.E_NUMBER);
                    result = TypedVariable.ConvertToCPP(this.converter, EnumDataType.E_NUMBER, output, result);
                    break;
                case '/':
                    ++this.index;
                    a = this.ConvertExpressionToCPP(EnumDataType.E_NUMBER, false);
                    b = this.ConvertExpressionToCPP(EnumDataType.E_NUMBER, false);
                    if (a.IsComputable && b.IsComputable)
                    {
                        if (b.ValueInt != 0)
                        {
                            result.Set("", "", (a.ValueInt / b.ValueInt).ToString(), true, EnumDataType.E_NUMBER);
                        }
                        else
                        {
                            result.Set("", "", "0", true, EnumDataType.E_NUMBER);
                        }
                    }
                    else
                    {
                        result.Set("", "", "(" + a.ValueString + ") / (" + b.ValueString + ")", false, EnumDataType.E_NUMBER);
                    }
                    result.UseThis(EnumDataType.E_NUMBER);
                    result = TypedVariable.ConvertToCPP(this.converter, EnumDataType.E_NUMBER, output, result);
                    break;
                case '?':
                    ++this.index;
                    a = this.ConvertExpressionToCPP(EnumDataType.E_STRING_OBJECT, false);;
                    b = this.ConvertExpressionToCPP(EnumDataType.E_STRING, false);;
                    if (a.IsComputable && b.IsComputable)
                    {
                        result.Set("", "", (a.ValueString == b.ValueString) ? "1" : "0", true, EnumDataType.E_BOOL);
                    }
                    else
                    {
                        result.Set("", "", "(" + a.ValueString + ") == (" + b.ValueString + ")", false, EnumDataType.E_BOOL);
                    }
                    result.UseThis(EnumDataType.E_BOOL);
                    result = TypedVariable.ConvertToCPP(this.converter, EnumDataType.E_BOOL, output, result);
                    break;
                case '.':
                    ++this.index;
                    a = this.ConvertExpressionToCPP(EnumDataType.E_STRING, false);;
                    b = this.ConvertExpressionToCPP(EnumDataType.E_STRING, false);;
                    if (a.IsComputable && b.IsComputable)
                    {
                        string str = String.Empty;
                        if (a.ValueInt == 0 && !String.IsNullOrEmpty(a.ValueString))
                        {
                            if (a.ValueString.StartsWith("L\"") && a.ValueString.EndsWith("\""))
                            {
                                str += a.ValueString.Substring(2, a.ValueString.Length - 3);
                            }
                            else
                            {
                                str += a.ValueString;
                            }
                        }
                        else
                        {
                            str += a.ValueString;
                        }
                        if (b.ValueInt == 0 && !String.IsNullOrEmpty(b.ValueString))
                        {
                            if (b.ValueString.StartsWith("L\"") && b.ValueString.EndsWith("\""))
                            {
                                str += b.ValueString.Substring(2, b.ValueString.Length - 3);
                            }
                            else
                            {
                                str += b.ValueString;
                            }
                        }
                        else
                        {
                            str += b.ValueString;
                        }
                        result.Set("", "", "L\"" + str + "\"", true, EnumDataType.E_STRING);
                        result.UseThis(EnumDataType.E_STRING);
                        result = TypedVariable.ConvertToCPP(this.converter, EnumDataType.E_STRING, output, result);
                    }
                    else
                    {
                        result.Set("", "", a.ValueString + " + " + b.ValueString, false, EnumDataType.E_STRING);
                        result.UseThis(EnumDataType.E_STRING_OBJECT);
                        result = TypedVariable.ConvertToCPP(this.converter, EnumDataType.E_STRING, output, result);
                    }
                    break;
                case ':':
                    ++this.index;
                    a = this.ConvertExpressionToCPP(EnumDataType.E_STRING, false);;
                    b = this.ConvertExpressionToCPP(EnumDataType.E_NUMBER, false);
                    if (a.IsComputable && b.IsComputable)
                    {
                        if (a.ValueString.StartsWith("L\""))
                            a.Value = a.ValueString.Substring(2, a.ValueString.Length - 3);
                        if (b.ValueInt > 0 && b.ValueInt <= a.ValueString.Length)
                        {
                            result.Set("", "", "L'" + a.ValueString[b.ValueInt - 1].ToString() + "'", true, EnumDataType.E_WCHAR);
                            result.UseThis(EnumDataType.E_WCHAR);
                        }
                        else
                        {
                            result.Set("", "", "L''", true, EnumDataType.E_WCHAR);
                            result.UseThis(EnumDataType.E_WCHAR);
                        }
                    }
                    else
                    {
                        result.Set("", "", "(" + a.ValueString + ")[(" + b.ValueString + ")-1]", false, EnumDataType.E_WCHAR);
                        result.UseThis(EnumDataType.E_WCHAR);
                    }
                    result = TypedVariable.ConvertToCPP(this.converter, EnumDataType.E_WCHAR, output, result);
                    break;
                case '@':
                    ++this.index;
                    // constant integer value
                    result.Set("", "", this.stock[this.index].ToString(), true, EnumDataType.E_NUMBER);
                    result.UseThis(EnumDataType.E_NUMBER);
                    ++this.index;
                    result = TypedVariable.ConvertToCPP(this.converter, EnumDataType.E_NUMBER, output, result);
                    break;
                case '!':
                    ++this.index;
                    // constant string
                    result.Set("", "", "L\"" + MicrosoftCPPConverter.Escape(this.words[(int)this.stock[this.index]]) + "\"", true, EnumDataType.E_STRING);
                    result.UseThis(EnumDataType.E_STRING);
                    ++this.index;
                    result = TypedVariable.ConvertToCPP(this.converter, EnumDataType.E_STRING, output, result);
                    break;
                case 'a':
                    {
                        ++this.index;
                        string word = this.words[(int)this.stock[this.index]];
                        string str = String.Empty;
                        int count = 0;
                        do
                        {
                            word = word.Substring(1);
                            ++count;
                        } while (word.StartsWith(Compute.StackIdentifier.ToString()));
                        for (int counter = 0; counter < count; ++counter)
                        {
                            str = "/*" + str;
                        }
                        str += word;
                        str = str + "*/";
                        result.Set("", "", MicrosoftCPPConverter.Escape(str), false, EnumDataType.E_STRING);
                        result.UseThis(EnumDataType.E_STRING);
                        result = TypedVariable.ConvertToCPP(this.converter, EnumDataType.E_STRING, output, result);
                        ++this.index;
                    }
                    break;
                case 'ù':
                    {
                        ++this.index;
                        string varName = this.words[(int)this.stock[this.index]];
                        IData var = this.scope.GetVariable(varName);
                        if (!this.withVariables && var.IsComputable && var.DataType != EnumDataType.E_SIMPLETYPE)
                        {
                            if (var.ValueInt == 0)
                            {
                                string s = null;
                                if (var.ValueString.StartsWith("L\""))
                                    s = var.ValueString.Substring(2, var.ValueString.Length - 3);
                                else
                                    s = var.ValueString;
                                // string constant value
                                result.Set("", "", "L\"" + s + "\"", true, EnumDataType.E_STRING);
                                result.UseThis(EnumDataType.E_STRING);
                                result = TypedVariable.ConvertToCPP(this.converter, EnumDataType.E_STRING, output, result);
                            }
                            else
                            {
                                result.Set("", "", var.ValueInt.ToString(), true, EnumDataType.E_NUMBER);
                                result.UseThis(EnumDataType.E_NUMBER);
                                result = TypedVariable.ConvertToCPP(this.converter, EnumDataType.E_NUMBER, output, result);
                            }
                        }
                        else
                        {
                            if (!var.IsGlobal && !this.converter.CurrentFunction.UsedVariables.Exists(new Predicate<string>(delegate(string s) {
                                return var.Name == s;
                            })))
                            {
                                this.converter.CurrentFunction.UsedVariables.Add(var.Name);
                            }
                            if (!var.IsGlobal)
                                if (first)
                                    if (this.isByRef && var.DataType == output && (this.index + 1) == this.stock.Count) // il faut être sûr qu'il n'y aura pas de conversion de cette variable après ni d'instructions suivantes
                                        result.Set("", "", "$[byref:" + MicrosoftCPPConverter.Escape(var.PrefixedName) + "]", false, var.DataType);
                                    else
                                        result.Set("", "", "$[byvalue:" + MicrosoftCPPConverter.Escape(var.PrefixedName) + "]", false, var.DataType);
                                else
                                    result.Set("", "", "$[byvalue:" + MicrosoftCPPConverter.Escape(var.PrefixedName) + "]", false, var.DataType);
                            else
                                result.Set("", "", MicrosoftCPPConverter.Escape(var.PrefixedName) , false, var.DataType);
                            result.UseThis(var.DataType);
                            // pas de conversion pour une variable SimpleType ou Writer
                            if (var.DataType != EnumDataType.E_SIMPLETYPE && var.DataType != EnumDataType.E_WRITER)
                                result = TypedVariable.ConvertToCPP(this.converter, var.DataType, output, result);
                        }
                        ++this.index;
                    }
                    break;
                case ',':
                    {
                        bool isComputable = true;
                        IData val = null;
                        string str = String.Empty;
                        ++this.index;
                        while (this.stock[this.index] == ',')
                        {
                            ++this.index;
                            val = this.ConvertExpressionToCPP(EnumDataType.E_STRING_OBJECT, false);;
                            if (!val.IsComputable) isComputable = false;
                            str += val.ValueString + " + \",\" + ";
                        }
                        val = this.ConvertExpressionToCPP(EnumDataType.E_STRING_OBJECT, false);;
                        if (!val.IsComputable) isComputable = false;
                        str += val.ValueString + " + \",\" + ";
                        val = this.ConvertExpressionToCPP(EnumDataType.E_STRING_OBJECT, false);;
                        if (!val.IsComputable) isComputable = false;
                        result.Set("", "", str + val.ValueString, isComputable, EnumDataType.E_STRING_OBJECT);
                        result.UseThis(EnumDataType.E_STRING_OBJECT);
                        result = TypedVariable.ConvertToCPP(this.converter, EnumDataType.E_STRING_OBJECT, output, result);
                        break;
                    }
                case 'µ':
                    ++this.index;
                    string functionName = this.words[(int)this.stock[this.index]];
                    ++this.index;
                    switch (functionName)
                    {
                        case "array":
                            {
                                result = this.ConvertExpressionToCPP(EnumDataType.E_STRING_OBJECT, false);;
                                break;
                            }
                        case "upper":
                            {
                                IData val = this.ConvertExpressionToCPP(EnumDataType.E_STRING, false);;
                                if (val.IsComputable)
                                {
                                    string s = null;
                                    if (val.ValueString.StartsWith("L\""))
                                        s = val.ValueString.Substring(2, val.ValueString.Length - 3);
                                    else
                                        s = val.ValueString;
                                    result.Set("", "", "L\"" + s.ToUpper() + "\"", true, EnumDataType.E_STRING);
                                    result.UseThis(EnumDataType.E_STRING);
                                    result = TypedVariable.ConvertToCPP(this.converter, EnumDataType.E_STRING, output, result);
                                }
                                else
                                {
                                    string localName = this.MakeNewVariable();
                                    this.converter.CurrentFunction.AdditionalSource += "wstring " + localName + "(" + val.ValueString + ");" + Environment.NewLine;
                                    this.converter.CurrentFunction.AdditionalSource += "for_each(" + localName + ".begin(), " + localName + ".end(), toupper);" + Environment.NewLine;
                                    result.Set("", "", localName, false, EnumDataType.E_STRING_OBJECT);
                                    result.UseThis(EnumDataType.E_STRING_OBJECT);
                                    result = TypedVariable.ConvertToCPP(this.converter, EnumDataType.E_STRING_OBJECT, output, result);
                                }
                                break;
                            }
                        default:
                            break;
                    }
                    break;
            }
            return result;
        }

        #endregion

        #region Public static Methods
        public static bool IsComputable(ICodeConverter converter, IScope scope, string varName)
        {
            if (scope.Exists(varName))
            {
                IData var = scope.GetVariable(varName);
                if (converter.CurrentFunction.LocalVariables.Exists(new Predicate<IStructure>(delegate(IStructure st) { return st.PrefixedFieldName == var.PrefixedName; })))
                {
                    return var.IsComputable;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Public Methods
        public IDataNotInScope run(string expr, IScope scope)
        {
            this.scope = scope;
            this.parentheses(expr);
            this.index = 0;
            IDataNotInScope result = null;
            try
            {
                result = calculate();
            }
            catch (Exception ex)
            {
                throw new Exception("L'expression '" + expr + "' n'a pas fonctionnée pour la raison suivante :" + Environment.NewLine + ex.ToString());
            }
            return result;
        }

        public void Convert(ICodeConverter converter, string expr, IScope scope, EnumDataType outputType, bool withVariables)
        {
            this.outputType = outputType;
            this.scope = scope;
            this.parentheses(expr);
            this.index = 0;
            this.withVariables = withVariables;
            // on efface les utilisations précédentes
            converter.CurrentFunction.UsedVariables.Clear();
            converter.Convert(this);
        }
        #endregion

        #region ILanguageConverter Membres

        public void WriteInJava(ICodeConverter converter)
        {
        }

        public void WriteInCSharp(ICodeConverter converter)
        {
        }

        public void WriteInMacOSCPP(ICodeConverter converter)
        {
        }

        public void WriteInUnixCPP(ICodeConverter converter)
        {
        }

        public void WriteInMicrosoftCPP(ICodeConverter converter)
        {
            this.converter = converter;
            this.isByRef = converter.CurrentFunction.IsByReferenceReturn;
            IDataNotInScope val = this.ConvertExpressionToCPP(this.outputType, true);
            // si by ref demandé
            if (this.isByRef)
            {
                if (this.cannotRef)
                {
                    throw new InvalidCastException("L'expression ne peut pas être exprimée sous la forme d'une référence");
                }
            }
            converter.CurrentFunction.DataTypeResult = val.DataType;
            converter.CurrentFunction.CacheSource = val.ValueString;
            converter.CurrentFunction.IsComputableExpression = val.IsComputable;
        }

        public void WriteInPerl(ICodeConverter converter)
        {
        }

        public void WriteInPython(ICodeConverter converter)
        {
        }

        public void WriteInVBScript(ICodeConverter converter)
        {
            this.converter = converter;
            this.isByRef = converter.CurrentFunction.IsByReferenceReturn;
            IDataNotInScope val = this.ConvertExpressionToVBScript(this.outputType);
            converter.CurrentFunction.DataTypeResult = val.DataType;
            converter.CurrentFunction.CacheSource = val.ValueString;
            converter.CurrentFunction.IsComputableExpression = val.IsComputable;
        }

        public void WriteInPowerShell(ICodeConverter converter)
        {
            this.converter = converter;
            this.isByRef = converter.CurrentFunction.IsByReferenceReturn;
            IDataNotInScope val = this.ConvertExpressionToPowerShell(this.outputType, true);
            // si by ref demandé
            if (this.isByRef)
            {
                if (this.cannotRef)
                {
                    throw new InvalidCastException("L'expression ne peut pas être exprimée sous la forme d'une référence");
                }
            }
            converter.CurrentFunction.DataTypeResult = val.DataType;
            converter.CurrentFunction.CacheSource = val.ValueString;
            converter.CurrentFunction.IsComputableExpression = val.IsComputable;
        }

        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            if (this.canvas != null) { this.canvas.Dispose(); this.canvas = null; }
        }
        #endregion
    }

    /// <summary>
    /// Class implementation to evaluate (compute or convert) a string expression
    /// </summary>
    [Guid("CA901DF6-4ACC-48B6-8B1A-54406322EE4F")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Expression : IExpression
    {
        #region Public Static Methods
        /// <summary>
        /// Converts an expression to the destination programming language
        /// The converted expression is saved into the string CacheSource
        /// AdditionalSource is a string used to complete conversion with intermediate variable declaration
        /// </summary>
        /// <param name="converter">converter object</param>
        /// <param name="expression">expression string</param>
        /// <param name="obj">the scope object</param>
        /// <param name="addToSource">boolean for add immediately into the source code</param>
        public static void Convert(ICodeConverter converter, string expression, Object obj, bool addToSource)
        {
            if (addToSource)
            {
                converter.CurrentFunction.AdditionalSource = String.Empty;
            }
            Expression expr = new Expression();
            expr.ConvExpr(converter, expression, obj);
            if (addToSource)
            {
                converter.CurrentFunction.AddToSource(converter.CurrentFunction.AdditionalSource);
                converter.CurrentFunction.AdditionalSource = String.Empty;
            }
        }

        /// <summary>
        /// Converts an expression to the destination programming language
        /// The converted expression is saved into the string CacheSource
        /// AdditionalSource is a string used to complete conversion with intermediate variable declaration
        /// </summary>
        /// <param name="converter">converter object</param>
        /// <param name="expression">expression string</param>
        /// <param name="obj">the scope object</param>
        /// <param name="addToSource">boolean for add immediately into the source code</param>
        /// <param name="withVariables">true for just writing variables names and not their values in the result expression</param>
        public static void Convert(ICodeConverter converter, string expression, Object obj, bool addToSource, bool withVariables)
        {
            if (addToSource)
            {
                converter.CurrentFunction.AdditionalSource = String.Empty;
            }
            Expression expr = new Expression();
            expr.ConvExpr(converter, expression, obj, withVariables);
            if (addToSource)
            {
                converter.CurrentFunction.AddToSource(converter.CurrentFunction.AdditionalSource);
                converter.CurrentFunction.AdditionalSource = String.Empty;
            }
        }

        /// <summary>
        /// Converts an expression to the destination programming language
        /// The converted expression is saved into the string CacheSource
        /// AdditionalSource is a string used to complete conversion with intermediate variable declaration
        /// </summary>
        /// <param name="converter">converter object</param>
        /// <param name="expression">expression string</param>
        /// <param name="obj">the scope object</param>
        /// <param name="addToSource">boolean for add immediately into the source code</param>
        /// <param name="desiredType">data type of the resulted expression (it writes conversion expression)</param>
        public static void Convert(ICodeConverter converter, string expression, Object obj, bool addToSource, EnumDataType desiredType)
        {
            if (addToSource)
            {
                converter.CurrentFunction.AdditionalSource = String.Empty;
            }
            Expression expr = new Expression();
            expr.ConvExpr(converter, expression, obj, desiredType);
            if (addToSource)
            {
                converter.CurrentFunction.AddToSource(converter.CurrentFunction.AdditionalSource);
                converter.CurrentFunction.AdditionalSource = String.Empty;
            }
        }

        /// <summary>
        /// Converts an expression to the destination programming language
        /// The converted expression is saved into the string CacheSource
        /// AdditionalSource is a string used to complete conversion with intermediate variable declaration
        /// </summary>
        /// <param name="converter">converter object</param>
        /// <param name="expression">expression string</param>
        /// <param name="obj">the scope object</param>
        /// <param name="addToSource">boolean for add immediately into the source code</param>
        /// <param name="desiredType">data type of the resulted expression (it writes conversion expression)</param>
        /// <param name="withVariables">true for just writing variables names and not their values in the result expression</param>
        public static void Convert(ICodeConverter converter, string expression, Object obj, bool addToSource, EnumDataType desiredType, bool withVariables)
        {
            if (addToSource)
            {
                converter.CurrentFunction.AdditionalSource = String.Empty;
            }
            Expression expr = new Expression();
            expr.ConvExpr(converter, expression, obj, desiredType, withVariables);
            if (addToSource)
            {
                converter.CurrentFunction.AddToSource(converter.CurrentFunction.AdditionalSource);
                converter.CurrentFunction.AdditionalSource = String.Empty;
            }
        }
        #endregion

        #region Private Methods
        private void ConvExpr(ICodeConverter converter, string expr, Object obj)
        {
            Compute c = new Compute();
            IScope scope = obj as IScope;
            if (!String.IsNullOrEmpty(expr))
            {
                c.Convert(converter, expr, scope, EnumDataType.E_ANY, false);
            }
            else
            {
                converter.CurrentFunction.DataTypeResult = EnumDataType.E_STRING;
                converter.CurrentFunction.CacheSource = "\"\"";
                converter.CurrentFunction.IsComputableExpression = true;
            }
        }

        private void ConvExpr(ICodeConverter converter, string expr, Object obj, bool withVariables)
        {
            Compute c = new Compute();
            IScope scope = obj as IScope;
            if (!String.IsNullOrEmpty(expr))
            {
                c.Convert(converter, expr, scope, EnumDataType.E_ANY, withVariables);
            }
            else
            {
                converter.CurrentFunction.DataTypeResult = EnumDataType.E_STRING;
                converter.CurrentFunction.CacheSource = "\"\"";
                converter.CurrentFunction.IsComputableExpression = true;
            }
        }

        private void ConvExpr(ICodeConverter converter, string expr, Object obj, EnumDataType desiredType)
        {
            Compute c = new Compute();
            IScope scope = obj as IScope;
            if (!String.IsNullOrEmpty(expr))
            {
                c.Convert(converter, expr, scope, desiredType, false);
            }
            else
            {
                converter.CurrentFunction.DataTypeResult = EnumDataType.E_STRING;
                converter.CurrentFunction.CacheSource = "\"\"";
                converter.CurrentFunction.IsComputableExpression = true;
            }
        }

        private void ConvExpr(ICodeConverter converter, string expr, Object obj, EnumDataType desiredType, bool withVariables)
        {
            Compute c = new Compute();
            IScope scope = obj as IScope;
            if (!String.IsNullOrEmpty(expr))
            {
                c.Convert(converter, expr, scope, desiredType, withVariables);
            }
            else
            {
                converter.CurrentFunction.DataTypeResult = EnumDataType.E_STRING;
                converter.CurrentFunction.CacheSource = "\"\"";
                converter.CurrentFunction.IsComputableExpression = true;
            }
        }
        #endregion

        #region IExpression Membres

        /// <summary>
        /// Parse and compute an expression string
        /// </summary>
        /// <param name="expr">expression string</param>
        /// <param name="obj">scope object</param>
        /// <returns>result value</returns>
        public IDataNotInScope Evaluate(string expr, Object obj)
        {
            Variable var = new Variable("", true);
            var.Name = "Result";
            Compute c = new Compute();
            IScope scope = obj as IScope;
            if (!String.IsNullOrEmpty(expr))
            {
                IData res = c.run(expr, scope);
                var.Set(res.Prefix, res.BelongsTo, res.ValueString, res.IsComputable, res.DataType);
                var.UseThis(res.DataType);
            }
            else
            {
                var.Set("", "", "", true, EnumDataType.E_STRING);
                var.UseThis(EnumDataType.E_STRING);
            }
            return var;
        }

        #endregion
    }
}
