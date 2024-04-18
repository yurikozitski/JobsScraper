namespace JobsScraper.BLL.EnumDisplayAttributes
{
    public abstract class ParamAttribute : Attribute
    {
        public string Text { get; set; }

        protected ParamAttribute(string paramText)
        {
            this.Text = paramText;
        }
    }
}
