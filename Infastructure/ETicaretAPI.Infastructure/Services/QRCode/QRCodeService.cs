using ETicaretAPI.Application.Abstractions.Services.QRCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRCoder;
namespace ETicaretAPI.Infastructure.Services.QRCode
{
    public class QRCodeService : IQRCodeService
    {
        public byte[] GenerateQRCode(string text)
        {
            QRCodeGenerator generator = new();
            QRCodeData qRCodeData = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode pngByteQRCode = new PngByteQRCode(qRCodeData);
            byte[] byteGraphic = pngByteQRCode.GetGraphic(10, new byte[] { 84, 99, 71 }, new byte[] { 240, 240, 240 });

            return byteGraphic;
        }
    }
}
