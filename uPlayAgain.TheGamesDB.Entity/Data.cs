﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace uPlayAgain.TheGamesDB.Entity
{
    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Data
    {

        private string baseImgUrlField;

        private DataGame gameField;

        /// <remarks/>
        public string baseImgUrl
        {
            get
            {
                return this.baseImgUrlField;
            }
            set
            {
                this.baseImgUrlField = value;
            }
        }

        /// <remarks/>
        public DataGame Game
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

        public byte[] DowloadedFrontImage { get; set; }
    }
}
