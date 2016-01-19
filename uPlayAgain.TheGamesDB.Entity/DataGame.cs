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
    public partial class DataGame
    {

        private int idField;

        private string gameTitleField;

        private int platformIdField;

        private string platformField;

        private string releaseDateField;

        private string overviewField;

        private string eSRBField;

        private DataGameGenres genresField;

        private string playersField;

        private string coopField;

        private string youtubeField;

        private string publisherField;

        private string developerField;

        private decimal ratingField;

        private DataGameSimilar similarField;

        private DataGameImages imagesField;

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
        public string GameTitle
        {
            get
            {
                return this.gameTitleField;
            }
            set
            {
                this.gameTitleField = value;
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

        /// <remarks/>
        public string Platform
        {
            get
            {
                return this.platformField;
            }
            set
            {
                this.platformField = value;
            }
        }

        /// <remarks/>
        public string ReleaseDate
        {
            get
            {
                return this.releaseDateField;
            }
            set
            {
                this.releaseDateField = value;
            }
        }

        /// <remarks/>
        public string Overview
        {
            get
            {
                return this.overviewField;
            }
            set
            {
                this.overviewField = value;
            }
        }

        /// <remarks/>
        public string ESRB
        {
            get
            {
                return this.eSRBField;
            }
            set
            {
                this.eSRBField = value;
            }
        }

        /// <remarks/>
        public DataGameGenres Genres
        {
            get
            {
                return this.genresField;
            }
            set
            {
                this.genresField = value;
            }
        }

        /// <remarks/>
        public string Players
        {
            get
            {
                return this.playersField;
            }
            set
            {
                this.playersField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute("Co-op")]
        public string Coop
        {
            get
            {
                return this.coopField;
            }
            set
            {
                this.coopField = value;
            }
        }

        /// <remarks/>
        public string Youtube
        {
            get
            {
                return this.youtubeField;
            }
            set
            {
                this.youtubeField = value;
            }
        }

        /// <remarks/>
        public string Publisher
        {
            get
            {
                return this.publisherField;
            }
            set
            {
                this.publisherField = value;
            }
        }

        /// <remarks/>
        public string Developer
        {
            get
            {
                return this.developerField;
            }
            set
            {
                this.developerField = value;
            }
        }

        /// <remarks/>
        public decimal Rating
        {
            get
            {
                return this.ratingField;
            }
            set
            {
                this.ratingField = value;
            }
        }

        /// <remarks/>
        public DataGameSimilar Similar
        {
            get
            {
                return this.similarField;
            }
            set
            {
                this.similarField = value;
            }
        }

        /// <remarks/>
        public DataGameImages Images
        {
            get
            {
                return this.imagesField;
            }
            set
            {
                this.imagesField = value;
            }
        }
    }
}
