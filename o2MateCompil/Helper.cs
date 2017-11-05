using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Converters;

namespace o2Mate
{
    [System.Runtime.InteropServices.ComVisible(false)]
    internal static class Helper
    {
        #region Public Delegates
        public delegate void IsolatedFunctionDelegate();
        #endregion

        #region Private Static Methods
        /// <summary>
        /// Dans le cas où la variable n'est pas ou plus calculable
        /// Enregistre une expression dans une variable incalculable permettant d'utiliser l'expression
        /// dans la conversion d'une variable assignée dont le type est défini par l'inférence de l'expression
        /// </summary>
        /// <param name="comp">compilateur</param>
        /// <param name="proc">process</param>
        /// <param name="converter">langage de conversion</param>
        /// <param name="expression">expression</param>
        /// <param name="varName">variable</param>
        /// <returns>l'objet variable utilisé</returns>
        private static IData ConvertExpression(ICompilateur comp, IProcessInstance proc, ICodeConverter converter, string expression, string varName)
        {
            EnumDataType fixedDataType = converter.CurrentFunction.DataTypeResult;
            // store the result in the scope with the varName parameter
            if (proc.CurrentScope.Exists(varName))
            {
                // get the variable infos
                IData myVar = proc.CurrentScope.GetVariable(varName);
                // the variable exists in the scope, assumed infer the data type from expression
                // include additional statements for converting expression
                o2Mate.Expression.Convert(converter, expression, proc.CurrentScope, true, fixedDataType, true);
                if (myVar.TypeExists(fixedDataType))
                {
                    proc.CurrentScope.Update(varName,
                                             converter.CurrentFunction.CacheSource,
                                             myVar.PrefixInfo(fixedDataType).BelongsTo, false,
                                             fixedDataType);
                }
                else
                {
                    // le nouveau type de données de cette variable doit appartenir au processus
                    // qui a initialisé la variable
                    proc.CurrentScope.Update(varName,
                                             converter.CurrentFunction.CacheSource,
                                             myVar.BelongsTo, false,
                                             fixedDataType);
                }
                // on met à jour les paramètres en indiquant que cette variable est mutable
                comp.UpdateParameters(converter, proc, varName, true);
                return myVar;
            }
            else
            {
                // convert expression including variable name
                o2Mate.Expression.Convert(converter, expression, proc.CurrentScope, true, fixedDataType, true);
                // create variable, don't use value and assumes to be non-computable and infer data type by the expression
                IData added = proc.CurrentScope.Add(varName,
                                                    converter.CurrentFunction.CacheSource, proc.Name, false,
                                                    fixedDataType);
                Helper.AddIntoLocal(converter, added);
                return added;
            }
        }

        /// <summary>
        /// Infers the expression and changes the data type of the existing variable
        /// </summary>
        /// <param name="comp">compilation</param>
        /// <param name="proc">process</param>
        /// <param name="converter">language converter</param>
        /// <param name="expression">expression</param>
        /// <param name="varName">variable name</param>
        /// <returns>a stored variable with the infered data type by the expression</returns>
        private static IData ComputeExpression(ICompilateur comp, IProcessInstance proc, ICodeConverter converter, string expression, string varName)
        {
            IData converted = null;

            o2Mate.Expression val = new o2Mate.Expression();
            // computes the exact value of the inline expression even using variables but all computable
            IData res = val.Evaluate(expression, proc.CurrentScope);
            // compute calculability
            bool calculability = converter.CurrentFunction.IsStaticControlFlow;

            // store the result in the scope with the varName parameter
            if (proc.CurrentScope.Exists(varName))
            {
                // get the variable infos
                IData myVar = proc.CurrentScope.GetVariable(varName);
                if (res.IsComputable && calculability)
                {
                    if (res.DataType != myVar.DataType)
                    {
                        if (myVar.TypeExists(res.DataType))
                        {
                            // update variable with the new data type, value and computable switch
                            proc.CurrentScope.Update(varName,
                                                     res.ValueString,
                                                     myVar.PrefixInfo(res.DataType).BelongsTo,
                                                     true, res.DataType);
                        }
                        else
                        {
                            // update variable with the new data type, value and computable switch
                            proc.CurrentScope.Update(varName,
                                                     res.ValueString,
                                                     myVar.BelongsTo,
                                                     true, res.DataType);
                        }
                    }
                    else
                    {
                        // the same current data type of the stored variable is infered with the expression
                        myVar.Value = res.ValueString;
                    }
                    // on met à jour les paramètres en indiquant que la variable est mutable
                    comp.UpdateParameters(converter, proc, varName, true);
                    converted = myVar;
                }
                else
                {
                    // à cause de l'état du flux de contrôle (boucles,if,etc), converting expression
                    myVar.IsComputable = false;
                    Expression.Convert(converter, expression, proc.CurrentScope, false, false);
                    converted = Helper.ConvertExpression(comp, proc, converter, expression, varName);
                }
            }
            else
            {
                // comme la variable n'existe pas ou plus, une affectation va la créer
                // de ce fait, si son évaluation est calculable, elle est donc constante
                if (res.IsComputable)
                {
                    converted = proc.CurrentScope.Add(varName, res.ValueString, proc.Name, true, res.DataType);
                }
                else
                {
                    // créer une nouvelle variable dans le scope et l'ajouter aux variables locales
                    converted = proc.CurrentScope.Add(varName,
                                                      "",
                                                      proc.Name, false,
                                                      res.DataType);
                }
                Helper.AddIntoLocal(converter, converted);
            }
            return converted;
        }

