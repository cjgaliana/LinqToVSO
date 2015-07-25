using LinqToVso.PCL.Context;
using LinqToVso.PCL.Hooks;
using LinqToVso.PCL.Processes;
using LinqToVso.PCL.Team;

namespace LinqToVso
{
    public partial class VsoContext
    {
        public VsoQueryable<Project> Projects
        {
            get
            {
                return new VsoQueryable<Project>(this);
            }
        }

        public VsoQueryable<Team> Teams
        {
            get
            {
                return new VsoQueryable<Team>(this);
            }
        }

        public VsoQueryable<TeamMember> TeamMembers
        {
            get
            {
                return new VsoQueryable<TeamMember>(this);
            }
        }

        public VsoQueryable<Process> Processes
        {
            get
            {
                return new VsoQueryable<Process>(this);
            }
        }

        public VsoQueryable<Hook> Hooks
        {
            get
            {
                return new VsoQueryable<Hook>(this);
            }
        }
    }
}