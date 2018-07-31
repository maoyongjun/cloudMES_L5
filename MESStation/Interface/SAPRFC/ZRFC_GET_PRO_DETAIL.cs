using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Interface.SAPRFC
{
    class ZRFC_GET_PRO_DETAIL : SAP_RFC_BASE
    {
        public ZRFC_GET_PRO_DETAIL() : base()
        {
            SetRFC_NAME("ZRFC_GET_PRO_DETAIL");
        }
        public void SetValues(string WO, string FACTORY)
        {
            ClearValues();

            SetValue("PO_NO", WO);
            SetValue("PLANT", FACTORY);


        }
    }
}
