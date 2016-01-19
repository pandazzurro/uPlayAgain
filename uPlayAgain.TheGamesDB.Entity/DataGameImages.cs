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
    public partial class DataGameImages
    {

        private DataGameImagesFanart[] fanartField;

        private DataGameImagesBoxart[] boxartField;

        private DataGameImagesBanner[] bannerField;

        private DataGameImagesScreenshot screenshotField;

        private DataGameImagesClearlogo clearlogoField;

        /// <remarks/>
        [XmlElementAttribute("fanart")]
        public DataGameImagesFanart[] fanart
        {
            get
            {
                return this.fanartField;
            }
            set
            {
                this.fanartField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute("boxart")]
        public DataGameImagesBoxart[] boxart
        {
            get
            {
                return this.boxartField;
            }
            set
            {
                this.boxartField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute("banner")]
        public DataGameImagesBanner[] banner
        {
            get
            {
                return this.bannerField;
            }
            set
            {
                this.bannerField = value;
            }
        }

        /// <remarks/>
        public DataGameImagesScreenshot screenshot
        {
            get
            {
                return this.screenshotField;
            }
            set
            {
                this.screenshotField = value;
            }
        }

        /// <remarks/>
        public DataGameImagesClearlogo clearlogo
        {
            get
            {
                return this.clearlogoField;
            }
            set
            {
                this.clearlogoField = value;
            }
        }
    }
}
