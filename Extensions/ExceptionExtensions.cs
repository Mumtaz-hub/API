using System;

namespace Extensions
{
    public static class ExceptionExtensions
    {
        public static Exception GetBaseException(this Exception ex)
        {
           return ex.GetBaseException();
         
        }

        public static string ToString(this Exception ex)
        {
            var innerEx = string.Empty;

            if(ex != null)
                innerEx += ex.Message;

            while (ex?.InnerException != null)
                innerEx += ex.InnerException.Message;

            return innerEx;
        }
    }
}