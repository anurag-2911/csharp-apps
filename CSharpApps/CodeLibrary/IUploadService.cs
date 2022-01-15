using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace CodeLibrary
{
    [ServiceContract]
    public interface IUploadService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "testMessage?msg={msg}")]
        bool TestMessage(string msg);
    }
}
