using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Converters;

namespace o2Mate
{
    internal class BeginJobs : ILanguageConverter
    {
        #region Private Fields
        private List<ICompiler> statements;
        private ICompilateurInstance cachedComp;
        private IProcess cachedProc;
        private IFunction cachedPreviousFunc;
        private IFunction cachedFunc;
        private bool alreadyInstantiated;
        private IStructure structRef;
        private List<IStructure> structList;
        #endregion

        #region Default Constructor
        public BeginJobs()
        {
            this.statements = new List<ICompiler>();
            this.alreadyInstantiated = false;
            this.structRef = null;
            this.structList = new List<IStructure>();
        }
        #endregion

        #region Public Properties
        public List<ICompiler> Statements
        {
            get { return this.statements; }
        }

        public IStructure Structure
        {
            get { return this.structRef; }
        }

        #endregion

        #region Private Methods
        private IEnumerable<IStructure> GetElements(List<IStructure> current, ICodeConverter converter, IFunction fun)
        {
            List<string> distincts = new List<string>();
            foreach (IStructure st in current) { yield return st; }
            foreach (IParameter par in fun.Parameters)
            {
                // ne pas ajouter si existe déjà
                if (!current.Exists(new Predicate<IStructure>(delegate(IStructure st)
                {
                    return (!converter.IsStronglyTyped && st.FieldName == par.VariableName) || (converter.IsStronglyTyped && st.PrefixedFieldName == par.EffectiveParameter);
                })))
                {
                    // ne pas ajouter des paramètres typés, utiliser des VariableName distinctes pour des langages non typés
                    if (converter.IsStronglyTyped || !distincts.Exists(new Predicate<string>(delegate(string s)
                    {
                        return s == par.VariableName;
                    })))
                    {
                        if (par.DataType == EnumDataType.E_STRUCTURE)
                        {
                            foreach (IStructure copied in par.StructureReferences)
                            {
                                if (converter.IsStronglyTyped)
                                    yield return converter.CreateNewField(this.Structure.FieldName, copied.DataType, copied.PrefixedFieldName, copied.IsMutable);
                                else
                                    yield return converter.CreateNewField(this.Structure.FieldName, copied.DataType, copied.FieldName, copied.IsMutable);
                            }
                        }
                        else
                            if (converter.IsStronglyTyped)
                                yield return converter.CreateNewField(this.Structure.FieldName, par.DataType, par.EffectiveParameter, par.IsMutableParameter);
                            else
                                yield return converter.CreateNewField(this.Structure.FieldName, par.DataType, par.VariableName, par.IsMutableParameter);
                    }
                }
            }
        }
        #endregion