        /// <summary>
        /// Converts a newer variable or an existing variable
        /// The variable will be non computable after these changes
        /// </summary>
        /// <param name="proc">process object</param>
        /// <param name="converter">converter object</param>
        /// <param name="varName">name of the variable</param>
        /// <param name="desiredDataType">data type to convert</param>
        /// <returns>the variable (assumes to be present in the scope)</returns>
        private static IData ConvertNonComputableVariableType(IProcessInstance proc, ICodeConverter converter, string varName, EnumDataType desiredDataType)
        {
            // store the result in the scope with the varName parameter
            if (proc.CurrentScope.Exists(varName))
            {
                // get the variable infos
                IData myVar = proc.CurrentScope.GetVariable(varName);
                // specific prefix is used with desired data type
                if (myVar.TypeExists(desiredDataType))
                {
                    // update variable, don't use value and assumes to be non-computable
                    proc.CurrentScope.Update(varName, "0", myVar.PrefixInfo(desiredDataType).BelongsTo, false, desiredDataType);
                }
                else
                {
                    // update variable, don't use value and assumes to be non-computable
                    proc.CurrentScope.Update(varName, "0", myVar.BelongsTo, false, desiredDataType);
                }
                return myVar;
            }
            else
            {
                // create variable, don't use value and assumes to be non-computable
                IData added = proc.CurrentScope.Add(varName, "0", proc.Name, false, desiredDataType);
                Helper.AddIntoLocal(converter, added);
                return added;
            }
        }

        /// <summary>
        /// Assumes the expression to be non-calculable
        /// Convert the expression and infers the data type
        /// </summary>
        /// <param name="comp">compilation</param>
        /// <param name="proc">process</param>
        /// <param name="converter">language converter</param>
        /// <param name="expression">expression</param>
        /// <returns>standalone variable with the result value (do not add into the scope)</returns>
        private static IDataNotInScope ConvertExpression(ICompilateur comp, IProcessInstance proc, ICodeConverter converter, string expression)
        {
            // does add additional statements
            o2Mate.Expression.Convert(converter, expression, proc.CurrentScope, true, true);
            IDataNotInScope res = new Variable("", false, converter.CurrentFunction.DataTypeResult);
            res.Value = converter.CurrentFunction.CacheSource;
            return res;
        }

        /// <summary>
        /// Convert the expression with the desired data type
        /// </summary>
        /// <param name="comp">compilation</param>
        /// <param name="proc">process</param>
        /// <param name="converter">language converter</param>
        /// <param name="expression">expression</param>
        /// <param name="desiredType">desired type conversion</param>
        /// <returns>standalone variable with the result value (do not add into the scope)</returns>
        private static IDataNotInScope ConvertExpression(ICompilateur comp, IProcessInstance proc, ICodeConverter converter, string expression, o2Mate.EnumDataType desiredType)
        {
            // does add additional statements
            o2Mate.Expression.Convert(converter, expression, proc.CurrentScope, true, desiredType, true);
            IDataNotInScope res = new Variable("", false, converter.CurrentFunction.DataTypeResult);
            res.Value = converter.CurrentFunction.CacheSource;
            return res;
        }

        /// <summary>
        /// Assumes to be a calculable expression
        /// Evaluates the expression and returns the value
        /// </summary>
        /// <param name="comp">compilation</param>
        /// <param name="proc">process</param>
        /// <param name="converter">language converter</param>
        /// <param name="expression">expression</param>
        /// <returns>a standalone variable object (do not add into the scope)</returns>
        private static IDataNotInScope ComputeExpression(ICompilateur comp, IProcessInstance proc, ICodeConverter converter, string expression)
        {
            // print expression with all variables and additional source
            o2Mate.Expression expr = new o2Mate.Expression();
            IDataNotInScope val = expr.Evaluate(expression, proc.CurrentScope);
            return val;
        }

