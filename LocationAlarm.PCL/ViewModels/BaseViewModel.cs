using LocationAlarm.PCL.Utils;

namespace LocationAlarm.PCL.ViewModels
{
    public class BaseViewModel : NotifiableViewModel
    {
        public virtual void OnStart()
        { }

        public virtual void OnStop()
        { }
    }
}
