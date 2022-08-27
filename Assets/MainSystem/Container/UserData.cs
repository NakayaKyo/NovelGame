using System.Collections.Generic;

namespace MainSystem.Container
{
    public class UserData
    {
        /// <summary>
        /// ƒ†[ƒU[•Ï”
        /// </summary>
        public Dictionary<string, string> UserVariable { get; set; }

        /// <summary>
        /// ‰Šú‰»
        /// </summary>
        public UserData()
        {
            UserVariable = new Dictionary<string, string>(){};
        }
    }
}