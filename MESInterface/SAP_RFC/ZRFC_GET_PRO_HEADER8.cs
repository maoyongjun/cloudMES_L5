using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInterface.SAP_RFC 
{
    class ZRFC_GET_PRO_HEADER8 : SAP_RFC_BASE
    {
        public ZRFC_GET_PRO_HEADER8(string BU) : base(BU)
        {
            SetRFC_NAME("ZRFC_GET_PRO_HEADER8");
        }
        public void SetValue(string WO, string POSTDATE,
          string QTY, string SAP_STATION_CODE)
        {
            ClearValues();

            SetValue("I_AUFNR", WO);
            SetValue("I_BUDAT", POSTDATE);
            //SetValue("I_FLAG", confirmed_flag);
            //SetValue("I_LGORT_TO", storge);
            SetValue("I_LMNGA", QTY);
            SetValue("I_STATION", SAP_STATION_CODE);

        }
    }
}
