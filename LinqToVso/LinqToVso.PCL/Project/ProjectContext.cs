// Camilo Galiana
// LinqToVso.PCL
// ProjectContext.cs
// 19 / 07 / 2015

using System;
using System.Threading.Tasks;

namespace LinqToVso
{
    public partial class VsoContext
    {
        public async Task CreateProject(string name, string description, SourceControlType sourceControl,
            string templateId)
        {
            throw new NotImplementedException(
                "Sorry! This method is not implemented. If you want to contribute with the library, I accept Pull Requests at https://github.com/cjgaliana/LinqToVSO");
        }
    }
}