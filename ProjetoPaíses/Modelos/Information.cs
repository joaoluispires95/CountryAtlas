namespace ProjetoPaíses.Modelos
{
    public class Information
    {
        string _Name;

        string _Info;

        public int id { get; set; }

        public string Name
        {
            get { return _Name; }

            set
            {
                if (value != null && value != "")
                    _Name = value;
                else
                    _Name = "Name not available";
            }
        }

        public string Info
        {
            get { return _Info; }

            set
            {
                if (value != null && value != "")
                    _Info = value;
                else
                    _Info = "Information not available";
            }
        }
    }
}
