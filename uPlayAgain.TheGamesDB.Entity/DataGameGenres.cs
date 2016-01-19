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
    public partial class DataGameGenres
    {

        private string genreField;

        /// <remarks/>
        public string genre
        {
            get
            {
                return this.genreField;
            }
            set
            {
                this.genreField = value;
            }
        }
    }
}
