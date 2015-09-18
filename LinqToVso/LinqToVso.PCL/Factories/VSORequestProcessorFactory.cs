using System;
using LinqToVso.Linqify;
using LinqToVso.PCL.Hooks;
using LinqToVso.PCL.Processes;
using LinqToVso.PCL.Subscriptions;
using LinqToVso.PCL.Team;
using LinqToVso.PCL.TeamRoom;

namespace LinqToVso.PCL.Factories
{
    public static class VsoRequestProcessorFactory
    {
        public static IRequestProcessor<T> Create<T>(string requestType) where T : class
        {
            switch (requestType)
            {
                case "Project":
                    return new ProjectRequestProcessor<T>();

                case "Team":
                    return new TeamRequestProcessor<T>();

                case "TeamMember":
                    return new TeamMemberRequestProcessor<T>();

                case "Process":
                    return new ProcessRequestProcessor<T>();

                case "Hook":
                    return new HookRequestProcessor<T>();

                case "Subscription":
                    return new SubscriptionRequestProcessor<T>();

                case "TeamRoom":
                    return new TeamRoomRequestProcessor<T>();

                default:
                    throw new ArgumentException("Type, " + requestType + " isn't a supported LINQ to VSO entity.",
                        "requestType");
            }
        }
    }
}