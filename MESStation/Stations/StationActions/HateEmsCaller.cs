using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using MESStation.HateEmsGetDataService;

namespace MESStation.Stations.StationActions
{
    public class HateEmsCaller
    {
        public static object EmsService(object value)
        {
            if (value is HateEmsData)
            {
                var data = (HateEmsData)value;
                if (!string.IsNullOrEmpty(data.MesWebProxy))
                {
                    var proxy = new WebProxy(data.MesWebProxy, true);
                    //var proxy = new WebProxy(data.MesWebProxy, data.MesWebProxyPort);
                    WebRequest.DefaultWebProxy = proxy;
                }

                HateEmsGetDataService.HateEmsGetDataService Ems = new HateEmsGetDataServiceClient();

                var header = new header { props = new headerEntry[3] };
                header.props[0] = new headerEntry { key = "username", value = data.UserName+"@"+data.Factory };
                header.props[1] = new headerEntry { key = "factory", value = data.Factory };
                header.props[2] = new headerEntry { key = "procstep", value = data.ProcStep };

                var ServiceIn = new hateEmsGetDataServiceIn();
                ServiceIn.language = data.Language;
                ServiceIn.service = data.Service;
                ServiceIn.barcode = data.Barcode;
                ServiceIn.operation = data.Operation;
                ServiceIn.barcodeType = data.BarcodeType;
                ServiceIn.result = data.Result;

                var DataService = new emsGetDataService() { @in = ServiceIn };
                var Request = new emsGetDataService1(header, DataService);
                var Response = Ems.emsGetDataService(Request);
                var ServiceOut = Response.@out;

                return ServiceOut;
            }
            return null;
        }
    }

    public class HateEmsData
    {
        public string MesWebProxy { get; set; }

        public string MesWebProxyPort { get; set; }
        public string UserName { get; set; }
        public string Factory { get; set; }
        public string ProcStep { get; set; }
        public string Barcode { get; set; }
        public string Operation { get; set; }
        public string BarcodeType { get; set; }
        public string Service { get; set; }
        public string Result { get; set; }
        public ushort Language { get; set; }
    }
}
