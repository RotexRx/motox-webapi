using MediatR;

public class UploadImageCommand : IRequest<int>
{
    public string ImagesBase64 { get; set; }
    public string Price { get; set; }

}
