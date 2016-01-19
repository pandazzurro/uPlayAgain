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
    public partial class DataGameSimilar
    {

        private int similarCountField;

        private DataGameSimilarGame[] gameField;

        /// <remarks/>
        public int SimilarCount
        {
            get
            {
                return this.similarCountField;
            }
            set
            {
                this.similarCountField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute("Game")]
        public DataGameSimilarGame[] Game
        {
            get
            {
                return this.gameField;
            }
            set
            {
                this.gameField = value;
            }
        }
    }
}
