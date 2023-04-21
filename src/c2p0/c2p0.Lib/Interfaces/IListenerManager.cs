using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using c2p0.Lib.Models;

namespace c2p0.Lib.Interfaces
{
    public interface IListenerManager
    {
        public List<IListener> GetListeners();
        public void AddListener(IListener listener);
        public bool RemoveListener(IListener listener);
        public IListener GetListener(string guid);
    }
    public class ListenerManager : IListenerManager
    {
        private List<IListener> Listeners = new List<IListener>();
        public List<IListener> GetListeners()
        {
            return Listeners;
        }

        public void AddListener(IListener listener) 
        {
            Listeners.Add(listener);
        }

        public bool RemoveListener(IListener listener)
        {
            return Listeners.Remove(listener);
        }

        public IListener GetListener(string guid)
        {
            return Listeners.FirstOrDefault(x => x.ListenerGuid == guid);
        }
    }
}