        private static string ReplaceStringParameter(UseTemplate ut, string parameterName)
        {
            string value = ut.Parameters[parameterName];
            value = value.Replace("&lt;", "<");
            value = value.Replace("&gt;", ">");
            value = value.Replace("&amp;", "&");
            return value;
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Convert a newer variable or an existing variable.
        /// Keeps the current value of this variable.
        /// </summary>
        /// <param name="proc">process object</param>
        /// <param name="converter">converter object</param>
        /// <param name="varName">name of the variable</param>
        /// <param name="desiredDataType">data type to convert</param>
        /// <returns>the conversion string result</returns>
        public static string ConvertVariableType(IProcessInstance proc, ICodeConverter converter, string varName, EnumDataType desiredDataType)
        {
            // store the result in the scope with the varName parameter
            if (proc.CurrentScope.Exists(varName))
            {
                string result = String.Empty;
                // get the variable infos
                IData myVar = proc.CurrentScope.GetVariable(varName);
                // specific prefix is used with desired data type
                if (myVar.TypeExists(desiredDataType))
                {
                    // if the desired data type is not the current type
                    if (myVar.DataType != desiredDataType)
                    {
                        TypedVariable tv = new TypedVariable(myVar.Name, myVar.Prefix, myVar.DataType, desiredDataType);
                        result = tv.MoveType(converter, proc.CurrentScope);
                    }
                }
                else
                {
                    TypedVariable tv = new TypedVariable(myVar.Name, myVar.Prefix, myVar.DataType, desiredDataType);
                    result = tv.MoveType(converter, proc.CurrentScope);
                }
                return result;
            }
            else
            {
                throw new ArgumentException("La variable '" + varName + "' n'existe pas dans le scope");
            }
        }

        /// <summary>
        /// Use this function to assign an existing variable (or not) with a calculable expression or not.
        /// The variable data type is infered by the expression
        /// </summary>
        /// <param name="comp">compilation</param>
        /// <param name="proc">process</param>
        /// <param name="converter">language converter</param>
        /// <param name="expression">expression</param>
        /// <param name="varName">variable name</param>
        /// <returns>a newer variable with its value (stored in the scope)</returns>
        public static IData ConvertNewInferedVariable(ICompilateur comp, IProcessInstance proc, ICodeConverter converter, string expression, string varName)
        {
            // convert inline expression using variables value stored in scope
            // does not add additional statements
            o2Mate.Expression.Convert(converter, expression, proc.CurrentScope, false);

            // switch about calculability (but do not pass by constants)
            if (converter.CurrentFunction.IsComputableExpression && false)
            {
                // the value of the expression can be exactly computed among the execution
                return Helper.ComputeExpression(comp, proc, converter, expression, varName);
            }
            else
            {
                // since variable is not computable, check to all used variable, needed to clarify locals or parameters
                foreach (string used in converter.CurrentFunction.UsedVariables)
                {
                    comp.UpdateParameters(converter, proc, used, false);
                }
                // the value of the expression cannot be exactly computed among the execution
                return Helper.ConvertExpression(comp, proc, converter, expression, varName);
            }
        }

        /// <summary>
        /// Use this function to convert an expression which can be computable or not
        /// Returns a variable object to do not store in the scope
        /// </summary>
        /// <param name="comp">compilation</param>
        /// <param name="proc">process</param>
        /// <param name="converter">language converter</param>
        /// <param name="expression">expression</param>
        /// <returns>a standalone variable with its value (do not store it in the scope)</returns>
        public static IDataNotInScope ConvertNewExpression(ICompilateur comp, IProcessInstance proc, ICodeConverter converter, string expression)
        {
            // convert inline expression using variables stored in scope
            // does not add additional statements
            o2Mate.Expression.Convert(converter, expression, proc.CurrentScope, false);

            // switch about calculability
            if (converter.CurrentFunction.IsComputableExpression)
            {
                // the value of the expression can be exactly computed among the execution
                return Helper.ComputeExpression(comp, proc, converter, expression);
            }
            else
            {
                // since variable is not computable, check to all used variable, needed to clarify locals or parameters
                foreach (string used in converter.CurrentFunction.UsedVariables)
                {
                    comp.UpdateParameters(converter, proc, used, false);
                }
                // the value of the expression cannot be exactly computed among the execution
                return Helper.ConvertExpression(comp, proc, converter, expression);
            }
        }

        /// <summary>
        /// Use this function to convert an expression which can be computable or not
        /// Returns a variable object to do not store in the scope
        /// Supply the type to convert 
        /// </summary>
        /// <param name="comp">compilation</param>
        /// <param name="proc">process</param>
        /// <param name="converter">language converter</param>
        /// <param name="expression">expression</param>
        /// <param name="desiredType">desired data type result</param>
        /// <returns>a string with the converted expression</returns>
        public static string ConvertNewExpression(ICompilateur comp, IProcessInstance proc, ICodeConverter converter, string expression, o2Mate.EnumDataType desiredType)
        {
            // convert inline expression using variables stored in scope
            // does add additional statements
            o2Mate.Expression.Convert(converter, expression, proc.CurrentScope, true, desiredType, true);

            // since variable is used, check to all used variable, needed to clarify locals or parameters
            foreach (string used in converter.CurrentFunction.UsedVariables)
            {
                comp.UpdateParameters(converter, proc, used, false);
            }

            return converter.CurrentFunction.CacheSource;
        }

        /// <summary>
        /// Defines or update a variable object stored in the scope; the variable data type is an object string only (std::wstring in C++)
        /// Because of assigned from a dictionary string, the variable is never computable;
        /// if the variable was computable, it won't be
        /// As the variable value has changed, make a mutable parameter if any
        /// As the variable was not exist in the scope, the variable prefixed name (with a type's prefix) is stored in local
        /// </summary>
        /// <param name="comp">compilation</param>
        /// <param name="proc">process</param>
        /// <param name="converter">language converter</param>
        /// <param name="varName">variable name to be stored in the scope</param>
        /// <returns>an added variable object stored in the scope</returns>
        public static IData ConvertNewDictionaryString(ICompilateur comp, IProcessInstance proc, ICodeConverter converter, string varName)
        {
            // control if variable is in the scope
            if (proc.CurrentScope.Exists(varName))
            {
                // get the variable infos
                IData myVar = proc.CurrentScope.GetVariable(varName);
                // update variable
                if (myVar.TypeExists(EnumDataType.E_STRING_OBJECT))
                {
                    proc.CurrentScope.Update(varName, "",
                                             myVar.PrefixInfo(EnumDataType.E_STRING_OBJECT).BelongsTo,
                                             false, EnumDataType.E_STRING_OBJECT);
                }
                else
                {
                    proc.CurrentScope.Update(varName, "", myVar.BelongsTo, false, EnumDataType.E_STRING_OBJECT);
                }
                // on met à jour les paramètres en indiquant que la variable est mutable
                comp.UpdateParameters(converter, proc, varName, true);
                return myVar;
            }
            else
            {
                // créer une nouvelle variable dans le scope; en déduire qu'elle est locale
                IData added = proc.CurrentScope.Add(varName, "", proc.Name, false, EnumDataType.E_STRING_OBJECT);
                Helper.AddIntoLocal(converter, added);
                return added;
            }
        }

        /// <summary>
        /// Defines or update a variable object stored in the scope; the variable data type is an object string only (std::wstring in C++)
        /// Because of assigned from a dictionary field, the variable is never computable;
        /// if the variable was computable, it won't be
        /// As the variable value has changed, make a mutable parameter if any
        /// As the variable was not exist in the scope, the variable prefixed name (with a type's prefix) is stored in local
        /// Just call ConvertNewDictionaryString
        /// </summary>
        /// <param name="comp">compilation</param>
        /// <param name="proc">process</param>
        /// <param name="converter">language converter</param>
        /// <param name="varName">variable name to be stored in the scope</param>
        /// <returns>an added variable object stored in the scope</returns>
        public static IData ConvertNewDictionaryArray(ICompilateur comp, IProcessInstance proc, ICodeConverter converter, string varName)
        {
            return Helper.ConvertNewDictionaryString(comp, proc, converter, varName);
        }

        /// <summary>
        /// Defines or update a variable object stored in the scope; the variable data type is an integer only
        /// Because of assigned from the size of an array, the variable is never computable;
        /// if the variable was computable, it won't be
        /// As the variable value has changed, make a mutable parameter if any
        /// As the variable was not exist in the scope, the variable prefixed name (with a type's prefix) is stored in local
        /// Identical from ConvertNewDictionaryString except a different data type
        /// </summary>
        /// <param name="comp">compilation</param>
        /// <param name="proc">process</param>
        /// <param name="converter">language converter</param>
        /// <param name="varName">variable name to be stored in the scope</param>
        /// <returns>an added variable object stored in the scope</returns>
        public static IData ConvertNewDictionarySize(ICompilateur comp, IProcessInstance proc, ICodeConverter converter, string varName)
        {
            // control if variable is in the scope
            if (proc.CurrentScope.Exists(varName))
            {
                // get the variable infos
                IData myVar = proc.CurrentScope.GetVariable(varName);
                // update variable
                if (myVar.TypeExists(EnumDataType.E_NUMBER))
                {
                    proc.CurrentScope.Update(varName, "",
                                             myVar.PrefixInfo(EnumDataType.E_NUMBER).BelongsTo,
                                             false, EnumDataType.E_NUMBER);
                }
                else
                {
                    proc.CurrentScope.Update(varName, "", myVar.BelongsTo, false, EnumDataType.E_NUMBER);
                }
                // on met à jour les paramètres en indiquant que la variable est mutable
                comp.UpdateParameters(converter, proc, varName, true);
                return myVar;
            }
            else
            {
                // créer une nouvelle variable dans le scope; en déduire qu'elle est locale
                IData added = proc.CurrentScope.Add(varName, "", proc.Name, false, EnumDataType.E_NUMBER);
                Helper.AddIntoLocal(converter, added);
                return added;
            }
        }

        /// <summary>
        /// Verify if a variable already exists in locals
        /// </summary>
        /// <param name="converter">language converter</param>
        /// <param name="var">variable object</param>
        /// <returns>true if is local</returns>
        public static bool IsLocal(ICodeConverter converter, IData var)
        {
            // chercher la fonction en cours
            return converter.CurrentFunction.LocalVariables.Exists(new Predicate<IStructure>(delegate(IStructure st)
            {
                return (converter.IsStronglyTyped && st.PrefixedFieldName == var.PrefixedName) || (!converter.IsStronglyTyped && st.FieldName == var.Name);
            }));
        }

        /// <summary>
        /// Verify if a variable already exists in locals
        /// </summary>
        /// <param name="converter">language converter</param>
        /// <param name="f">funcion</param>
        /// <param name="var">variable object</param>
        /// <returns>true if is local</returns>
        public static bool IsLocal(ICodeConverter converter, IFunction f, IData var)
        {
            return f.LocalVariables.Exists(new Predicate<IStructure>(delegate(IStructure st)
            {
                return (converter.IsStronglyTyped && st.PrefixedFieldName == var.PrefixedName) || (!converter.IsStronglyTyped && st.FieldName == var.Name);
            }));
        }

        /// <summary>
        /// Fill a new method
        /// </summary>
        /// <param name="comp">compilation</param>
        /// <param name="subProc">sub process object</param>
        /// <param name="converter">language converter</param>
        /// <param name="f">new function</param>
        /// <param name="file">writing in</param>
        public static void FillNewMethod(ICompilateurInstance comp, IProcess subProc, ICodeConverter converter, IFunction f, FinalFile file)
        {
            // la fonction est implémentée
            converter.PushFunction(converter.CurrentFunction);
            f.PropagateControlFlow(converter.CurrentFunction);
            converter.SetCurrentFunction(f);
            subProc.FunctionName = converter.ProcessAsFunction;
            converter.CurrentFunction.ForwardControlFlowSub();
            comp.Convert(converter, subProc, file);
            converter.CurrentFunction.BackwardControlFlowSub();
            converter.SetCurrentFunction(converter.PopFunction());
        }

        /// <summary>
        /// Clear a function ; using it for rewriting a function to prevent changes of calling functions
        /// </summary>
        /// <param name="f">function</param>
        public static void ClearMethod(IFunction f)
        {
            f.Clear();
        }

        /// <summary>
        /// Verify if the function need to be renewed
        /// </summary>
        /// <param name="converter">language converter</param>
        /// <param name="curProc">process</param>
        /// <param name="newFunc">new function</param>
        /// <returns>true or false</returns>
        public static bool CheckNewMethod(ICodeConverter converter, IProcess curProc, IFunction newFunc)
        {
            return curProc.HasChanged || !converter.ImplementedFunctions.Exists(new Predicate<IFunction>(delegate(IFunction func)
            {
                return newFunc.Equals(func) && newFunc.InstanceNumber != func.InstanceNumber;
            }));
        }

        /// <summary>
        /// Special function for CPP conversion
        /// It's needed to confirm for each data type of formal parameters to be the same than the effective parameters
        /// if not, variable have to be converted in the good data type
        /// </summary>
        /// <param name="proc">process</param>
        /// <param name="converter">language converter instance</param>
        /// <param name="newFunc">new function</param>
        /// <returns>sequence call statement in CPP</returns>
        public static string MakeNewMethodForCPP(IProcess proc, ICodeConverter converter, IFunction newFunc)
        {
            string callStatement = String.Empty;
            callStatement = MicrosoftCPPConverter.Escape((newFunc.IsMacro ? "macro_" : newFunc.IsJob ? "job_" : "func_") + newFunc.Name + "(");
            bool first = true;
            foreach (IParameter p in newFunc.Parameters)
            {
                if (!first) { callStatement += ", "; } else { first = false; }
                if (p.IsMutableParameter)
                    callStatement += "$[byref:" + p.EffectiveParameter + "]";
                else
                    callStatement += "$[byvalue:" + p.EffectiveParameter + "]";
            }
            callStatement += ");" + Environment.NewLine;
            return callStatement;
        }

        /// <summary>
        /// Verify if the function need to be renewed
        /// </summary>
        /// <param name="converter">language converter</param>
        /// <param name="proc">process object</param>
        /// <param name="funName">function name</param>
        /// <returns>true or false</returns>
        public static bool NeedNewMethod(ICodeConverter converter, IProcess proc, string funName)
        {
            return proc.HasChanged ||
                !converter.ImplementedFunctions.Exists(new Predicate<IFunction>(delegate(IFunction func)
                {
                    return func.StrictName == funName;
                }));
        }

        /// <summary>
        /// Make a new function with the incremented instance number
        /// </summary>
        /// <param name="converter">language converter</param>
        /// <param name="proc">process object</param>
        /// <returns>a new function</returns>
        public static IFunction MakeNewMethod(ICodeConverter converter, IProcess proc)
        {
            IFunction f;
            string funName = Process.ProcessAsFunction(proc.Name);
            if (Helper.NeedNewMethod(converter, proc, funName))
            {
                f = new Function();
                f.StrictName = funName;
                f.InstanceNumber = converter.ImplementedFunctions.FindAll(new Predicate<IFunction>(delegate(IFunction func)
                {
                    return func.StrictName == funName;
                })).Count;
                converter.ImplementedFunctions.Add(f);
            }
            else
            {
                f = converter.ImplementedFunctions.FindLast(new Predicate<IFunction>(delegate(IFunction func)
                {
                    return func.StrictName == funName;
                }));
                // je recrée la fonction
                Helper.ClearMethod(f);
            }
            return f;
        }

        /// <summary>
        /// Add into the current local variable list (if not exists)
        /// </summary>
        /// <param name="converter">language converter</param>
        /// <param name="var">variable object</param>
        public static void AddIntoLocal(ICodeConverter converter, IData var)
        {
            if (!Helper.IsLocal(converter, var))
            {
                IStructure st = converter.CreateNewField(converter.RootStructureInstance, var, false);
                converter.CurrentFunction.LocalVariables.Add(st);
            }
        }

        /// <summary>
        /// Add into the current local variable list (if not exists)
        /// </summary>
        /// <param name="converter">language converter</param>
        /// <param name="f">function</param>
        /// <param name="var">variable object</param>
        public static void AddIntoLocal(ICodeConverter converter, IFunction f, IData var)
        {
            if (!Helper.IsLocal(converter, f, var))
            {
                IStructure st = converter.CreateNewField(converter.RootStructureInstance, var, false);
                f.LocalVariables.Add(st);
            }
        }

        /// <summary>
        /// Create a dependant variable : it's a stored variable in a specific use template (implemented for a specific language)
        /// the variable has a data type fixed. If it's not the same, the variable value is converted into the good data type
        /// if needed, this variable makes a mutable parameter
        /// </summary>
        /// <param name="comp">compiler object</param>
        /// <param name="converter">language converter</param>
        /// <param name="proc">process</param>
        /// <param name="ut">UseTemplate object</param>
        /// <param name="valueName">name of the variable</param>
        /// <param name="desiredDataType">fixed data type</param>
        /// <returns>the name of the variable (it comes to non computable)</returns>
        public static string CreateDependantVariable(ICompilateur comp, ICodeConverter converter, IProcessInstance proc, UseTemplate ut, string valueName, EnumDataType desiredDataType)
        {
            string var = valueName;
            if (ut.Parameters.ContainsKey(valueName)) var = Helper.ReplaceStringParameter(ut, valueName);
            // important : la variable n'est plus calculable
            IData newVar = Helper.ConvertNonComputableVariableType(proc, converter, var, desiredDataType);
            comp.UpdateParameters(converter, proc, newVar.Name, true);
            return converter.ReturnVarName(newVar);
        }

        /// <summary>
        /// Create an expression : expression is written for a specific use template (implemented for a specific language)
        /// the expression is converted if the infered data type in the expression is not the fixed data type
        /// </summary>
        /// <param name="converter">language converter</param>
        /// <param name="comp">compilation</param>
        /// <param name="proc">process</param>
        /// <param name="ut">UseTemplate object</param>
        /// <param name="valueName">name of the variable</param>
        /// <param name="defaultValue">initial value</param>
        /// <param name="desiredDataType">fixed data type</param>
        /// <returns>the converted expression</returns>
        public static string CreateExprVariable(ICodeConverter converter, ICompilateur comp, IProcessInstance proc, UseTemplate ut, string valueName, string defaultValue, EnumDataType desiredDataType)
        {
            // init
            string value = defaultValue;
            if (ut.Parameters.ContainsKey(valueName)) value = Helper.ReplaceStringParameter(ut, valueName);

            // convertir l'expression
            o2Mate.Expression.Convert(converter, value, proc.CurrentScope, false, desiredDataType);

            // définir en paramètre les variables utilisées
            foreach (string used in converter.CurrentFunction.UsedVariables)
            {
                comp.UpdateParameters(converter, proc, used, false);
            }
            // convertir à nouveau l'expression mais avec des variables
            o2Mate.Expression.Convert(converter, value, proc.CurrentScope, true, desiredDataType, true);
            return converter.CurrentFunction.CacheSource;
        }

        /// <summary>
        /// Create a value name
        /// </summary>
        /// <param name="ut">UseTemplate object</param>
        /// <param name="valueName">initial name</param>
        /// <returns></returns>
        public static string CreateValue(UseTemplate ut, string valueName)
        {
            string var = valueName;
            if (ut.Parameters.ContainsKey(valueName)) var = Helper.ReplaceStringParameter(ut, valueName);
            return var;
        }

        /// <summary>
        /// Call coding process
        /// </summary>
        /// <param name="comp">compilation</param>
        /// <param name="converter">language converter</param>
        /// <param name="ut">UseTemplate object</param>
        /// <param name="codingName">process name of coding</param>
        /// <param name="file">writing in</param>
        public static void CallCoding(ICompilateurInstance comp, ICodeConverter converter, UseTemplate ut, string codingName, FinalFile file)
        {
            Coding coding = ut.GetCoding(codingName);
            if (coding != null)
            {
                IProcess proc = comp.GetCodingProcess(coding.UniqueCodingName);
                proc.FunctionName = converter.ProcessAsFunction;
                comp.Convert(converter, proc, file);
                comp.RemoveCodingProcess(coding.UniqueCodingName);
            }
        }

        /// <summary>
        /// Call coding process
        /// Uses a newer variable in which you are adding it in the coding process
        /// </summary>
        /// <param name="comp">compilation</param>
        /// <param name="proc">process</param>
        /// <param name="converter">language converter</param>
        /// <param name="coding">coding object</param>
        /// <param name="varName">name of the variable</param>
        /// <param name="desiredDataType">desired data type of the variable</param>
        /// <param name="file">writing in</param>
        public static void CallCoding(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, Coding coding, string varName, EnumDataType desiredDataType, FinalFile file)
        {
            IProcess subProc = comp.GetCodingProcess(coding.UniqueCodingName);
            // la fonction courante est associée à ce processus
            // ce processus ne crée pas de fonction
            subProc.FunctionName = converter.ProcessAsFunction;
            // les instructions du coding sont stockées dans un processus à part
            // mais il n'y a pas de création d'une nouvelle fonction et cela implique
            // que je dois ranger les variables utilisées dans le processus au dessus
            // afin qu'elles soient déjà connues dans le processus du coding
            IData var = Helper.ConvertNonComputableVariableType(proc, converter, varName, desiredDataType);
            // mais j'ajoute la variable en local dans la fonction courante
            // pour ne pas qu'elle devienne un paramètre de la fonction courante
            Helper.AddIntoLocal(converter, var);
            comp.Convert(converter, subProc, file);
            comp.RemoveCodingProcess(coding.UniqueCodingName);
        }

        public static void IsolatedFunction(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, IsolatedFunctionDelegate d)
        {
            if (converter.CurrentFunction.IsStaticControlFlow)
            {
                Dictionary<string, IParameter> varNames, leftNames;
                Action<KeyValuePair<string, IParameter>> action;
                TypedVariable tv;

                IFunction prec = converter.CurrentFunction;
                IFunction thisFunc = new Function();
                thisFunc.IndentSize = prec.IndentSize;
                thisFunc.IsVisible = true;
                thisFunc.StrictName = "inner_" + comp.Unique.ComputeNewString();
                thisFunc.PropagateControlFlow(prec);

                // ajoute la fonction dans la liste
                converter.ImplementedFunctions.Add(thisFunc);
                // change la fonction courante
                converter.SetCurrentFunction(thisFunc);

                d();

                // retourne à la fonction précédente pour traiter le changement de type
                converter.SetCurrentFunction(prec);

                // supprime la fonction de la liste
                converter.ImplementedFunctions.Remove(thisFunc);

                leftNames = new Dictionary<string, IParameter>();
                // élimine les variables passées en paramètre qui n'ont pas changé de type
                thisFunc.Parameters.ForEach(new Action<IParameter>(delegate(IParameter par)
                {
                    if (!leftNames.ContainsKey(par.VariableName) && par.Order != EnumParameterOrder.E_NEW)
                        leftNames.Add(par.VariableName, par);
                }));
                // enregistre le nom distinct des variables passées en paramètre qui soient mutable et initiale
                varNames = new Dictionary<string, IParameter>();
                thisFunc.Parameters.ForEach(new Action<IParameter>(delegate(IParameter par)
                {
                    if (!varNames.ContainsKey(par.VariableName) && leftNames.ContainsKey(par.VariableName) && par.IsMutableParameter && par.Order == EnumParameterOrder.E_NEW)
                        varNames.Add(par.VariableName, par);
                }));
                // modifie toutes les variables en SimpleType
                action = new Action<KeyValuePair<string, IParameter>>(delegate(KeyValuePair<string, IParameter> kv)
                {
                    Regex reg = new Regex(@"\$\[([^:]*):([a-z]+_" + kv.Key + @")\]");
                    thisFunc.Source = reg.Replace(thisFunc.Source, new MatchEvaluator(delegate(Match m)
                    {
                        string type = m.Groups[1].Value;
                        string name = m.Groups[2].Value;
                        int pos = name.IndexOf('_');
                        string result = String.Empty;
                        if (pos != -1)
                        {
                            result = "$[" + type + ":st_" + name.Substring(pos + 1) + "]";
                        }
                        else
                        {
                            result = "$[" + type + ":st_" + name + "]";
                        }
                        return result;
                    }));
                    tv = new TypedVariable(kv.Value.VariableName, kv.Value.FormalParameter, kv.Value.DataType, EnumDataType.E_SIMPLETYPE);
                    tv.MoveType(converter, proc.CurrentScope);
                });
                foreach (KeyValuePair<string, IParameter> kv in varNames.Except(leftNames))
                {
                    action(kv);
                }
                // copie de l'implémentation de cette fonction dans la fonction en cours
                prec.Source += thisFunc.Source;
                // copies des variables locales de cette fonction dans la fonction en cours
                thisFunc.LocalVariables.ForEach(new Action<IStructure>(delegate(IStructure loc)
                {
                    if (!converter.CurrentFunction.LocalVariables.Exists(new Predicate<IStructure>(delegate(IStructure cur)
                    {
                        return cur.PrefixedFieldName == loc.PrefixedFieldName;
                    })))
                    {
                        converter.CurrentFunction.LocalVariables.Add(loc);
                    }
                }));
                // mettre à jour les paramètres
                foreach (KeyValuePair<string, IParameter> kv in varNames.Except(leftNames))
                {
                    List<IParameter> pars = thisFunc.Parameters.FindAll(new Predicate<IParameter>(delegate(IParameter par)
                    {
                        return par.VariableName == kv.Key;
                    }));
                    if (pars.Count > 1)
                    {
                        pars.ForEach(new Action<IParameter>(delegate(IParameter update)
                        {
                            if (update.Order == EnumParameterOrder.E_LAST)
                            {
                                tv = new TypedVariable(update.VariableName, update.FormalParameter, EnumDataType.E_SIMPLETYPE, update.DataType);
                                tv.MoveType(converter, proc.CurrentScope);
                            }
                        }));
                    }
                    else if (pars.Count == 1)
                    {
                        tv = new TypedVariable(pars[0].VariableName, pars[0].FormalParameter, EnumDataType.E_SIMPLETYPE, pars[0].DataType);
                        tv.MoveType(converter, proc.CurrentScope);
                    }
                }
            }
            else
            {
                d();
            }
        }
        #endregion
    }
}