        #region Public Methods
        public void ConvertToParallel(ICodeConverter converter, ICompilateurInstance comp, IProcessInstance previousProc, FinalFile file)
        {
            EndJobs ej = new EndJobs();

            this.cachedComp = comp;
            this.cachedPreviousFunc = converter.ImplementedFunctions.FindLast(new Predicate<IFunction>(delegate(IFunction search)
            {
                return search.StrictName == previousProc.FunctionName;
            }));

            // première instruction
            this.Statements[0].Convert(converter, previousProc, file);

            // implémenter les fonctions job_xxx
            for (int index = 1; index < this.Statements.Count - 1; ++index)
            {
                ICompiler tache = this.Statements[index];
                // Check if it's a call job (blindage)
                if (tache is CallJob)
                {
                    CallJob cj = this.Statements[index] as CallJob;
                    ej.Jobs.Add(cj);

                    this.cachedProc = this.cachedComp.GetJobProcess(cj.ProcessName);
                    // Ecriture du code de la fonction
                    IFunction newFunc = Helper.MakeNewMethod(converter, this.cachedProc);
                    Helper.FillNewMethod(comp, this.cachedProc, converter, newFunc, file);
                    newFunc.IsJob = true;
                    //if (this.cachedProc.HasChanged) previousProc.HasChanged = true;

                    // aller chercher les variables exterieures au processus appellé
                    this.cachedFunc = converter.ImplementedFunctions.FindLast(new Predicate<IFunction>(delegate(IFunction func) { return func.IsJob && func.StrictName == cj.ProcessName; }));
                    if (!converter.CallingFunctions.Exists(new Predicate<IFunction>(delegate(IFunction func) { return func.IsJob && func.StrictName == cj.ProcessName && func.InstanceNumber == this.cachedFunc.InstanceNumber; })))
                    {
                        converter.CallingFunctions.Add(this.cachedFunc);
                    }

                    // convertir pour le parallèllisme
                    if (!this.alreadyInstantiated)
                    {
                        // create a new structure to contain all external values
                        string structTypeName = this.cachedComp.Unique.ComputeNewString();
                        ++converter.CurrentFunction.PrivateVariableCounter;
                        string structInstanceName = structTypeName + converter.CurrentFunction.PrivateVariableCounter.ToString();

                        // Construit une structure et indique que l'instance doit être mutable
                        this.structRef = converter.CreateNewField(structTypeName, structInstanceName, true);
                    }

                    // ajoute une instance de la structure déclarée dans la fonction en cours
                    if (converter.IsStronglyTyped)
                        this.cachedFunc.InstancesStructure.Add(this.Structure.PrefixedFieldName, this.Structure.StructureType);
                    else
                        this.cachedFunc.InstancesStructure.Add(this.Structure.FieldName, this.Structure.StructureType);

                    // créer un nouveau paramètre et y inscrire tous les autres paramètres
                    Converters.Parameter parStructure = new Parameter();
                    parStructure.EffectiveParameter = this.Structure.PrefixedFieldName;
                    parStructure.FormalParameter = this.Structure.PrefixedFieldName;
                    parStructure.VariableName = this.Structure.FieldName;
                    parStructure.IsComputable = false;
                    parStructure.IsMutableParameter = true;
                    // stocker tous les paramètres dans le nouveau paramètre de type structure
                    List<IStructure> newParams = this.GetElements(this.structList, converter, this.cachedFunc).ToList();
                    parStructure.StructureReferences.AddRange(newParams);
                    this.structList = newParams;
                    parStructure.DataType = EnumDataType.E_STRUCTURE;

                    this.cachedFunc.Parameters.Clear();
                    this.cachedFunc.Parameters.Add(parStructure);
                    this.alreadyInstantiated = true;

                }

            }

            // ajout de la structure
            converter.Convert(this);

            // création des instructions en parallèle
            // appels des fonctions job_xxx
            for (int index = 1; index < this.Statements.Count - 1; ++index)
            {
                ICompiler tache = this.Statements[index];
                // Check if it's a call job (blindage)
                if (tache is CallJob)
                {
                    CallJob cj = tache as CallJob;
                    cj.StructureReference = this.Structure;
                    this.cachedProc = this.cachedComp.GetJobProcess(cj.ProcessName);
                    this.cachedProc.MakeOneInstance(previousProc, new RunInnerInstanceProcess(delegate(IProcessInstance previous, IProcessInstance i)
                    {
                        cj.Convert(converter, i, file);
                    }));
                }
            }

            // attendre la fin de toutes les tâches
            ej.WaitForAllJobs(converter);

            // dernière instruction
            this.Statements[this.Statements.Count - 1].Convert(converter, previousProc, file);
        }

        #endregion

        #region ILanguageConverter Members

        public void WriteInVBScript(ICodeConverter converter)
        {
            throw new NotImplementedException();
        }

        public void WriteInPowerShell(ICodeConverter converter)
        {
            converter.CurrentFunction.AddToSource("$" + this.Structure.FieldName + " = @{" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            foreach (IStructure st in this.structList)
            {
                if (st.IsMutable)
                    converter.CurrentFunction.AddToSource("\"" + st.FieldName + "\" = S[byref:" + st.FieldName + "]" + Environment.NewLine);
                else
                    converter.CurrentFunction.AddToSource("\"" + st.FieldName + "\" = S[byvalue:" + st.FieldName + "]" + Environment.NewLine);
            }
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
        }

        public void WriteInPerl(ICodeConverter converter)
        {
            throw new NotImplementedException();
        }

        public void WriteInPython(ICodeConverter converter)
        {
            throw new NotImplementedException();
        }

        public void WriteInMicrosoftCPP(ICodeConverter converter)
        {
            converter.CurrentFunction.AddToSource(this.Structure.StructureType + " " + this.Structure.PrefixedFieldName + ";" + Environment.NewLine);
            foreach (IStructure st in this.structList)
            {
                if (st.IsMutable)
                    converter.CurrentFunction.AddToSource(this.Structure.PrefixedFieldName + "." + st.PrefixedFieldName + " = $[byref:" + st.PrefixedFieldName + "];" + Environment.NewLine);
                else
                    converter.CurrentFunction.AddToSource(this.Structure.PrefixedFieldName + "." + st.PrefixedFieldName + " = $[byvalue:" + st.PrefixedFieldName + "];" + Environment.NewLine);
            }
            converter.CurrentFunction.AddToSource("Parallel().Init(" + (this.Statements.Count - 2).ToString() + ");" + Environment.NewLine);
        }

        public void WriteInUnixCPP(ICodeConverter converter)
        {
            throw new NotImplementedException();
        }

        public void WriteInCSharp(ICodeConverter converter)
        {
            throw new NotImplementedException();
        }

        public void WriteInJava(ICodeConverter converter)
        {
            throw new NotImplementedException();
        }

        public void WriteInMacOSCPP(ICodeConverter converter)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
