using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrip.MyTripLogic.Models
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.topografix.com/GPX/1/1")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.topografix.com/GPX/1/1", IsNullable = false, ElementName ="gpx")]
    public partial class Gpx
    {

        private gpxMetadata metadataField;

        private gpxTrk trkField;

        private gpxWpt[] wptField;

        private decimal versionField;

        private string creatorField;

        /// <remarks/>
        public gpxMetadata metadata
        {
            get
            {
                return this.metadataField;
            }
            set
            {
                this.metadataField = value;
            }
        }

        /// <remarks/>
        public gpxTrk trk
        {
            get
            {
                return this.trkField;
            }
            set
            {
                this.trkField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("wpt")]
        public gpxWpt[] wpt
        {
            get
            {
                return this.wptField;
            }
            set
            {
                this.wptField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string creator
        {
            get
            {
                return this.creatorField;
            }
            set
            {
                this.creatorField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.topografix.com/GPX/1/1")]
    public partial class gpxMetadata
    {

        private System.DateTime timeField;

        /// <remarks/>
        public System.DateTime time
        {
            get
            {
                return this.timeField;
            }
            set
            {
                this.timeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.topografix.com/GPX/1/1")]
    public partial class gpxTrk
    {

        private gpxTrkTrkpt[] trksegField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("trkpt", IsNullable = false)]
        public gpxTrkTrkpt[] trkseg
        {
            get
            {
                return this.trksegField;
            }
            set
            {
                this.trksegField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.topografix.com/GPX/1/1")]
    public partial class gpxTrkTrkpt
    {

        private System.DateTime timeField;

        private string fixField;

        private byte eleField;

        private byte satField;

        private decimal latField;

        private decimal lonField;

        /// <remarks/>
        public System.DateTime time
        {
            get
            {
                return this.timeField;
            }
            set
            {
                this.timeField = value;
            }
        }

        /// <remarks/>
        public string fix
        {
            get
            {
                return this.fixField;
            }
            set
            {
                this.fixField = value;
            }
        }

        /// <remarks/>
        public byte ele
        {
            get
            {
                return this.eleField;
            }
            set
            {
                this.eleField = value;
            }
        }

        /// <remarks/>
        public byte sat
        {
            get
            {
                return this.satField;
            }
            set
            {
                this.satField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal lat
        {
            get
            {
                return this.latField;
            }
            set
            {
                this.latField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal lon
        {
            get
            {
                return this.lonField;
            }
            set
            {
                this.lonField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.topografix.com/GPX/1/1")]
    public partial class gpxWpt
    {

        private System.DateTime timeField;

        private byte eleField;

        private string nameField;

        private decimal latField;

        private decimal lonField;

        /// <remarks/>
        public System.DateTime time
        {
            get
            {
                return this.timeField;
            }
            set
            {
                this.timeField = value;
            }
        }

        /// <remarks/>
        public byte ele
        {
            get
            {
                return this.eleField;
            }
            set
            {
                this.eleField = value;
            }
        }

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal lat
        {
            get
            {
                return this.latField;
            }
            set
            {
                this.latField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal lon
        {
            get
            {
                return this.lonField;
            }
            set
            {
                this.lonField = value;
            }
        }
    }
}