using System;
using System.Collections.Generic;
using System.Text;

namespace micro_c_app
{
    //https://stackoverflow.com/a/44126899
    public interface IToastMessage
    {
        void LongAlert(string message);
        void ShortAlert(string message);
    }
}
