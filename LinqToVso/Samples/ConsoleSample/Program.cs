using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToVso;
using LinqToVso.Linqify;
using LinqToVso.PCL.Authorization;
using LinqToVso.PCL.Context;
using LinqToVso.PCL.Hooks;

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

                //var processes = await context.Processes.ToListAsync();
                //var processDetails =
                //    await context.Processes.
                //    Where(x => x.Id == processes.FirstOrDefault().Id)
                //    .FirstOrDefaultAsync();

                var consumerHooks = await context.Hooks
                    .Where(x => x.Type == HookType.Consumer)
                    .ToListAsync();

                var publisherHooks = await context.Hooks
                    .Where(x => x.Type == HookType.Publisher)
                    .ToListAsync();

                var subscriptions = await context.Subscriptions.ToListAsync();
                var firstSubscription = await context.Subscriptions
                    .Where(x => x.Id == subscriptions.FirstOrDefault().Id)
                    .FirstOrDefaultAsync();


                var teamRooms = await context.TeamRooms.ToListAsync();
                var firstRoom = await context.TeamRooms.
                    Where(x => x.Id == teamRooms.FirstOrDefault().Id)
                    .FirstOrDefaultAsync();
                var firstRoomMembers = await context.TeamMembers.Where(x => x.TeamRoomId == firstRoom.Id).ToListAsync();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw;
            }
        }
    }
}