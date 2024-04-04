namespace Exceptions
{
    public class InvalidPersonIDException : ArgumentException
    {
        public InvalidPersonIDException() :base() { }
        public InvalidPersonIDException(string message) : base(message)
        {

        }
        public InvalidPersonIDException(string? message,Exception? innerException)
        {
            // vậy thôi nó sẽ tự khởi tạo constructor -> cần thiết lắm mới xài :))_
        }


    }
}
