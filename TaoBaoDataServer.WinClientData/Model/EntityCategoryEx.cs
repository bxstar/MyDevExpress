using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaoBaoDataServer.WinClientData.Model
{
    /// <summary>
    /// 类目扩展类
    /// </summary>
    public class EntityCategoryEx
    {
        public long CatId { get; set; }

        public long CatLevel { get; set; }

        public string CatName { get; set; }

        public string CatPathId { get; set; }

        public string CatPathName { get; set; }

        public long ParentCatId { get; set; }


        public long Click { get; set; }
        
        public long Competition { get; set; }
        
        public long Cost { get; set; }
        
        public string Coverage { get; set; }
        
        public string Cpc { get; set; }
        
        public string Ctr { get; set; }
        
        public long Directtransaction { get; set; }
        
        public long Directtransactionshipping { get; set; }
        
        public long Favitemtotal { get; set; }
        
        public long Favshoptotal { get; set; }
        
        public long Favtotal { get; set; }
        
        public long Impression { get; set; }
        
        public long Indirecttransaction { get; set; }
        
        public long Indirecttransactionshipping { get; set; }
        
        public string Roi { get; set; }
        
        public long Transactionshippingtotal { get; set; }
        
        public long Transactiontotal { get; set; }
    }
}
