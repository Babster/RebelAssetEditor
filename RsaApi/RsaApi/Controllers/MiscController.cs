using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RsaApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Misc")]
    public class MiscController : ApiController
    {

        [Route("Image")]
        public HttpResponseMessage GetImage(int ImageId)
        {
            HttpResponseMessage response;

            string steamId = User.Identity.Name;
            int playerId = PlayerDataSql.PlayerId(steamId);
            if(playerId == 0)
            {
                response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                return response;
            }

            Story.RebelImageToTransfer curImage = Story.RebelImageToTransfer.ImageToTransferById(ImageId);
            if(curImage == null)
            {
                response = new HttpResponseMessage(HttpStatusCode.NotFound);
                return response;
            }

            string serializedElement = JsonConvert.SerializeObject(curImage);
            response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(serializedElement);
            return response;
        }
    }
}
