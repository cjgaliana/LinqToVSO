
using Linqify;
using LinqToVso;
using LinqToVso.PCL.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToVso.PCL.Context;

namespace ConsoleSample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            MainAsync().Wait();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static async Task MainAsync()
        {
            try
            {
    
                var handler = new BasicAuthHttpClientHandler("user", "password");
                var vsoExecutor = new VsoExecute(handler);
                var context = new VsoContext(vsoExecutor);

                var projects = await context.Projects.Include("Test").ToListAsync();

                //var teams = await context.Teams
                //    .Where(x => x.ProjectId == projects.FirstOrDefault().Id)
                //    .ToListAsync();

                //var firstTeam = teams.FirstOrDefault();
                //var members = await context.TeamMembers
                //    .Where(x => x.ProjectId == projects.FirstOrDefault().Id && x.TeamId == firstTeam.Id)
                //    .ToListAsync();

                //PrintProjects(projects);

                var processes = await context.Processes.ToListAsync();
                var processDetails =
                    await context.Processes.
                    Where(x => x.Id == processes.FirstOrDefault().Id)
                    .FirstOrDefaultAsync();
                var a = 5;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw;
            }
        }

        private static void PrintProjects(List<Project> projects)
        {
            Console.WriteLine("Projects:");

            foreach (var project in projects)
            {
                Console.WriteLine(
                    "ID: {0}\nName: {1}\nDescription: {2}\n\n",
                    project.Id,
                    project.Name,
                    project.Description);
            }
        }
    }
}