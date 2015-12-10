namespace LinqToVso.Samples.UWP.Services
{
    public interface IVsoDataService
    {
        void Initialize(string account, string username, string password);

        VsoContext Context { get; }
    }

    public class VsoDataService : IVsoDataService
    {
        private VsoContext _vsoContext;

        public void Initialize(string account, string username, string password)
        {
            this._vsoContext = new VsoContext(account, username, password);
        }

        public VsoContext Context => this._vsoContext;
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