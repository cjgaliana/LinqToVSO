namespace LinqToVso.Samples.UWP.Services
{
    public interface IVsoDataService
    {
        void Initialize(string account, string username, string password);

        VsoContext Context { get; }
    }

    public class VsoDataService : IVsoDataService
    {
        public void Initialize(string account, string username, string password)
        {
            this.Context = new VsoContext(account, username, password);
        }

        public VsoContext Context { get; private set; }
    }

    public class VsoDesignDataService : IVsoDataService
    {
        public void Initialize(string account, string username, string password)
        {
            //Nothing, is design time!
        }

        public VsoContext Context { get; set; }
    }
}