using System;

namespace OT.Assessment.App.Exceptions
{
    
        public class NotFoundException : Exception
        {
            public NotFoundException(string errorMessage) : base(errorMessage) { }
        }

        public class ValidationException : Exception
        {
            public ValidationException(string errorMessage) : base(errorMessage) { }
        }

        public class ServiceException : Exception
        {
            public ServiceException(string errorMessage) : base(errorMessage) { }
        }

}
