using MESStation.Interface.SAPRFC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInterface.SAP_RFC 
{
    class ZRFC_GET_PRO_HEADER8 : SAP_RFC_BASE
    {
        public ZRFC_GET_PRO_HEADER8() : base()
        {
            SetRFC_NAME("ZRFC_GET_PRO_HEADER8");
        }
        public void SetValues(string WO, string FACTORY)
        {
            ClearValues();

            SetValue("I_AUFNR", WO);
            SetValue("I_BUDAT", FACTORY);
    

        }
    }
}
