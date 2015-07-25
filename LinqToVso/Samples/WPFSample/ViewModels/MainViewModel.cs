using LinqToVso;
using LinqToVso.PCL.Authorization;
using LinqToVso.PCL.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WPFSample.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
        }

        public async Task LoadAsync()
        {
            try
            {
                var context = new VsoContext(new BasicAuth
                {
                    User = "user",
                    Password = "password",
                    Account = "account"
                });
                var projects = context.Projects.ToList();
                var projectsasync = await context.Projects.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error loading main window: {0}\nStacktrace: {1}", ex.Message, ex.StackTrace));
                throw;
            }
        }
    }
}