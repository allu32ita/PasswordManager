namespace PasswordManager.Models.Options
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class PasswordsOptions
    {

        public long PerPage { get; set; }

        public PasswordsOrderOptions Order { get; set; }
    }

    public partial class PasswordsOrderOptions
    {
        public string By { get; set; }

        public bool Ascending { get; set; }

        public string[] Allow { get; set; }
    }
}
