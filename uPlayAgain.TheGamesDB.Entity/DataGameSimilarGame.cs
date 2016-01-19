using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace uPlayAgain.TheGamesDB.Entity
{
    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class DataGameSimilarGame
    {

        private int idField;

        private int platformIdField;

        /// <remarks/>
        public int id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        public int PlatformId
        {
            get
            {
                return this.platformIdField;
            }
            set
            {
                this.platformIdField = value;
            }
        }
    }
}
