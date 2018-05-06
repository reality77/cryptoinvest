using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Services
{
    public class PlatformServices : IEnumerable<IPlatformService>
    {
        private static PlatformServices s_instance;

        public static PlatformServices Instance
        {
            get
            {
                lock (typeof(PlatformServices))
                {
                    if (s_instance != null)
                        return s_instance;
                    else
                    {
                        s_instance = new PlatformServices();
                        return s_instance;
                    }
                }
            }
        }

        private Dictionary<string, IPlatformService> _services;

        private PlatformServices()
        {
            _services = new Dictionary<string, IPlatformService>();
        }

        public void AddService(string serviceKey, IPlatformService service)
        {
            lock (_services)
            {
                _services.Add(serviceKey, service);
            }
        }

        public IPlatformService Get(string serviceKey)
        {
            lock (_services)
            {
                return _services.GetValueOrDefault(serviceKey);
            }
        }

        public int Count()
        {
            lock (_services)
            {
                return _services.Count;
            }
        }

        public IEnumerator<IPlatformService> GetEnumerator()
        {
            return _services.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _services.Values.GetEnumerator();
        }
    }


    public class PlatformRateResult
    {
        public IEnumerable<DateTime> Dates { get; set; }

        public dal.models.Currency CurrencySource { get; set; }
        public dal.models.Currency CurrencyTarget { get; set; }

        public Dictionary<DateTime, PlatformRateData> Rates { get; set; }
    }

    public class PlatformRateData
    {
        public DateTime Time { get; set; }
        public decimal Low { get; set; }
        public decimal High { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
    }
}
