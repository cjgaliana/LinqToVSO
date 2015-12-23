using System;
using LinqToVso.Linqify;

namespace LinqToVso
{
    public static class VsoRequestProcessorFactory
    {
        public static IRequestProcessor<T> Create<T>(Type requestType) where T : class
        {
            if (requestType.FullName == typeof (Project).FullName)
            {
                return new ProjectRequestProcessor<T>();
            }

            if (requestType.FullName == typeof (Team).FullName)
            {
                return new TeamRequestProcessor<T>();
            }

            if (requestType.FullName == typeof (TeamMember).FullName)
            {
                return new TeamMemberRequestProcessor<T>();
            }

            if (requestType.FullName == typeof (Process).FullName)
            {
                return new ProcessRequestProcessor<T>();
            }

            if (requestType.FullName == typeof (Hook).FullName)
            {
                return new HookRequestProcessor<T>();
            }

            if (requestType.FullName == typeof (Subscription).FullName)
            {
                return new SubscriptionRequestProcessor<T>();
            }

            if (requestType.FullName == typeof (TeamRoom).FullName)
            {
                return new TeamRoomRequestProcessor<T>();
            }

            throw new ArgumentException(
                string.Format("Type, " + requestType + " isn't a supported LINQ to VSO entity.", requestType.Name));
        }
    }
}