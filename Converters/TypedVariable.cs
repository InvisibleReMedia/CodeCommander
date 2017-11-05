using System;
using System.Collections.Generic;
using System.Text;
using o2Mate;

namespace Converters
{
    /// <summary>
    /// Variable definition with a type
    /// Usefull for moving the data type
    /// </summary>
    public class TypedVariable : ILanguageConverter
    {
        #region Private Fields
        private string varName;
        private string prefixedName;
        private EnumDataType from, to;
        private IDataNotInScope var;
        private IDataNotInScope changedVar;
        private string convertedValue;
        #endregion

        #region Default Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="varName">variable name</param>
        /// <param name="prefixedName">specific prefix associated with a type</param>
        /// <param name="from">current data type</param>
        /// <param name="type">new date type</param>
        public TypedVariable(string varName, string prefixedName, EnumDataType from, EnumDataType type)
        {
            this.varName = varName;
            this.prefixedName = prefixedName;
            this.from = from;
            this.to = type;
            this.convertedValue = String.Empty;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the variable name
        /// </summary>
        public string VariableName { get { return this.varName; } }

        /// <summary>
        /// Gets the prefix name
        /// </summary>
        public string PrefixedName { get { return this.prefixedName; } }

        /// <summary>
        /// Gets the current data type
        /// </summary>
        public EnumDataType FromType { get { return this.from; } }

        /// <summary>
        /// Gets the new data type
        /// </summary>
        public EnumDataType ToType { get { return this.to; } }
        #endregion

        #region Private Static Methods
        private static string MakeNewVariable(ICodeConverter converter)
        {
            ++converter.CurrentFunction.PrivateVariableCounter;
            return "x" + converter.CurrentFunction.PrivateVariableCounter.ToString();
        }

        private static void SetVar(IDataNotInScope var, string value, bool isComputable, EnumDataType dataType)
        {
            if (var.TypeExists(dataType))
            {
                var.Set(var.PrefixInfo(dataType).Prefix,
                        var.PrefixInfo(dataType).BelongsTo,
                        value,
                        isComputable,
                        dataType);
            }
            else
            {
                var.Set(Scope.StandardPrefix(dataType),
                        var.PrefixInfo(var.DataType).BelongsTo,
                        value,
                        isComputable,
                        dataType);
            }
        }

        #endregion

        #region Public Static Methods
        /// <summary>
        /// Converts a data type to an another and
        /// performs the correct transform code in VBScript
        /// </summary>
        /// <param name="converter">current converter object</param>
        /// <param name="from">the current data type</param>
        /// <param name="to">the new data type</param>
        /// <param name="var">variable object</param>
        /// <returns>a new variable object - with the same variable name and a different prefix but doesn't reside in a scope</returns>
        public static IDataNotInScope ConvertToVBScript(ICodeConverter converter, EnumDataType from, EnumDataType to, IDataNotInScope var)
        {
            Variable output = new Variable(var.BelongsTo, var.IsComputable, var.DataType);
            output.Value = var.ValueString;
            output.Name = var.Name;
            if (from == to || to == EnumDataType.E_ANY) { return output; }
            else if (from == EnumDataType.E_BOOL)
            {
                if (to == EnumDataType.E_NUMBER)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, (var.ValueInt != 0 ? "1" : "0"), true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "CLng(" + var.ValueString + ")", false, EnumDataType.E_NUMBER);
                    }
                    output.UseThis(EnumDataType.E_NUMBER);
                }
                else if (to == EnumDataType.E_WCHAR)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "\"" + (var.ValueInt != 0 ? "1" : "0") + "\"", true, EnumDataType.E_WCHAR);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "CStr(" + var.ValueString + ")", false, EnumDataType.E_WCHAR);
                    }
                    output.UseThis(EnumDataType.E_WCHAR);
                }
                else if (to == EnumDataType.E_STRING)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "\"" + var.ValueString + "\"", true, EnumDataType.E_STRING);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "CStr(" + var.ValueString + ")", false, EnumDataType.E_STRING);
                    }
                    output.UseThis(EnumDataType.E_STRING);
                }
                else if (to == EnumDataType.E_STRING_OBJECT)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "\"" + var.ValueString + "\"", true, EnumDataType.E_STRING_OBJECT);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "CStr(" + var.ValueString + ")", false, EnumDataType.E_STRING_OBJECT);
                    }
                    output.UseThis(EnumDataType.E_STRING_OBJECT);
                }
                else if (to == EnumDataType.E_WRITER)
                {
                    throw new InvalidCastException("Impossible de convertir le booléen '" + var.Name + "' en un objet écrivain");
                }
            }
            else if (from == EnumDataType.E_NUMBER)
            {
                if (to == EnumDataType.E_BOOL)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, (var.ValueInt != 0) ? "1" : "0", true, EnumDataType.E_BOOL);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "((" + var.ValueString + ")<>0 ? 1 : 0)", false, EnumDataType.E_BOOL);
                    }
                    output.UseThis(EnumDataType.E_BOOL);
                }
                else if (to == EnumDataType.E_WCHAR)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "\"" + var.ValueString[0].ToString() + "\"", true, EnumDataType.E_WCHAR);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "Mid(CStr(" + var.ValueString + "), 1, 1)", false, EnumDataType.E_WCHAR);
                    }
                    output.UseThis(EnumDataType.E_WCHAR);
                }
                else if (to == EnumDataType.E_STRING)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "\"" + var.ValueInt + "\"", true, EnumDataType.E_WCHAR);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "CStr(" + var.ValueString + ")", false, EnumDataType.E_WCHAR);
                    }
                    output.UseThis(EnumDataType.E_WCHAR);
                }
                else if (to == EnumDataType.E_STRING_OBJECT)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "\"" + var.ValueInt + "\"", true, EnumDataType.E_WCHAR);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "CStr(" + var.ValueString + ")", false, EnumDataType.E_WCHAR);
                    }
                    output.UseThis(EnumDataType.E_STRING_OBJECT);
                }
                else if (to == EnumDataType.E_WRITER)
                {
                    throw new InvalidCastException("Impossible de convertir le nombre '" + var.Name + "' en un objet écrivain");
                }
            }
            else if (from == EnumDataType.E_STRING)
            {
                if (to == EnumDataType.E_BOOL)
                {
                    if (var.IsComputable)
                    {
                        if (var.ValueInt != 0)
                        {
                            TypedVariable.SetVar(output, "1", true, EnumDataType.E_BOOL);
                        }
                        else
                        {
                            TypedVariable.SetVar(output, "0", true, EnumDataType.E_BOOL);
                        }
                    }
                    else
                    {
                        converter.CurrentFunction.AdditionalSource = "On Error Resume Next" + Environment.NewLine;
                        string localName = TypedVariable.MakeNewVariable(converter);
                        converter.CurrentFunction.AdditionalSource += localName + " = " + "CBool(" + var.ValueString + ")" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += "If Err.Number <> 0 Then:localName = 0:End If" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += "On Error GoTo 0" + Environment.NewLine;
                        TypedVariable.SetVar(output, localName, false, EnumDataType.E_BOOL);
                    }
                    output.UseThis(EnumDataType.E_BOOL);
                }
                else if (to == EnumDataType.E_WCHAR)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "\"" + var.ValueString[0].ToString() + "\"", true, EnumDataType.E_WCHAR);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "Mid(" + var.ValueString + ", 1, 1)", false, EnumDataType.E_WCHAR);
                    }
                    output.UseThis(EnumDataType.E_WCHAR);
                }
                else if (to == EnumDataType.E_NUMBER)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, var.ValueInt.ToString(), true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        converter.CurrentFunction.AdditionalSource = "On Error Resume Next" + Environment.NewLine;
                        string localName = TypedVariable.MakeNewVariable(converter);
                        converter.CurrentFunction.AdditionalSource += localName + " = " + "CLng(" + var.ValueString + ")" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += "If Err.Number <> 0 Then:localName = 0:End If" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += "On Error GoTo 0" + Environment.NewLine;
                        TypedVariable.SetVar(output, localName, false, EnumDataType.E_NUMBER);
                    }
                    output.UseThis(EnumDataType.E_NUMBER);
                }
                else if (to == EnumDataType.E_STRING_OBJECT)
                {
                    TypedVariable.SetVar(output, var.ValueString, output.IsComputable, EnumDataType.E_STRING_OBJECT);
                    output.UseThis(EnumDataType.E_STRING_OBJECT);
                }
                else if (to == EnumDataType.E_WRITER)
                {
                    throw new InvalidCastException("Impossible de convertir la chaîne '" + var.Name + "' en un objet écrivain");
                }
            }
            else if (from == EnumDataType.E_STRING_OBJECT)
            {
                if (to == EnumDataType.E_BOOL)
                {
                    if (var.IsComputable)
                    {
                        if (var.ValueInt != 0)
                        {
                            TypedVariable.SetVar(output, "1", true, EnumDataType.E_BOOL);
                        }
                        else
                        {
                            TypedVariable.SetVar(output, "0", true, EnumDataType.E_BOOL);
                        }
                    }
                    else
                    {
                        converter.CurrentFunction.AdditionalSource = "On Error Resume Next" + Environment.NewLine;
                        string localName = TypedVariable.MakeNewVariable(converter);
                        converter.CurrentFunction.AdditionalSource += localName + " = " + "CBool(" + var.ValueString + ")" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += "If Err.Number <> 0 Then:localName = 0:End If" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += "On Error GoTo 0" + Environment.NewLine;
                        TypedVariable.SetVar(output, localName, false, EnumDataType.E_BOOL);
                    }
                    output.UseThis(EnumDataType.E_BOOL);
                }
                else if (to == EnumDataType.E_WCHAR)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "\"" + var.ValueString[0].ToString() + "\"", true, EnumDataType.E_WCHAR);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "Mid(" + var.ValueString + ", 1, 1)", false, EnumDataType.E_WCHAR);
                    }
                    output.UseThis(EnumDataType.E_WCHAR);
                }
                else if (to == EnumDataType.E_NUMBER)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, var.ValueInt.ToString(), true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        converter.CurrentFunction.AdditionalSource = "On Error Resume Next" + Environment.NewLine;
                        string localName = TypedVariable.MakeNewVariable(converter);
                        converter.CurrentFunction.AdditionalSource += localName + " = " + "CLng(" + var.ValueString + ")" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += "If Err.Number <> 0 Then:localName = 0:End If" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += "On Error GoTo 0" + Environment.NewLine;
                        TypedVariable.SetVar(output, localName, false, EnumDataType.E_NUMBER);
                    }
                    output.UseThis(EnumDataType.E_NUMBER);
                }
                else if (to == EnumDataType.E_STRING)
                {
                    TypedVariable.SetVar(output, var.ValueString, output.IsComputable, EnumDataType.E_STRING);
                    output.UseThis(EnumDataType.E_STRING);
                }
                else if (to == EnumDataType.E_WRITER)
                {
                    throw new InvalidCastException("Impossible de convertir la chaîne '" + var.Name + "' en un objet écrivain");
                }
            }
            else if (from == EnumDataType.E_WCHAR)
            {
                if (to == EnumDataType.E_NUMBER)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, var.ValueString.Substring(1, var.ValueString.Length - 2), true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        converter.CurrentFunction.AdditionalSource = "On Error Resume Next" + Environment.NewLine;
                        string localName = TypedVariable.MakeNewVariable(converter);
                        converter.CurrentFunction.AdditionalSource += localName + " = " + "CLng(" + var.ValueString + ")" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += "If Err.Number <> 0 Then:localName = 0:End If" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += "On Error GoTo 0" + Environment.NewLine;
                        TypedVariable.SetVar(output, localName, false, EnumDataType.E_NUMBER);
                    }
                    output.UseThis(EnumDataType.E_NUMBER);
                }
                else if (to == EnumDataType.E_BOOL)
                {
                    if (var.IsComputable)
                    {
                        int a = 0;
                        if (Int32.TryParse(var.ValueString.Substring(1, var.ValueString.Length - 2), out a))
                        {
                            TypedVariable.SetVar(output, (a > 0 ? "1" : "0"), true, EnumDataType.E_BOOL);
                        }
                        else
                        {
                            TypedVariable.SetVar(output, "0", true, EnumDataType.E_BOOL);
                        }
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "((" + var.ValueString + ") <> \"0\" ? 1 : 0)", false, EnumDataType.E_BOOL);
                    }
                    output.UseThis(EnumDataType.E_BOOL);
                }
                else if (to == EnumDataType.E_STRING)
                {
                    TypedVariable.SetVar(output, var.ValueString, output.IsComputable, EnumDataType.E_STRING);
                    output.UseThis(EnumDataType.E_STRING);
                }
                else if (to == EnumDataType.E_STRING_OBJECT)
                {
                    TypedVariable.SetVar(output, var.ValueString, output.IsComputable, EnumDataType.E_STRING_OBJECT);
                    output.UseThis(EnumDataType.E_STRING_OBJECT);
                }
                else if (to == EnumDataType.E_WRITER)
                {
                    throw new InvalidCastException("Impossible de convertir le caractère '" + var.Name + "' en un objet écrivain");
                }
            }
            else if (from == EnumDataType.E_CONST_STRING_OBJECT)
            {
                if (to == EnumDataType.E_BOOL)
                {
                    if (var.IsComputable)
                    {
                        if (var.ValueInt != 0)
                        {
                            TypedVariable.SetVar(output, "1", true, EnumDataType.E_BOOL);
                        }
                        else
                        {
                            TypedVariable.SetVar(output, "0", true, EnumDataType.E_BOOL);
                        }
                    }
                    else
                    {
                        converter.CurrentFunction.AdditionalSource = "On Error Resume Next" + Environment.NewLine;
                        string localName = TypedVariable.MakeNewVariable(converter);
                        converter.CurrentFunction.AdditionalSource += localName + " = " + "CBool(" + var.ValueString + ")" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += "If Err.Number <> 0 Then:localName = 0:End If" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += "On Error GoTo 0" + Environment.NewLine;
                        TypedVariable.SetVar(output, localName, false, EnumDataType.E_BOOL);
                    }
                    output.UseThis(EnumDataType.E_BOOL);
                }
                else if (to == EnumDataType.E_WCHAR)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "\"" + var.ValueString[0].ToString() + "\"", true, EnumDataType.E_WCHAR);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "Mid(" + var.ValueString + ", 1, 1)", false, EnumDataType.E_WCHAR);
                    }
                    output.UseThis(EnumDataType.E_WCHAR);
                }
                else if (to == EnumDataType.E_NUMBER)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, var.ValueInt.ToString(), true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        converter.CurrentFunction.AdditionalSource = "On Error Resume Next" + Environment.NewLine;
                        string localName = TypedVariable.MakeNewVariable(converter);
                        converter.CurrentFunction.AdditionalSource += localName + " = " + "CLng(" + var.ValueString + ")" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += "If Err.Number <> 0 Then:localName = 0:End If" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += "On Error GoTo 0" + Environment.NewLine;
                        TypedVariable.SetVar(output, localName, false, EnumDataType.E_NUMBER);
                    }
                    output.UseThis(EnumDataType.E_NUMBER);
                }
                else if (to == EnumDataType.E_STRING)
                {
                    TypedVariable.SetVar(output, var.ValueString, output.IsComputable, EnumDataType.E_STRING);
                    output.UseThis(EnumDataType.E_STRING);
                }
                else if (to == EnumDataType.E_STRING_OBJECT)
                {
                    TypedVariable.SetVar(output, var.ValueString, output.IsComputable, EnumDataType.E_STRING_OBJECT);
                    output.UseThis(EnumDataType.E_STRING_OBJECT);
                }
                else if (to == EnumDataType.E_WRITER)
                {
                    throw new InvalidCastException("Impossible de convertir la chaîne '" + var.Name + "' en un objet écrivain");
                }
            }
            else if (from == EnumDataType.E_WRITER)
            {
                throw new InvalidCastException("Impossible de convertir l'objet écrivain '" + var.Name + "'");
            }
            return output;
        }

        /// <summary>
        /// Converts a data type to an another and
        /// performs the correct transform code in PowerShell
        /// </summary>
        /// <param name="converter">current converter object</param>
        /// <param name="from">the current data type</param>
        /// <param name="to">the new data type</param>
        /// <param name="var">variable object</param>
        /// <returns>a new variable object - with the same variable name and a different prefix but doesn't reside in a scope</returns>
        public static IDataNotInScope ConvertToPowerShell(ICodeConverter converter, EnumDataType from, EnumDataType to, IDataNotInScope var)
        {
            Variable output = new Variable(var.BelongsTo, var.IsComputable, var.DataType);
            output.Value = var.ValueString;
            output.Name = var.Name;
            if (from == to || to == EnumDataType.E_ANY) { return output; }
            else if (from == EnumDataType.E_BOOL)
            {
                if (to == EnumDataType.E_NUMBER)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, var.ValueInt == 0 ? "0" : "1", true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "[int](" + var.ValueString + ")", false, EnumDataType.E_NUMBER);
                    }
                    output.UseThis(EnumDataType.E_NUMBER);
                }
                else if (to == EnumDataType.E_WCHAR)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, (var.ValueInt != 0 ? "'1'" : "'0'"), true, EnumDataType.E_WCHAR);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "(" + var.ValueString + " -eq 0 ? '0' : '1')", false, EnumDataType.E_WCHAR);
                    }
                    output.UseThis(EnumDataType.E_WCHAR);
                }
                else if (to == EnumDataType.E_STRING)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "\"" + var.ValueString + "\"", true, EnumDataType.E_STRING);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "(" + var.ValueString + ").ToString()", false, EnumDataType.E_STRING);
                    }
                    output.UseThis(EnumDataType.E_STRING);
                }
                else if (to == EnumDataType.E_STRING_OBJECT)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "\"" + var.ValueString + "\"", true, EnumDataType.E_STRING_OBJECT);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "(" + var.ValueString + ").ToString()", false, EnumDataType.E_STRING_OBJECT);
                    }
                    output.UseThis(EnumDataType.E_STRING_OBJECT);
                }
                else if (to == EnumDataType.E_WRITER)
                {
                    throw new InvalidCastException("Impossible de convertir le booléen '" + var.Name + "' en un objet écrivain");
                }
            }
            else if (from == EnumDataType.E_NUMBER)
            {
                if (to == EnumDataType.E_BOOL)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, (var.ValueInt != 0) ? "1" : "0", true, EnumDataType.E_BOOL);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "((" + var.ValueString + ") -eq 0 ? 0 : 1)", false, EnumDataType.E_BOOL);
                    }
                    output.UseThis(EnumDataType.E_BOOL);
                }
                else if (to == EnumDataType.E_WCHAR)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "'" + var.ValueInt.ToString()[0] + "'", true, EnumDataType.E_WCHAR);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "(" + var.ValueString + ")[0]", false, EnumDataType.E_WCHAR);
                    }
                    output.UseThis(EnumDataType.E_WCHAR);
                }
                else if (to == EnumDataType.E_STRING)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "\"" + var.ValueInt.ToString() + "\"", true, EnumDataType.E_STRING);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "(" + var.ValueString + ").ToString()", false, EnumDataType.E_STRING);
                    }
                    output.UseThis(EnumDataType.E_STRING);
                }
                else if (to == EnumDataType.E_STRING_OBJECT)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "\"" + var.ValueInt.ToString() + "\"", true, EnumDataType.E_STRING_OBJECT);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "(" + var.ValueString + ").ToString()", false, EnumDataType.E_STRING_OBJECT);
                    }
                    output.UseThis(EnumDataType.E_STRING_OBJECT);
                }
                else if (to == EnumDataType.E_WRITER)
                {
                    throw new InvalidCastException("Impossible de convertir le nombre '" + var.Name + "' en un objet écrivain");
                }
            }
            else if (from == EnumDataType.E_STRING)
            {
                if (to == EnumDataType.E_BOOL)
                {
                    if (var.IsComputable)
                    {
                        if (var.ValueInt != 0)
                        {
                            TypedVariable.SetVar(output, "1", true, EnumDataType.E_BOOL);
                        }
                        else
                        {
                            TypedVariable.SetVar(output, "0", true, EnumDataType.E_BOOL);
                        }
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "[Convert]::ToBoolean(" + var.ValueString + ")", false, EnumDataType.E_BOOL);
                    }
                    output.UseThis(EnumDataType.E_BOOL);
                }
                else if (to == EnumDataType.E_WCHAR)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "'" + var.ValueString[0].ToString() + "'", true, EnumDataType.E_WCHAR);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "(" + var.ValueString + ")[0]", false, EnumDataType.E_WCHAR);
                    }
                    output.UseThis(EnumDataType.E_WCHAR);
                }
                else if (to == EnumDataType.E_NUMBER)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, var.ValueInt.ToString(), true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "[Convert]::ToInt32(" + var.ValueString + ")", false, EnumDataType.E_NUMBER);
                    }
                    output.UseThis(EnumDataType.E_NUMBER);
                }
                else if (to == EnumDataType.E_STRING_OBJECT)
                {
                    TypedVariable.SetVar(output, var.ValueString, output.IsComputable, EnumDataType.E_STRING_OBJECT);
                    output.UseThis(EnumDataType.E_STRING_OBJECT);
                }
                else if (to == EnumDataType.E_WRITER)
                {
                    throw new InvalidCastException("Impossible de convertir la chaîne '" + var.Name + "' en un objet écrivain");
                }
            }
            else if (from == EnumDataType.E_STRING_OBJECT)
            {
                if (to == EnumDataType.E_BOOL)
                {
                    if (var.IsComputable)
                    {
                        if (var.ValueInt != 0)
                        {
                            TypedVariable.SetVar(output, "1", true, EnumDataType.E_BOOL);
                        }
                        else
                        {
                            TypedVariable.SetVar(output, "0", true, EnumDataType.E_BOOL);
                        }
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "[Convert]::ToBoolean(" + var.ValueString + ")", false, EnumDataType.E_BOOL);
                    }
                    output.UseThis(EnumDataType.E_BOOL);
                }
                else if (to == EnumDataType.E_WCHAR)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "'" + var.ValueString[0].ToString() + "'", true, EnumDataType.E_WCHAR);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "(" + var.ValueString + ")[0]", false, EnumDataType.E_WCHAR);
                    }
                    output.UseThis(EnumDataType.E_WCHAR);
                }
                else if (to == EnumDataType.E_NUMBER)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, var.ValueInt.ToString(), true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "[Convert]::ToInt32(" + var.ValueString + ")", false, EnumDataType.E_NUMBER);
                    }
                    output.UseThis(EnumDataType.E_NUMBER);
                }
                else if (to == EnumDataType.E_STRING)
                {
                    TypedVariable.SetVar(output, var.ValueString, output.IsComputable, EnumDataType.E_STRING);
                    output.UseThis(EnumDataType.E_STRING);
                }
                else if (to == EnumDataType.E_WRITER)
                {
                    throw new InvalidCastException("Impossible de convertir la chaîne '" + var.Name + "' en un objet écrivain");
                }
            }
            else if (from == EnumDataType.E_WCHAR)
            {
                if (to == EnumDataType.E_NUMBER)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, var.ValueString.Substring(1, var.ValueString.Length - 2), true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "[Convert]::ToInt32(" + var.ValueString + ")", false, EnumDataType.E_NUMBER);
                    }
                    output.UseThis(EnumDataType.E_NUMBER);
                }
                else if (to == EnumDataType.E_BOOL)
                {
                    if (var.IsComputable)
                    {
                        int a = 0;
                        if (Int32.TryParse(var.ValueString.Substring(1, var.ValueString.Length - 2), out a))
                        {
                            TypedVariable.SetVar(output, (a != 0 ? "1" : "0"), true, EnumDataType.E_BOOL);
                        }
                        else
                        {
                            TypedVariable.SetVar(output, "0", true, EnumDataType.E_BOOL);
                        }
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "((" + var.ValueString + ") -eq '0' ? 0 : 1)", false, EnumDataType.E_BOOL);
                    }
                    output.UseThis(EnumDataType.E_BOOL);
                }
                else if (to == EnumDataType.E_STRING)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "\"" + var.ValueString.Substring(1, var.ValueString.Length - 2) + "\"", true, EnumDataType.E_STRING);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "(" + var.ValueString + ").ToString()", false, EnumDataType.E_STRING);
                    }
                    output.UseThis(EnumDataType.E_STRING);
                }
                else if (to == EnumDataType.E_STRING_OBJECT)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "\"" + var.ValueString.Substring(1, var.ValueString.Length - 2) + "\"", true, EnumDataType.E_STRING_OBJECT);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "(" + var.ValueString + ").ToString()", false, EnumDataType.E_STRING_OBJECT);
                    }
                    output.UseThis(EnumDataType.E_STRING_OBJECT);
                }
                else if (to == EnumDataType.E_WRITER)
                {
                    throw new InvalidCastException("Impossible de convertir le caractère '" + var.Name + "' en un objet écrivain");
                }
            }
            else if (from == EnumDataType.E_CONST_STRING_OBJECT)
            {
                if (to == EnumDataType.E_BOOL)
                {
                    if (var.IsComputable)
                    {
                        if (var.ValueInt != 0)
                        {
                            TypedVariable.SetVar(output, "1", true, EnumDataType.E_BOOL);
                        }
                        else
                        {
                            TypedVariable.SetVar(output, "0", true, EnumDataType.E_BOOL);
                        }
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "[Convert]::ToBoolean(" + var.ValueString + ")", false, EnumDataType.E_BOOL);
                    }
                    output.UseThis(EnumDataType.E_BOOL);
                }
                else if (to == EnumDataType.E_WCHAR)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "'" + var.ValueString[0].ToString() + "'", true, EnumDataType.E_WCHAR);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "(" + var.ValueString + ")[0]", false, EnumDataType.E_WCHAR);
                    }
                    output.UseThis(EnumDataType.E_WCHAR);
                }
                else if (to == EnumDataType.E_NUMBER)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, var.ValueInt.ToString(), true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "[Convert]::ToInt32(" + var.ValueString + ")", false, EnumDataType.E_NUMBER);
                    }
                    output.UseThis(EnumDataType.E_NUMBER);
                }
                else if (to == EnumDataType.E_STRING)
                {
                    TypedVariable.SetVar(output, var.ValueString, output.IsComputable, EnumDataType.E_STRING);
                    output.UseThis(EnumDataType.E_STRING);
                }
                else if (to == EnumDataType.E_STRING_OBJECT)
                {
                    TypedVariable.SetVar(output, var.ValueString, output.IsComputable, EnumDataType.E_STRING_OBJECT);
                    output.UseThis(EnumDataType.E_STRING_OBJECT);
                }
                else if (to == EnumDataType.E_WRITER)
                {
                    throw new InvalidCastException("Impossible de convertir la chaîne '" + var.Name + "' en un objet écrivain");
                }
            }
            else if (from == EnumDataType.E_WRITER)
            {
                throw new InvalidCastException("Impossible de convertir l'objet écrivain '" + var.Name + "'");
            }
            return output;
        }

        /// <summary>
        /// Converts a data type to an another and
        /// performs the correct transform code in CPP
        /// </summary>
        /// <param name="converter">current converter object</param>
        /// <param name="from">the current data type</param>
        /// <param name="to">the new data type</param>
        /// <param name="var">variable object</param>
        /// <returns>a new variable object - with the same variable name and a different prefix but doesn't reside in a scope</returns>
        public static IDataNotInScope ConvertToCPP(ICodeConverter converter, EnumDataType from, EnumDataType to, IDataNotInScope var)
        {
            Variable output = new Variable(var.BelongsTo, var.IsComputable, var.DataType);
            output.Value = var.ValueString;
            output.Name = var.Name;
            if (from == to || to == EnumDataType.E_ANY) { return output; }
            else if (from == EnumDataType.E_BOOL)
            {
                if (to == EnumDataType.E_NUMBER)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, var.ValueInt != 0 ? "1" : "0", true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "(int)(" + var.ValueString + ")", false, EnumDataType.E_NUMBER);
                    }
                    output.UseThis(EnumDataType.E_NUMBER);
                }
                else if (to == EnumDataType.E_WCHAR)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "L'" + (var.ValueInt != 0 ? "1" : "0") + "'", true, EnumDataType.E_WCHAR);
                    }
                    else
                    {
                        string localName = TypedVariable.MakeNewVariable(converter);
                        converter.CurrentFunction.AdditionalSource += "wstringstream " + localName + ";" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += localName + " << (" + var.ValueString + ");" + Environment.NewLine;
                        TypedVariable.SetVar(output, "wstring2wch(" + localName + ".str())", false, EnumDataType.E_WCHAR);
                    }
                    output.UseThis(EnumDataType.E_WCHAR);
                }
                else if (to == EnumDataType.E_STRING)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "L\"" + var.ValueString + "\"", true, EnumDataType.E_STRING);
                    }
                    else
                    {
                        string localName = TypedVariable.MakeNewVariable(converter);
                        converter.CurrentFunction.AdditionalSource += "wstringstream " + localName + ";" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += localName + " << (" + var.ValueString + ");" + Environment.NewLine;
                        TypedVariable.SetVar(output, localName + ".str().c_str()", false, EnumDataType.E_STRING);
                    }
                    output.UseThis(EnumDataType.E_STRING);
                }
                else if (to == EnumDataType.E_STRING_OBJECT)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "wstring(L\"" + var.ValueString + "\")", true, EnumDataType.E_STRING_OBJECT);
                    }
                    else
                    {
                        string localName = TypedVariable.MakeNewVariable(converter);
                        converter.CurrentFunction.AdditionalSource += "wstringstream " + localName + ";" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += localName + " << (" + var.ValueString + ");" + Environment.NewLine;
                        TypedVariable.SetVar(output, localName + ".str()", false, EnumDataType.E_STRING_OBJECT);
                    }
                    output.UseThis(EnumDataType.E_STRING_OBJECT);
                }
                else if (to == EnumDataType.E_SIMPLETYPE)
                {
                    TypedVariable.SetVar(output, "SimpleType($[byvalue:" + MicrosoftCPPConverter.Escape(var.PrefixedName) + "])", false, EnumDataType.E_SIMPLETYPE);
                    output.UseThis(EnumDataType.E_SIMPLETYPE);
                }
                else if (to == EnumDataType.E_WRITER)
                {
                    throw new InvalidCastException("Impossible de convertir le booléen '" + var.PrefixedName + "' en objet écrivain");
                }
            }
            else if (from == EnumDataType.E_NUMBER)
            {
                if (to == EnumDataType.E_BOOL)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, (var.ValueInt != 0) ? "1" : "0", true, EnumDataType.E_BOOL);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "((" + var.ValueString + ")==0 ? 0 : 1)", false, EnumDataType.E_BOOL);
                    }
                    output.UseThis(EnumDataType.E_BOOL);
                }
                else if (to == EnumDataType.E_WCHAR)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "L'" + var.ValueString[0] + "'", true, EnumDataType.E_WCHAR);
                    }
                    else
                    {
                        string localName = TypedVariable.MakeNewVariable(converter);
                        converter.CurrentFunction.AdditionalSource += "wstringstream " + localName + ";" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += localName + "<< (" + var.ValueString + ");" + Environment.NewLine;
                        TypedVariable.SetVar(output, "wstring2wch(" + localName + ".str())", false, EnumDataType.E_WCHAR);
                    }
                    output.UseThis(EnumDataType.E_WCHAR);
                }
                else if (to == EnumDataType.E_STRING)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "L\"" + var.ValueString + "\"", true, EnumDataType.E_STRING);
                    }
                    else
                    {
                        string localName = TypedVariable.MakeNewVariable(converter);
                        converter.CurrentFunction.AdditionalSource += "wstringstream " + localName + ";" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += localName + "<< (" + var.ValueString + ");" + Environment.NewLine;
                        TypedVariable.SetVar(output, localName + ".str().c_str()", false, EnumDataType.E_STRING);
                    }
                    output.UseThis(EnumDataType.E_STRING);
                }
                else if (to == EnumDataType.E_STRING_OBJECT)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "wstring(L\"" + var.ValueString + "\")", true, EnumDataType.E_STRING_OBJECT);
                    }
                    else
                    {
                        string localName = TypedVariable.MakeNewVariable(converter);
                        converter.CurrentFunction.AdditionalSource += "wstringstream " + localName + ";" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += localName + "<< (" + var.ValueString + ");" + Environment.NewLine;
                        TypedVariable.SetVar(output, localName + ".str()", false, EnumDataType.E_STRING_OBJECT);
                    }
                    output.UseThis(EnumDataType.E_STRING_OBJECT);
                }
                else if (to == EnumDataType.E_SIMPLETYPE)
                {
                    TypedVariable.SetVar(output, "SimpleType($[byvalue:" + MicrosoftCPPConverter.Escape(var.PrefixedName) + "])", false, EnumDataType.E_SIMPLETYPE);
                    output.UseThis(EnumDataType.E_SIMPLETYPE);
                }
                else if (to == EnumDataType.E_WRITER)
                {
                    throw new InvalidCastException("Impossible de convertir le nombre '" + var.PrefixedName + "' en objet écrivain");
                }
            }
            else if (from == EnumDataType.E_STRING)
            {
                if (to == EnumDataType.E_BOOL)
                {
                    if (var.IsComputable)
                    {
                        if (var.ValueInt != 0)
                        {
                            TypedVariable.SetVar(output, "1", true, EnumDataType.E_BOOL);
                        }
                        else
                        {
                            TypedVariable.SetVar(output, "0", true, EnumDataType.E_BOOL);
                        }
                    }
                    else
                    {
                        string localName = TypedVariable.MakeNewVariable(converter);
                        converter.CurrentFunction.AdditionalSource += "wstringstream " + localName + ";" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += localName + ".str(" + var.ValueString + ");" + Environment.NewLine;
                        string localName2 = TypedVariable.MakeNewVariable(converter);
                        converter.CurrentFunction.AdditionalSource += "bool " + localName2 + ";" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += localName + " >> " + localName2 + ";" + Environment.NewLine;
                        TypedVariable.SetVar(output, localName2, false, EnumDataType.E_BOOL);
                    }
                    output.UseThis(EnumDataType.E_BOOL);
                }
                else if (to == EnumDataType.E_WCHAR)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "L'" + var.ValueString[0].ToString() + "'", true, EnumDataType.E_WCHAR);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "wstring2wch(" + var.ValueString + ")", false, EnumDataType.E_WCHAR);
                    }
                    output.UseThis(EnumDataType.E_WCHAR);
                }
                else if (to == EnumDataType.E_NUMBER)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, var.ValueInt.ToString(), true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        string localName = TypedVariable.MakeNewVariable(converter);
                        converter.CurrentFunction.AdditionalSource += "wstringstream " + localName + ";" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += localName + ".str(" + var.ValueString + ");" + Environment.NewLine;
                        string localName2 = TypedVariable.MakeNewVariable(converter);
                        converter.CurrentFunction.AdditionalSource += "int " + localName2 + ";" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += localName + " >> " + localName2 + ";" + Environment.NewLine;
                        TypedVariable.SetVar(output, localName2, false, EnumDataType.E_NUMBER);
                    }
                    output.UseThis(EnumDataType.E_NUMBER);
                }
                else if (to == EnumDataType.E_STRING_OBJECT)
                {
                    TypedVariable.SetVar(output, "wstring(" + var.ValueString + ")", false, EnumDataType.E_STRING_OBJECT);
                    output.UseThis(EnumDataType.E_STRING_OBJECT);
                }
                else if (to == EnumDataType.E_SIMPLETYPE)
                {
                    TypedVariable.SetVar(output, "SimpleType($[" + MicrosoftCPPConverter.Escape(var.PrefixedName) + "])", false, EnumDataType.E_SIMPLETYPE);
                    output.UseThis(EnumDataType.E_SIMPLETYPE);
                }
                else if (to == EnumDataType.E_WRITER)
                {
                    throw new InvalidCastException("Impossible de convertir la chaîne '" + var.PrefixedName + "' en objet écrivain");
                }
            }
            else if (from == EnumDataType.E_STRING_OBJECT)
            {
                if (to == EnumDataType.E_BOOL)
                {
                    TypedVariable.SetVar(output, "wstring2b(" + var.ValueString + ")", false, EnumDataType.E_BOOL);
                    output.UseThis(EnumDataType.E_BOOL);
                }
                else if (to == EnumDataType.E_WCHAR)
                {
                    TypedVariable.SetVar(output, "wstring2wch(" + var.ValueString + ")", false, EnumDataType.E_WCHAR);
                    output.UseThis(EnumDataType.E_WCHAR);
                }
                else if (to == EnumDataType.E_NUMBER)
                {
                    TypedVariable.SetVar(output, "wstring2i(" + var.ValueString + ")", false, EnumDataType.E_NUMBER);
                    output.UseThis(EnumDataType.E_NUMBER);
                }
                else if (to == EnumDataType.E_STRING)
                {
                    TypedVariable.SetVar(output, var.ValueString + ".c_str()", false, EnumDataType.E_STRING);
                    output.UseThis(EnumDataType.E_STRING);
                }
                else if (to == EnumDataType.E_SIMPLETYPE)
                {
                    TypedVariable.SetVar(output, "SimpleType($[byvalue:" + MicrosoftCPPConverter.Escape(var.PrefixedName) + "])", false, EnumDataType.E_SIMPLETYPE);
                    output.UseThis(EnumDataType.E_SIMPLETYPE);
                }
                else if (to == EnumDataType.E_WRITER)
                {
                    throw new InvalidCastException("Impossible de convertir la chaîne '" + var.PrefixedName + "' en objet écrivain");
                }
            }
            else if (from == EnumDataType.E_WCHAR)
            {
                if (to == EnumDataType.E_NUMBER)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, var.ValueString.Substring(2, var.ValueString.Length - 3), true, EnumDataType.E_NUMBER);
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "(int)(" + var.ValueString + ")", false, EnumDataType.E_NUMBER);
                    }
                    output.UseThis(EnumDataType.E_NUMBER);
                }
                else if (to == EnumDataType.E_BOOL)
                {
                    if (var.IsComputable)
                    {
                        int a = 0;
                        if (Int32.TryParse(var.ValueString.Substring(2, var.ValueString.Length - 3), out a))
                        {
                            TypedVariable.SetVar(output, (a != 0 ? "1" : "0"), true, EnumDataType.E_BOOL);
                        }
                        else
                        {
                            TypedVariable.SetVar(output, "0", true, EnumDataType.E_BOOL);
                        }
                    }
                    else
                    {
                        TypedVariable.SetVar(output, "((" + var.ValueString + ") == L'0' ? 0 : 1)", false, EnumDataType.E_BOOL);
                    }
                    output.UseThis(EnumDataType.E_BOOL);
                }
                else if (to == EnumDataType.E_STRING)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "L\"" + var.ValueString.Substring(2, var.ValueString.Length - 3) + "\"", true, EnumDataType.E_STRING);
                    }
                    else
                    {
                        string localName = TypedVariable.MakeNewVariable(converter);
                        converter.CurrentFunction.AdditionalSource += "wstringstream " + localName + ";" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += localName + " << (" + var.ValueString + ");" + Environment.NewLine;
                        TypedVariable.SetVar(output, localName + ".str().c_str()", false, EnumDataType.E_STRING);
                    }
                    output.UseThis(EnumDataType.E_STRING);
                }
                else if (to == EnumDataType.E_STRING_OBJECT)
                {
                    if (var.IsComputable)
                    {
                        TypedVariable.SetVar(output, "wstring(L\"" + var.ValueString.Substring(2, var.ValueString.Length - 3) + "\")", true, EnumDataType.E_STRING_OBJECT);
                    }
                    else
                    {
                        string localName = TypedVariable.MakeNewVariable(converter);
                        converter.CurrentFunction.AdditionalSource += "wstringstream " + localName + ";" + Environment.NewLine;
                        converter.CurrentFunction.AdditionalSource += localName + " << (" + var.ValueString + ");" + Environment.NewLine;
                        TypedVariable.SetVar(output, localName + ".str()", false, EnumDataType.E_STRING_OBJECT);
                    }
                    output.UseThis(EnumDataType.E_STRING_OBJECT);
                }
                else if (to == EnumDataType.E_SIMPLETYPE)
                {
                    TypedVariable.SetVar(output, "SimpleType($[byvalue:" + MicrosoftCPPConverter.Escape(var.PrefixedName) + "])", false, EnumDataType.E_SIMPLETYPE);
                    output.UseThis(EnumDataType.E_SIMPLETYPE);
                }
                else if (to == EnumDataType.E_WRITER)
                {
                    throw new InvalidCastException("Impossible de convertir la chaîne '" + var.PrefixedName + "' en objet écrivain");
                }
            }
            else if (from == EnumDataType.E_CONST_STRING_OBJECT)
            {
                if (to == EnumDataType.E_BOOL)
                {
                    TypedVariable.SetVar(output, "wstring2b(" + var.ValueString + ")", false, EnumDataType.E_BOOL);
                    output.UseThis(EnumDataType.E_BOOL);
                }
                else if (to == EnumDataType.E_WCHAR)
                {
                    TypedVariable.SetVar(output, "wstring2wch(" + var.ValueString + ")", false, EnumDataType.E_WCHAR);
                    output.UseThis(EnumDataType.E_WCHAR);
                }
                else if (to == EnumDataType.E_NUMBER)
                {
                    TypedVariable.SetVar(output, "wstring2i(" + var.ValueString + ")", false, EnumDataType.E_NUMBER);
                    output.UseThis(EnumDataType.E_NUMBER);
                }
                else if (to == EnumDataType.E_STRING)
                {
                    TypedVariable.SetVar(output, var.ValueString + ".c_str()", false, EnumDataType.E_STRING);
                    output.UseThis(EnumDataType.E_STRING);
                }
                else if (to == EnumDataType.E_STRING_OBJECT)
                {
                    TypedVariable.SetVar(output, "wstring(" + var.ValueString + ")", false, EnumDataType.E_STRING_OBJECT);
                    output.UseThis(EnumDataType.E_STRING_OBJECT);
                }
                else if (to == EnumDataType.E_SIMPLETYPE)
                {
                    TypedVariable.SetVar(output, "SimpleType($[" + MicrosoftCPPConverter.Escape(var.PrefixedName) + "])", false, EnumDataType.E_SIMPLETYPE);
                    output.UseThis(EnumDataType.E_SIMPLETYPE);
                }
            }
            else if (from == EnumDataType.E_SIMPLETYPE)
            {
                if (to == EnumDataType.E_BOOL)
                {
                    TypedVariable.SetVar(output, var.PrefixedName + ".getBOOL()", false, EnumDataType.E_BOOL);
                    output.UseThis(EnumDataType.E_BOOL);
                }
                else if (to == EnumDataType.E_NUMBER)
                {
                    TypedVariable.SetVar(output, var.PrefixedName + ".getNUMBER()", false, EnumDataType.E_NUMBER);
                    output.UseThis(EnumDataType.E_NUMBER);
                }
                else if (to == EnumDataType.E_STRING)
                {
                    TypedVariable.SetVar(output, var.PrefixedName + ".getSTRING()", false, EnumDataType.E_STRING);
                    output.UseThis(EnumDataType.E_STRING);
                }
                else if (to == EnumDataType.E_STRING_OBJECT)
                {
                    TypedVariable.SetVar(output, var.PrefixedName + ".getWSTRING()", false, EnumDataType.E_STRING_OBJECT);
                    output.UseThis(EnumDataType.E_STRING_OBJECT);
                }
                else if (to == EnumDataType.E_CONST_STRING_OBJECT)
                {
                    TypedVariable.SetVar(output, var.PrefixedName + ".getCWSTRING()", false, EnumDataType.E_CONST_STRING_OBJECT);
                    output.UseThis(EnumDataType.E_CONST_STRING_OBJECT);
                }
                else if (to == EnumDataType.E_WCHAR)
                {
                    TypedVariable.SetVar(output, var.PrefixedName + ".getWCHAR()", false, EnumDataType.E_WCHAR);
                    output.UseThis(EnumDataType.E_WCHAR);
                }
                else if (to == EnumDataType.E_WRITER)
                {
                    throw new InvalidCastException("Impossible de convertir l'objet '" + var.PrefixedName + "' en objet écrivain");
                }
            }
            else if (from == EnumDataType.E_WRITER)
            {
                throw new InvalidCastException("Impossible de convertir l'objet écrivain '" + var.Name + "'");
            }
            return output;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Move the type
        /// </summary>
        /// <param name="converter">current converter</param>
        /// <param name="scope">current scope</param>
        /// <returns>the converted expression</returns>
        public string MoveType(ICodeConverter converter, IScope scope)
        {
            if (scope.Exists(this.varName))
            {
                IData d = scope.GetVariable(this.varName);
                try
                {
                    if (!d.DataType.Equals(this.ToType))
                    {
                        this.var = new Variable("", true);
                        this.var.CopyFrom(d, false);
                        converter.CurrentFunction.AdditionalSource = "";
                        converter.Convert(this);
                        if (d.TypeExists(this.changedVar.DataType))
                        {
                            // a variable in the scope has no specific type ; it's just a string
                            // each time i want the value, i have to take a logical conversion with that string
                            d.Set(d.PrefixInfo(this.changedVar.DataType).Prefix,
                                  converter.ProcessAsFunction,
                                  d.ValueString,
                                  d.IsComputable,
                                  this.changedVar.DataType);
                            // this string contains the real transformation to the specific converted language
                            this.convertedValue = converter.CurrentFunction.CacheSource;
                        }
                        else
                        {
                            d.Set(Scope.StandardPrefix(this.changedVar.DataType),
                                  d.BelongsTo,
                                  d.ValueString,
                                  d.IsComputable,
                                  this.changedVar.DataType);
                            // this string contains the real transformation to the specific converted language
                            this.convertedValue = converter.CurrentFunction.CacheSource;
                            IStructure st = converter.CreateNewField(converter.RootStructureInstance, d, false);
                            converter.CurrentFunction.LocalVariables.Add(st);
                        }
                        d.UseThis(this.changedVar.DataType);
                    }
                    return this.convertedValue;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                throw new ArgumentException("La variable '" + this.varName + "' n'existe pas");
            }
        }

        /// <summary>
        /// Type change
        /// </summary>
        /// <param name="converter">current converter</param>
        /// <param name="scope">current scope</param>
        /// <returns>a new variable object (doesn't reside in scope)</returns>
        public IDataNotInScope ChangeType(ICodeConverter converter, IScope scope)
        {
            if (scope.Exists(this.varName))
            {
                IData d = scope.GetVariable(this.varName);
                try
                {
                    this.var = new Variable("", true);
                    this.var.CopyFrom(d, false);
                    if (!d.DataType.Equals(this.ToType))
                    {
                        converter.CurrentFunction.AdditionalSource = "";
                        converter.Convert(this);
                        return this.changedVar;
                    }
                    else
                    {
                        return this.var;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                throw new ArgumentException("La variable '" + this.varName + "' n'existe pas");
            }
        }
        #endregion

        #region ILanguageConverter Membres

        /// <summary>
        /// VBScript call
        /// </summary>
        /// <param name="converter">current converter</param>
        public void WriteInVBScript(ICodeConverter converter)
        {
            this.changedVar = TypedVariable.ConvertToVBScript(converter, this.var.DataType, this.ToType, this.var);
            converter.CurrentFunction.DataTypeResult = this.changedVar.DataType;
            converter.CurrentFunction.AddToSource(converter.CurrentFunction.AdditionalSource);
            converter.CurrentFunction.AddToSource(this.changedVar.Name + " = " + this.convertedValue + Environment.NewLine);
        }

        /// <summary>
        /// PowerShell call
        /// </summary>
        /// <param name="converter">current converter</param>
        public void WriteInPowerShell(ICodeConverter converter)
        {
            this.changedVar = TypedVariable.ConvertToPowerShell(converter, this.var.DataType, this.ToType, this.var);
            this.convertedValue = this.changedVar.ValueString;
            converter.CurrentFunction.DataTypeResult = this.changedVar.DataType;
            converter.CurrentFunction.AddToSource(converter.CurrentFunction.AdditionalSource);
            converter.CurrentFunction.AddToSource("$" + PowerShellConverter.Escape(this.changedVar.Name) + " = " + this.convertedValue + Environment.NewLine);
        }

        /// <summary>
        /// Perl call
        /// </summary>
        /// <param name="converter">current converter</param>
        public void WriteInPerl(ICodeConverter converter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Python call
        /// </summary>
        /// <param name="converter">current converter</param>
        public void WriteInPython(ICodeConverter converter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Microsoft CPP call
        /// </summary>
        /// <param name="converter">current converter</param>
        public void WriteInMicrosoftCPP(ICodeConverter converter)
        {
            this.changedVar = TypedVariable.ConvertToCPP(converter, this.var.DataType, this.ToType, this.var);
            this.convertedValue = this.changedVar.ValueString;
            converter.CurrentFunction.DataTypeResult = this.changedVar.DataType;
            converter.CurrentFunction.AddToSource(converter.CurrentFunction.AdditionalSource);
            converter.CurrentFunction.AddToSource("$[" + MicrosoftCPPConverter.Escape(this.changedVar.PrefixedName) + "] = " + this.convertedValue + ";" + Environment.NewLine);
        }

        /// <summary>
        /// Unix CPP call
        /// </summary>
        /// <param name="converter">current converter</param>
        public void WriteInUnixCPP(ICodeConverter converter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Microsoft CSharp call
        /// </summary>
        /// <param name="converter">current converter</param>
        public void WriteInCSharp(ICodeConverter converter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Java call
        /// </summary>
        /// <param name="converter">current converter</param>
        public void WriteInJava(ICodeConverter converter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Mac OS CPP call
        /// </summary>
        /// <param name="converter">current converter</param>
        public void WriteInMacOSCPP(ICodeConverter converter)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
