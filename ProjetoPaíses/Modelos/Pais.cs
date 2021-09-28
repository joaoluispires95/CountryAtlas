namespace ProjetoPaíses.Modelos
{
    using System.Collections.Generic;

    public class Pais
    {
        string _name;

        string _capital;

        string _region;

        string _subRegion;

        int? _population;

        double? _gini;

        string _flag;

        string _alpha3Code;

        List<double> _latlng;

        public string Name
        {
            get { return _name; }

            set
            {
                if (value != null && value != "")
                    _name = value;
                else
                    _name = "Name not available";
            }
        }
        public string Capital
        {
            get { return _capital; }

            set
            {
                if (value != null && value != "")
                    _capital = value;
                else
                    _capital = "Capital not available";
            }
        }
        public string Region
        {
            get { return _region; }

            set
            {
                if (value != null && value != "")
                    _region = value;
                else
                    _region = "Region not available";
            }
        }
        public string SubRegion
        {
            get { return _subRegion; }

            set
            {
                if (value != null && value != "")
                    _subRegion = value;
                else
                    _subRegion = "Sub-region not available";
            }
        }
        public int? Population
        {
            get { return _population; }

            set
            {
                if (value != null)
                    _population = value;
                else
                    _population = 0;
            }
        }
        public double? Gini
        {
            get { return _gini; }

            set
            {
                if (value != null)
                    _gini = value;
                else
                    _gini = 0;
            }
        }
        public string Flag
        {
            get { return _flag; }

            set
            {
                if (value != null)
                    _flag = value;
                else
                    _flag = "This country's flag is not available";
            }
        }
        public string Alpha3Code
        {
            get { return _alpha3Code; }

            set
            {
                if (value != null && value != "")
                    _alpha3Code = value;
                else
                    _alpha3Code = "Alpha3Code not available";
            }
        }

        public List<double> latlng
        {
            get { return _latlng; }

            set
            {
                if (value != null)
                    _latlng = value;
                else
                    _latlng = null;
            }
        }
    }
}
