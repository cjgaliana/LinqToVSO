using Linqify;
using LinqToVso.PCL.Hooks;
using LinqToVso.PCL.Processes;
using LinqToVso.PCL.Subscriptions;
using LinqToVso.PCL.Team;

namespace LinqToVso
{
    public partial class VsoContext
    {
        public LinqifyQueryable<Project> Projects
        {
            get
            {
                return new LinqifyQueryable<Project>(this);
            }
        }

        public LinqifyQueryable<Team> Teams
        {
            get
            {
                return new LinqifyQueryable<Team>(this);
            }
        }

        public LinqifyQueryable<TeamMember> TeamMembers
        {
            get
            {
                return new LinqifyQueryable<TeamMember>(this);
            }
        }

        public LinqifyQueryable<Process> Processes
        {
            get
            {
                return new LinqifyQueryable<Process>(this);
            }
        }

        public LinqifyQueryable<Hook> Hooks
        {
            get
            {
                return new LinqifyQueryable<Hook>(this);
            }
        }

        public LinqifyQueryable<Subscription> Subscriptions
        {
            get
            {
                return new LinqifyQueryable<Subscription>(this);
            }
        }
    }
}