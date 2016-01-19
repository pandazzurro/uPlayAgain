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
    public partial class DataGameImagesFanart
    {

        private DataGameImagesFanartOriginal originalField;

        private string thumbField;

        /// <remarks/>
        public DataGameImagesFanartOriginal original
        {
            get
            {
                return this.originalField;
            }
            set
            {
                this.originalField = value;
            }
        }

        /// <remarks/>
        public string thumb
        {
            get
            {
                return this.thumbField;
            }
            set
            {
                this.thumbField = value;
            }
        }
    }
}
