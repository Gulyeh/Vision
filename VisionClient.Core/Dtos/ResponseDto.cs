namespace VisionClient.Core.Dtos
{
    public class ResponseDto
    {
        public ResponseDto()
        {
            Response = new object();
        }

        public bool isSuccess { get; set; }
        public int Status { get; set; }
        public object Response { get; set; }
    }
}
