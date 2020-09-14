using System;
using System.Collections.Generic;
using System.Globalization;
using Wanghzh.Prism.Properties;
namespace Wanghzh.Prism.Modularity
{
    public class ModuleDependencySolver
    {
        private readonly ListDictionary<string, string> dependencyMatrix = new ListDictionary<string, string>();
        private readonly List<string> knownModules = new List<string>();
        public void AddModule(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.StringCannotBeNullOrEmpty, "name"));
            AddToDependencyMatrix(name);
            AddToKnownModules(name);
        }
        public void AddDependency(string dependingModule, string dependentModule)
        {
            if (String.IsNullOrEmpty(dependingModule))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.StringCannotBeNullOrEmpty, "dependingModule"));
            if (String.IsNullOrEmpty(dependentModule))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.StringCannotBeNullOrEmpty, "dependentModule"));
            if (!knownModules.Contains(dependingModule))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.DependencyForUnknownModule, dependingModule));
            AddToDependencyMatrix(dependentModule);
            dependencyMatrix.Add(dependentModule, dependingModule);
        }
        private void AddToDependencyMatrix(string module)
        {
            if (!dependencyMatrix.ContainsKey(module))
            {
                dependencyMatrix.Add(module);
            }
        }
        private void AddToKnownModules(string module)
        {
            if (!knownModules.Contains(module))
            {
                knownModules.Add(module);
            }
        }
        public string[] Solve()
        {
            List<string> skip = new List<string>();
            while (skip.Count < dependencyMatrix.Count)
            {
                List<string> leaves = this.FindLeaves(skip);
                if (leaves.Count == 0 && skip.Count < dependencyMatrix.Count)
                {
                    throw new CyclicDependencyFoundException(Resources.CyclicDependencyFound);
                }
                skip.AddRange(leaves);
            }
            skip.Reverse();
            if (skip.Count > knownModules.Count)
            {
                string moduleNames = this.FindMissingModules(skip);
                throw new ModularityException(moduleNames, String.Format(CultureInfo.CurrentCulture,
                                                            Resources.DependencyOnMissingModule,
                                                            moduleNames));
            }
            return skip.ToArray();
        }
        private string FindMissingModules(List<string> skip)
        {
            string missingModules = "";
            foreach (string module in skip)
            {
                if (!knownModules.Contains(module))
                {
                    missingModules += ", ";
                    missingModules += module;
                }
            }
            return missingModules.Substring(2);
        }
        public int ModuleCount
        {
            get { return dependencyMatrix.Count; }
        }
        private List<string> FindLeaves(List<string> skip)
        {
            List<string> result = new List<string>();
            foreach (string precedent in dependencyMatrix.Keys)
            {
                if (skip.Contains(precedent))
                {
                    continue;
                }
                int count = 0;
                foreach (string dependent in dependencyMatrix[precedent])
                {
                    if (skip.Contains(dependent))
                    {
                        continue;
                    }
                    count++;
                }
                if (count == 0)
                {
                    result.Add(precedent);
                }
            }
            return result;
        }
    }
}
