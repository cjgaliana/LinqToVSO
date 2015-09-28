using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToVso;
using LinqToVso.Linqify;
using LinqToVso.PCL.Authorization;
using LinqToVso.PCL.Context;

namespace WPFSample.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public async Task LoadAsync()
        {
            try
            {
                var context = new VsoContext("accoutn", "user", "password");

                var projects = context.Projects.ToList();
                var projectsasync = await context.Projects.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading main window: {0}\nStacktrace: {1}", ex.Message, ex.StackTrace);
                throw;
            }
        }
    }
}