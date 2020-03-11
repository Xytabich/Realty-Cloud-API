using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RealtyCloudAPI
{
	internal class Request
	{
		public delegate void AsyncResponseCallback(bool success, object value, string errMsg);

		private HttpWebRequest request;

		public Request(string[] path, string[,] requestData, X509Certificate certificate = null)
		{
			StringBuilder sb = new StringBuilder(128);
			RequestBuilder(sb, path, requestData);

			request = (HttpWebRequest)WebRequest.Create(sb.ToString());
			request.Method = "GET";
			request.Timeout = 100000;
			request.Credentials = CredentialCache.DefaultCredentials;
			if(certificate != null) request.ClientCertificates = new X509CertificateCollection(new X509Certificate[] { certificate });
			else request.ServerCertificateValidationCallback += ValidateCertificate;
		}

		public void SetAPIKey(string key)
		{
			request.Headers.Set("API-Key", key);
		}

		#region POST
		public object POSTRequest(byte[] data)
		{
			request.Method = "POST";
			request.ContentType = "application/json";
			request.ContentLength = data.Length;
			request.GetRequestStream().Write(data, 0, data.Length);

			return SyncResponse(HttpStatusCode.Created);
		}

		public void POSTRequestAsync(byte[] data, AsyncResponseCallback callback)
		{
			if(callback == null) throw new ArgumentNullException("callback");

			request.Method = "POST";
			request.ContentType = "application/json";
			request.ContentLength = data.Length;
			request.BeginGetRequestStream(OnRequetStream, new RequestInfo() { data = data, callback = callback });
		}

		private void OnRequetStream(IAsyncResult ar)
		{
			var info = (RequestInfo)ar.AsyncState;
			try
			{
				info.stream = request.EndGetRequestStream(ar);
				info.stream.BeginWrite(info.data, 0, info.data.Length, OnWriteEnd, info);
			}
			catch(Exception)
			{
				info.InvokeMessage(false, "Async: get request stream error");
				throw;
			}
		}

		private void OnWriteEnd(IAsyncResult ar)
		{
			var info = (RequestInfo)ar.AsyncState;
			try
			{
				info.stream.EndWrite(ar);
				request.BeginGetResponse(OnResponse, new ResponseInfo() { callback = info.callback, responseCode = HttpStatusCode.Created });
			}
			catch(Exception)
			{
				info.InvokeMessage(false, "Async: get request stream error");
				throw;
			}
		}
		#endregion

		#region GET
		public object GETRequest()
		{
			return SyncResponse(HttpStatusCode.OK);
		}

		public void GETRequestAsync(AsyncResponseCallback callback)
		{
			if(callback == null) throw new ArgumentNullException("callback");
			request.BeginGetResponse(OnResponse, new ResponseInfo() { callback = callback, responseCode = HttpStatusCode.OK });
		}
		#endregion

		#region response
		private object SyncResponse(HttpStatusCode responseCode)
		{
			using(var response = (HttpWebResponse)request.GetResponse())
			{
				if(response.StatusCode == responseCode)
				{
					string str;
					using(var reader = new StreamReader(response.GetResponseStream()))
					{
						str = reader.ReadToEnd();
					}
					object obj;
					if(JSON.TryParse(str, out obj))
					{
						return obj;
					}
					else
					{
						System.Diagnostics.Debug.WriteLine("Invalid JSON string: " + str);
					}
					return null;
				}
				else
				{
					using(var reader = new StreamReader(response.GetResponseStream()))
					{
						throw new WebException("Network exception: " + (int)response.StatusCode + " " + reader.ReadToEnd(), null, WebExceptionStatus.UnknownError, response);
					}
				}
			}
		}

		private void OnResponse(IAsyncResult ar)
		{
			var info = (ResponseInfo)ar.AsyncState;
			try
			{
				var response = (HttpWebResponse)request.EndGetResponse(ar);
				if(response.StatusCode == info.responseCode)
				{
					info.response = response;
					info.dataStream = new MemoryStream(ResponseInfo.BUFFER_SIZE);
					info.dataBuffer = new byte[ResponseInfo.BUFFER_SIZE];
					info.responseStream = response.GetResponseStream();
					info.responseStream.BeginRead(info.dataBuffer, 0, ResponseInfo.BUFFER_SIZE, OnRead, info);
				}
				else
				{
					using(var reader = new StreamReader(response.GetResponseStream()))
					{
						throw new WebException("Network exception: " + (int)response.StatusCode + " " + reader.ReadToEnd(), null, WebExceptionStatus.UnknownError, response);
					}
				}
			}
			catch(Exception)
			{
				info.InvokeMessage(false, "Async: get response error");
				throw;
			}
		}

		private void OnRead(IAsyncResult ar)
		{
			var info = (ResponseInfo)ar.AsyncState;
			try
			{
				int read = info.responseStream.EndRead(ar);
				if(read > 0)
				{
					info.dataStream.Write(info.dataBuffer, 0, read);
					info.responseStream.BeginRead(info.dataBuffer, 0, ResponseInfo.BUFFER_SIZE, OnRead, info);
				}
				else
				{
					info.responseStream.Close();
					info.response.Close();

					if(info.dataStream.Length > 0)
					{
						info.dataStream.Position = 0;

						string str;
						using(var reader = new StreamReader(info.dataStream))
						{
							str = reader.ReadToEnd();
						}
						
						object obj;
						if(JSON.TryParse(str, out obj))
						{
							info.InvokeValue(obj);
						}
						else
						{
							System.Diagnostics.Debug.WriteLine("Invalid JSON string: " + str);
							info.InvokeMessage(true, "Invalid JSON string");
						}
					}
					else info.InvokeMessage(false, "Async: data is emty");
				}
			}
			catch(Exception)
			{
				info.InvokeMessage(false, "Async: read error");
				throw;
			}
		}
		#endregion

		private static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		private static void RequestBuilder(StringBuilder sb, string[] path, string[,] request)
		{
			for(int i = 0; i < path.Length; i++)
			{
				if(i > 0) sb.Append('/');
				sb.Append(path[i]);
			}
			if(request.Length > 0)
			{
				sb.Append('?');

				for(int i = 0; i < request.GetLength(0); i++)
				{
					sb.Append(request[i, 0]);
					sb.Append('=');
					sb.Append(Uri.EscapeDataString(request[i, 1]));
				}
			}
		}

		private class RequestInfo
		{
			public byte[] data;
			public Stream stream;
			public AsyncResponseCallback callback;

			public void InvokeMessage(bool success, string errMsg)
			{
				callback.Invoke(success, null, errMsg);
			}
		}

		private class ResponseInfo
		{
			public const int BUFFER_SIZE = 1024;

			public HttpStatusCode responseCode;
			public HttpWebResponse response;
			public Stream responseStream;
			public MemoryStream dataStream;
			public byte[] dataBuffer;

			public AsyncResponseCallback callback;

			public void InvokeMessage(bool success, string errMsg)
			{
				callback.Invoke(success, null, errMsg);
			}

			public void InvokeValue(object value)
			{
				callback.Invoke(true, value, "");
			}
		}
	}
}