using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class InformationController : ApiController
    {

        DataClasses1DataContext dc = new DataClasses1DataContext();

        // GET: api/Information
        public List<Information> Get()
        {
            var lista = from Information in dc.Informations select Information;

            return lista.ToList();
        }

        // GET: api/Information/Portugal
        [Route("api/information/{name}")]
        public IHttpActionResult Get(string name)
        {
            Information info = dc.Informations.FirstOrDefault(x => x.Name == name);

            if(info != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, info.Info));
            }
            else
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "No information is registered about this country"));
            }
        }

        // POST: api/Information
        public IHttpActionResult Post([FromBody]Information newInfo)
        {
            Information info = dc.Informations.FirstOrDefault(x => x.Name == newInfo.Name);

            if(info != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Information about this country is already registered!"));
            }
            else
            {
                dc.Informations.InsertOnSubmit(newInfo);
            }

            try
            {
                dc.SubmitChanges();
            }
            catch(Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
        }

        // PUT: api/Information
        public IHttpActionResult Put([FromBody]Information newInfo)
        {
            Information info = dc.Informations.FirstOrDefault(x => x.Name == newInfo.Name);

            if (info == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "There is no registered information about that country!"));
            }

            info.Info = newInfo.Info;

            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
        }

        // DELETE: api/Information/Portugal
        [Route("api/information/{name}")]
        public IHttpActionResult Delete(string name)
        {
            Information info = dc.Informations.FirstOrDefault(x => x.Name == name);

            if(info != null)
            {
                dc.Informations.DeleteOnSubmit(info);

                try
                {
                    dc.SubmitChanges();
                }
                catch (Exception e)
                {

                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e));
                }

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
            }
            else
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "There is no registered information about that country!"));
            }
        }
    }
}
